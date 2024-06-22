using System.Collections.Generic;

namespace AG.Collections.Concurrent.Buckets
{
    /// <summary>Represents a bucket participating in a Trie data structure.</summary>
    /// <typeparam name="T"><see cref="IBucket{T}"/> element type.</typeparam>
    internal interface IBucket<T> : IEnumerable<T> where T : notnull
    {
        /// <summary>Finds an <paramref name="item"/> with a specified <paramref name="hash"/> using a <paramref name="comparer"/>.</summary>
        /// <param name="item">Item to look for.</param>
        /// <param name="hash">Hash to use for trie traversal.</param>
        /// <param name="comparer">Comparer to use for comparison.</param>
        /// <param name="flags">Find options.</param>
        /// <param name="value">Item if found item, original item if replaced or removed, default if not found.</param>
        /// <returns><see cref="BucketFindResult"/> representing the result of the operation.</returns>
        public BucketFindResult Find(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, out T value);

        /// <summary>Finds an <paramref name="item"/> with a specified <paramref name="hash"/> using a <paramref name="comparer"/>.</summary>
        /// <param name="item">Item to look for.</param>
        /// <param name="hash">Hash to use for trie traversal.</param>
        /// <param name="comparer">Comparer to use for comparison.</param>
        /// <param name="flags">Find options.</param>
        /// <param name="action">Action to execute if found.</param>
        /// <param name="value">Item if found item, original item if replaced or removed, default if not found.</param>
        /// <returns><see cref="BucketFindResult"/> representing the result of the operation.</returns>
        public BucketFindResult Find(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, BucketFindAction<T>? action, out T value);

        /// <summary>Finds an <paramref name="item"/> with a specified <paramref name="hash"/> using a <paramref name="comparer"/>.</summary>
        /// <typeparam name="TState">Action's state type.</typeparam>
        /// <param name="item">Item to look for.</param>
        /// <param name="hash">Hash to use for trie traversal.</param>
        /// <param name="comparer">Comparer to use for comparison.</param>
        /// <param name="flags">Find options.</param>
        /// <param name="action">Action to execute if found.</param>
        /// <param name="state">Action's state.</param>
        /// <param name="value">Item if found item, original item if replaced or removed, default if not found.</param>
        /// <returns><see cref="BucketFindResult"/> representing the result of the operation.</returns>
        public BucketFindResult Find<TState>(in T item, int hash, IEqualityComparer<T> comparer, BucketFindOptions flags, BucketFindAction<TState, T>? action, ref TState? state, out T value);

        /// <summary>Counts the number of items inside this <see cref="IBucket{T}"/>.</summary>
        /// <returns>A value representing the item count inside this <see cref="IBucket{T}"/>.</returns>
        public long GetCount();

        /// <summary>Remove all items inside this <see cref="IBucket{T}"/>.</summary>
        /// <returns>A value representing the item count cleared inside this <see cref="IBucket{T}"/>.</returns>
        public long Clear();

        /// <summary>Remove all items inside this <see cref="IBucket{T}"/>.</summary>
        /// <remarks>This is faster than <see cref="Clear"/> but resets the trie structure in the process.</remarks>
        public void ClearAndTrim();
    }
}