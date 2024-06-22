using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using static AG.Collections.Concurrent.Buckets.Bucket;

namespace AG.Collections.Concurrent.Buckets
{
    //[SuppressMessage("Critical Bug", "S2551", Justification = "Internal, not exposed")]
    internal sealed partial class TrieBucket<T> : IBucket<T> where T : notnull
    {
        private readonly int _level;
        internal Buckets _buckets;

        public TrieBucket() : this(0) { }
        public TrieBucket(int level)
        {
            Debug.Assert(level <= 8);
            this._level = level;
        }

        /// <remarks>Time: <c>O(log(n))</c></remarks>
        [SuppressMessage("Major Code Smell", "S907", Justification = "gotos")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BucketFindResult Find<TState>(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, BucketFindAction<TState, T>? action, ref TState? state, out T value)
        {
            var index = this.GetHashIndex(hash);

            ref var bucketRef = ref this._buckets[index];
            var bucket = bucketRef ?? Volatile.Read(ref bucketRef);

            if (bucket is null)
            {
                // No bucket has been allocated here before.

                // If not creationg an entry, simply return NotFound
                if ((flags & BucketFindOptions.Create) == 0)
                {
                    value = default!;
                    return BucketFindResult.NotFound;
                }

                // Lets create a new leaf bucket
                static IBucket<T> CreateBucket(ref IBucket<T>? bucketRef)
                {
                    var newBucket = new LeafBucket<T>();
                    return Interlocked.CompareExchange(ref bucketRef, newBucket, null) ?? newBucket;
                }
                bucket = CreateBucket(ref bucketRef);
            }

            if (bucket is LeafBucket<T> leafBucket)
            {
                Monitor.Enter(leafBucket);
                // A leaf bucket has been allocated here.

                // Leaf buckets always need to be locked,
                // lets lock it before operating onto it

                // Is the bucket being modified?
                if ((flags & BucketFindOptions.ModifyingFlags) != 0)
                {
                    // Lets make sure we're modifying the correct bucket
                    bucket = Volatile.Read(ref bucketRef);
                    if (!ReferenceEquals(leafBucket, bucket))
                    {
                        Monitor.Exit(leafBucket);
                        goto find_recurse_bucket;
                    }

                    // Will an entry be possibly created on a bucket at capacity?
                    if ((flags & BucketFindOptions.Create) != 0 && this._level is not 8 && leafBucket.GetCountFast() is LeafBucketSize)
                    {
                        // Convert to trie bucket to avoid overflow
                        static IBucket<T> ConvertToTrieBucket(LeafBucket<T> leafBucket, int level, IEqualityComparer<T> comparer, BucketFindAction<TState, T>? action, ref TState? state)
                        {
                            var bucket = new TrieBucket<T>(level);
                            foreach (ref var entry in leafBucket)
                            {
                                bucket.Find(in entry, comparer.GetHashCode(entry), comparer, BucketFindOptions.Create, action, ref state, out _);
                            }
                            return bucket;
                        }
                        bucket = ConvertToTrieBucket(leafBucket, this._level + 1, comparer, action, ref state);

                        // Set the new bucket and retry loop
                        Volatile.Write(ref bucketRef, bucket);
                        Monitor.Exit(leafBucket);
                        goto find_recurse_bucket;
                    }
                }

                // Recurse into the leaf bucket (locked)
                var result = leafBucket.Find(in item, hash, comparer, flags, out value);
                Monitor.Exit(leafBucket);
                return result;
            }

find_recurse_bucket:
            // Recurse into trie the bucket
            return Unsafe.As<TrieBucket<T>>(bucket)!.Find(in item, hash, comparer, flags, action, ref state, out value);
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

        /// <summary>Enumerate items from all buckets</summary>
        /// <remarks>Time: <c>O(n)</c></remarks>
        public IEnumerator<T> GetEnumerator() => GetBucketEnumerator(this);

        /// <returns>Cleared item count</returns>
        /// <remarks>Time: <c>O(n)</c></remarks>
        public long Clear()
        {
            var count = 0L;
            foreach (ref var bucketRef in this._buckets)
            {
                while (true)
                {
                    var bucket = bucketRef;
                    if (bucket is null) break;

                    if (bucket is LeafBucket<T>)
                    {
                        lock (bucket)
                        {
                            if (bucket != Volatile.Read(ref bucketRef)) continue;
                            count += bucket.Clear();
                        }
                    }
                    else count += bucket.Clear();
                    break;
                }

            }
            return count;
        }

        /// <remarks>Time: <c>O(1)</c></remarks>
        public void ClearAndTrim()
        {
            MemoryMarshal.CreateSpan(ref this._buckets[0], 16).Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Get <see cref="TrieBucket{T}"/>'s item count</summary>
        /// <remarks>Time: <c>O(n)</c></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetCount() => GetItemCount(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetHashIndex(int hash) => GetHashIndex(hash, this._level);
    }
}
