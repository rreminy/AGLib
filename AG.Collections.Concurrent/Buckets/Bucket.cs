using System;
using System.Collections.Generic;
using System.Threading;
using AG.Collections.Concurrent.Internal;

namespace AG.Collections.Concurrent.Buckets
{
    /// <summary>Helper static methods for buckets.</summary>
    internal static class Bucket
    {
        /// <summary>Leaf buckets size.</summary>
        public const int LeafBucketSize = 32; // Min = 1, Max = 32

        // Evaluated bucket Sizes:
        // - For ints: A Leaf bucket size of 32 seems to have the best performance.
        // - For strings: TODO

        /// <summary>Get <see cref="TrieBucket{T}"/>'s item count.</summary>
        /// <typeparam name="T"><see cref="IBucket{T}"/> element type.</typeparam>
        /// <param name="start">Starting <see cref="IBucket{T}"/>.</param>
        /// <remarks>Time: <c>O(n)</c>.</remarks>
        /// <returns>Item count.</returns>
        public static long GetItemCount<T>(IBucket<T> start) where T : notnull
        {
            var count = 0L;
            if (start is TrieBucket<T> trieBucket)
            {
                foreach (var bucket in trieBucket._buckets)
                {
                    if (bucket is null) continue;
                    count += GetItemCount(bucket);
                }
                return count;
            }
            else if (start is LeafBucket<T> leafBucket)
            {
                // Leaf buckets got their own method
                return leafBucket.GetCount();
            }

            BucketException.Throw($"Unknown bucket type for {nameof(GetItemCount)}: {nameof(T)}"); // Should not happen
            return 0; // Not reached
        }

        /// <summary>Gets an enumerator enumerating all items starting from <paramref name="start"/>.</summary>
        /// <typeparam name="T"><see cref="IBucket{T}"/> element type.</typeparam>
        /// <param name="start">Starting <see cref="IBucket{T}"/>.</param>
        /// <remarks>Time: <c>O(n)</c>.</remarks>
        /// <returns>An enumerator enumerating all items starting from <paramref name="start"/>.</returns>
        public static IEnumerator<T> GetBucketEnumerator<T>(IBucket<T> start) where T : notnull
        {
            var stack = new Stack<IBucket<T>>();
            var list = new RefList<T>(); // used as a buffer

            // Lets start with ourselves
            stack.Push(start);

            // Go throughout all buckets
            while (stack.TryPop(out var bucketObj))
            {
                // Check if this is a leaf bucket.
                if (bucketObj is LeafBucket<T> leafBucket)
                {
                    // This looks like a leaf bucket. Lets lock it then check again.
                    lock (leafBucket)
                    {
                        // This is a leaf bucket!

                        // Do not use yield return as we're inside a lock and instead
                        // use a list buffer to yield return from outside of the lock.

                        // The list effectively becomes a mini-snapshot of the bucket
                        // contents at this point of time.
                        static void BufferEntries(LeafBucket<T> leafBucket, RefList<T> list)
                        {
                            // Enumerator used here is a struct enumerator,
                            // no allocations should happen here.
                            foreach (ref var entry in leafBucket) list.Add(in entry);
                        }
                        BufferEntries(leafBucket, list);
                    }
                    if (list.Count > 0)
                    {
                        // Yield return all the contents of the list buffer then
                        // clear it for re-use at the next leaf bucket we find.

                        // Enumerator used here is a struct enumerator,
                        // no allocations should happen here.
                        foreach (var item in list) yield return item;
                        list.Clear();
                    }
                }
                else if (bucketObj is TrieBucket<T> trieBucket)
                {
                    // This is not a leaf bucket.
                    // Push to stack its inner buckets in reverse order to preserve
                    // hash-ordered consistency while poppint them out.
                    static void PushBuckets(TrieBucket<T> bucket, Stack<IBucket<T>> stack)
                    {
                        for (var index = 15; index >= 0; index--)
                        {
                            ref var bucketRef = ref bucket._buckets[index];
                            var innerBucket = bucketRef ?? Volatile.Read(ref bucketRef);
                            if (innerBucket is null) continue;
                            stack.Push(innerBucket);
                        }
                    }
                    PushBuckets(trieBucket, stack);
                }
                else
                {
                    BucketException.Throw($"Unknown bucket type for {nameof(GetBucketEnumerator)}: {nameof(T)}"); // Should not happen
                }
            }
        }

        public static BucketFindResult RunActionAndReturnResult<TState, T>(BucketFindResult result, BucketFindAction<TState, T>? action, ref TState? state, ref T value)
        {
            action?.Invoke(result, ref state, ref value);
            return result;
        }
    }
}
