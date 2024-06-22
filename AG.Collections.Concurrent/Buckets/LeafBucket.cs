using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AG.Collections.Concurrent.Internal;
using static AG.Collections.Concurrent.Buckets.Bucket;

namespace AG.Collections.Concurrent.Buckets
{
    internal sealed partial class LeafBucket<T> : IBucket<T> where T : notnull
    {
        private volatile uint _hasValues;
        private Entries _entries = default!;
        private volatile RefList<T>? _overflow;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BucketFindResult Find<TState>(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, BucketFindAction<TState, T>? action, ref TState? state, out T value)
        {
            // Step 0: Locals
            int index;
            var hasValues = this._hasValues;
            var entries = MemoryMarshal.CreateSpan(ref this._entries._element0, LeafBucketSize);

            // Step 1: Search entries
            // Figure out search bounds
            var count = LeafBucketSize - BitOperations.LeadingZeroCount(hasValues);
            index = BitOperations.TrailingZeroCount(hasValues);
            for (; index < count; index++)
            {
                // NOTE: Attempting to skip empty entry bits is
                // apparently slower than checking each entry bit
                if (!BitUtils.IsSet(hasValues, index)) continue;

                ref var entry = ref entries[index]!;
                if (comparer.Equals(item, entry))
                {
                    value = entry;
                    if ((flags & BucketFindOptions.Replace) != 0)
                    {
                        entry = item;
                        return RunActionAndReturnResult(BucketFindResult.Replaced, action, ref state, ref value);
                    }
                    else if ((flags & BucketFindOptions.Remove) != 0)
                    {
                        this._hasValues = BitUtils.UnSet(hasValues, index);
                        entry = default!;
                        return RunActionAndReturnResult(BucketFindResult.Removed, action, ref state, ref value);
                    }
                    return RunActionAndReturnResult(BucketFindResult.Found, action, ref state, ref value);
                }
            }

            // Step 2: Search overflow
            var overflow = this._overflow;
            if (overflow is not null)
            {
                index = overflow.IndexOf(item, comparer);
                if (index != -1)
                {
                    ref var entry = ref overflow[index];
                    value = entry;
                    if ((flags & BucketFindOptions.Replace) != 0)
                    {
                        entry = item;
                        return RunActionAndReturnResult(BucketFindResult.Replaced, action, ref state, ref value);
                    }
                    else if ((flags & BucketFindOptions.Remove) != 0)
                    {
                        overflow.RemoveAt(index);
                        return RunActionAndReturnResult(BucketFindResult.Removed, action, ref state, ref value);
                    }
                    return RunActionAndReturnResult(BucketFindResult.Found, action, ref state, ref value);
                }
            }

            // Step 3: Should we proceed creating an entry?
            if ((flags & BucketFindOptions.Create) == 0)
            {
                value = default!;
                return RunActionAndReturnResult(BucketFindResult.NotFound, action, ref state, ref value);
            }

            // Step 4: Search unused entry
            index = BitOperations.TrailingZeroCount(~hasValues);
            if (index < LeafBucketSize)
            {
                ref var entry = ref entries[index];
                this._hasValues = BitUtils.Set(hasValues, index);
                entry = item;
                value = default!;
                return RunActionAndReturnResult(BucketFindResult.Created, action, ref state, ref entry);
            }

            // Step 5: Use Overflow
            if (overflow is null) this._overflow = overflow = new();
            overflow.Add(in item);
            value = default!;
            return RunActionAndReturnResult(BucketFindResult.Created, action, ref state, ref overflow[^1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BucketFindResult Find(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, BucketFindAction<T>? action, out T value)
        {
            static void FindActionWithNoState(BucketFindResult result, ref BucketFindAction<T>? state, ref T entry)
            {
                state?.Invoke(result, ref entry);
            }
            return this.Find(in item, hash, comparer, flags, action is null ? null : FindActionWithNoState, ref action, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BucketFindResult Find(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, out T value)
        {
            return this.Find(in item, hash, comparer, flags, null, out value);
        }

        public long Clear()
        {
            var count = (long)BitOperations.PopCount(this._hasValues);
            if (count > 0)
            {
                this._hasValues = 0;
                MemoryMarshal.CreateSpan(ref this._entries[0], LeafBucketSize).Clear();
            }

            var overflow = this._overflow;
            if (overflow is not null)
            {
                count += overflow.Count;
                overflow.Clear();
            }

            return count;
        }

        /// <summary>Faster than <see cref="GetCount"/> but does not consider overflow.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCountFast()
        {
            // A faster version of GetCount doing only PopCount
            // and no conversion to long
            return BitOperations.PopCount(this._hasValues);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetCount()
        {
            var count = (long)BitOperations.PopCount(this._hasValues);
            var overflow = this._overflow;
            if (overflow is not null)
            {
                count += overflow.Count;
            }
            return count;
        }

        public void ClearAndTrim()
        {
            if (BitOperations.PopCount(this._hasValues) > 0)
            {
                this._hasValues = 0;
                MemoryMarshal.CreateSpan(ref this._entries[0], LeafBucketSize).Clear();
            }
            if (this._overflow is not null) this._overflow = null;
        }

        public struct LeafBucketEnumerator
        {
            private readonly LeafBucket<T> _bucket;
            private readonly uint _hasValues;
            private readonly int _count;
            private int _index = -1;

            internal LeafBucketEnumerator(LeafBucket<T> bucket)
            {
                this._bucket = bucket;
                this._hasValues = bucket._hasValues;

                this._index = BitOperations.TrailingZeroCount(this._hasValues) - 1;
                this._count = LeafBucketSize - BitOperations.LeadingZeroCount(this._hasValues);
            }

            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (this._index < this._count) return ref this._bucket._entries[this._index];
                    return ref this._bucket._overflow![this._index - this._count];
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (true)
                {
                    var index = ++this._index;
                    if (index < this._count)
                    {
                        if (BitUtils.IsSet(this._hasValues, index)) return true;
                    }
                    else return (index - this._count) < this._bucket._overflow?.Count;
                }
            }
        }

        public LeafBucketEnumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var entry in this) yield return entry;
        }
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
