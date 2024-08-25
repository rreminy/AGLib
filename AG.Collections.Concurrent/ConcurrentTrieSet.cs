using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AG.Collections.Concurrent.Buckets;

namespace AG.Collections.Concurrent
{
    /// <summary>An implementation of a concurrent <see cref="ISet{T}"/> using a trie data structure.</summary>
    /// <typeparam name="T"><see cref="ConcurrentTrieSet{T}"/> element type.</typeparam>
    public sealed class ConcurrentTrieSet<T> : ISet<T>, IReadOnlySet<T> where T : notnull
    {
        private readonly TrieBucket<T> _rootBucket = new();
        private readonly IEqualityComparer<T> _comparer;
        private readonly ConcurrentCounter _counter = new();

        /// <summary>Initializes a new instance of the <see cref="ConcurrentTrieSet{T}"/> class using the default comparer.</summary>
        public ConcurrentTrieSet() : this(null, null) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrentTrieSet{T}"/> class using the default comparer containing elements from <paramref name="source"/>.</summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> to copy elements from.</param>
        public ConcurrentTrieSet(IEnumerable<T>? source) : this(source, null) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrentTrieSet{T}"/> class using the specified <paramref name="comparer"/>.</summary>
        /// <param name="comparer">Comparer to use for this <see cref="ConcurrentTrieSet{T}"/>.</param>
        public ConcurrentTrieSet(IEqualityComparer<T>? comparer) : this(null, comparer) { }

        /// <summary>Initializes a new instance of the <see cref="ConcurrentTrieSet{T}"/> class using the specified <paramref name="comparer"/>.</summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <param name="comparer">Comparer to use for this <see cref="ConcurrentTrieSet{T}"/>.</param>
        public ConcurrentTrieSet(IEnumerable<T>? source, IEqualityComparer<T>? comparer)
        {
            this._comparer = comparer ?? EqualityComparer<T>.Default;
            if (source is not null) this.AddRange(source);
        }

        /// <summary>Gets item count.</summary>
        public int Count => (int)this._counter.GetCount();

        /// <summary>Gets item count as a <see cref="long"/>.</summary>
        public long LongCount => this._counter.GetCount();

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>Gets item count as a <see cref="long"/> by iterating through all trie structure.</summary>
        /// <returns>Item count.</returns>
        public long SlowCount() => this._rootBucket.GetCount();

        /// <inheritdoc/>
        public bool Add(T item)
        {
            var comparer = this._comparer;
            var hash = comparer.GetHashCode(item);
            if (this._rootBucket.Find(item, hash, comparer, BucketFindOptions.Create, out _) is BucketFindResult.Created)
            {
                this._counter.Increment(hash);
                return true;
            }
            return false;
        }

        /// <summary>Adds a range of elements to the current set.</summary>
        /// <param name="items">The elements to add to the set..</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items) this.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this._counter.Sub(this._rootBucket.Clear());
        }

        /// <summary>Removes all items from the <see cref="ConcurrentTrieSet{T}"/> and trim the trie structure.</summary>
        public void ClearAndTrim()
        {
            this._rootBucket.ClearAndTrim();
            this._counter.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            var comparer = this._comparer;
            var hash = comparer.GetHashCode(item);
            return this._rootBucket.Find(item, hash, comparer, BucketFindOptions.None, out _) is BucketFindResult.Found;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var entry in this._rootBucket)
            {
                array[arrayIndex++] = entry;
            }
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                this.Clear();
                return;
            }
            foreach (var item in other)
            {
                this.Remove(item);
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in this._rootBucket)
            {
                yield return entry;
            }
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (this.LongCount == 0) return;
            if (!other.Any())
            {
                this.Clear();
                return;
            }
            foreach (var item in this)
            {
                if (!other.Contains(item)) this.Remove(item);
            }
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return this.LongCount < other.Distinct().LongCount() && this.All(other.Contains);
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return other.Distinct().LongCount() < this.LongCount && other.All(this.Contains);
        }

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return this.All(other.Contains);
        }

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return other.All(this.Contains);
        }

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Any(this.Contains);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var comparer = this._comparer;
            var hash = comparer.GetHashCode(item);
            if (this._rootBucket.Find(item, hash, comparer, BucketFindOptions.Remove, out _) is BucketFindResult.Removed)
            {
                this._counter.Decrement(hash);
                return true;
            }
            return false;
        }

        /// <summary>Removes a range of items from the current <see cref="ConcurrentTrieSet{T}"/>.</summary>
        /// <param name="items">Items to remove.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items) this.Remove(item);
        }

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other)
        {
            return this.LongCount == other.LongCount() && other.Distinct().All(this.Contains);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                this.Clear();
                return;
            }
            foreach (var item in other)
            {
                if (!this.Remove(item)) this.Add(item);
            }
        }

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            if (ReferenceEquals(this, other)) return;
            foreach (var item in other)
            {
                this.Add(item);
            }
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item)
        {
            if (!this.Add(item)) throw new ArgumentException("Item already exists", nameof(item));
        }

        /// <summary>Searches the <see cref="ConcurrentTrieSet{T}"/> for a specific value and returns the equal value if any.</summary>
        /// <param name="value">Value to search for.</param>
        /// <returns><c>true</c> if found, <c>false</c> if not found.</returns>
        public bool TryGetValue(in T value, out T actualValue)
        {
            var comparer = this._comparer;
            return this._rootBucket.Find(value, comparer.GetHashCode(value), comparer, BucketFindOptions.None, out actualValue) == BucketFindResult.Found;
        }

        /// <summary>Gets or adds a value into the current <see cref="ConcurrentTrieSet{T}"/>.</summary>
        /// <param name="value">Value to add.</param>
        /// <returns><paramref name="value"/> if added, existing value otherwise.</returns>
        public T GetOrAdd(in T value)
        {
            var comparer = this._comparer;
            return this._rootBucket.Find(value, comparer.GetHashCode(value), comparer, BucketFindOptions.Create, out var result) is BucketFindResult.Created ? value : result;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
