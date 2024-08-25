using System;
using System.Collections;
using System.Collections.Generic;

namespace AG.Collections.Generic
{
    /// <content><see cref="IList{T}"/> and <see cref="IReadOnlyList{T}"/> implementation for <see cref="UnorderedRefList{T}"/>.</content>
    public sealed partial class UnorderedRefList<T> : IList<T>, IReadOnlyList<T>
    {
        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc/>
        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => this[index];

        /// <inheritdoc/>
        int IList<T>.IndexOf(T item) => this.IndexOf(in item);

        /// <inheritdoc/>
        void IList<T>.Insert(int index, T item) => this.Insert(index, in item);

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(in item);

        /// <inheritdoc/>
        bool ICollection<T>.Contains(T item) => this.Contains(in item);

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item) => this.Remove(in item);

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var span = this._array.AsSpan(0, this._count);
            span.CopyTo(array.AsSpan(arrayIndex));
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (var index = 0; index < this._count; index++) yield return this[index];
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
