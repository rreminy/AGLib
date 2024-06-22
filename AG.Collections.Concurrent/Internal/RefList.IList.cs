using System;
using System.Collections;
using System.Collections.Generic;

namespace AG.Collections.Concurrent.Internal
{
    internal sealed partial class RefList<T> : IList<T>, IReadOnlyList<T>
    {
        bool ICollection<T>.IsReadOnly => false;
        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        T IReadOnlyList<T>.this[int index] => this[index];

        int IList<T>.IndexOf(T item) => this.IndexOf(in item);
        void IList<T>.Insert(int index, T item) => this.Insert(index, in item);

        void ICollection<T>.Add(T item) => this.Add(in item);
        bool ICollection<T>.Contains(T item) => this.Contains(in item);
        bool ICollection<T>.Remove(T item) => this.Remove(in item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var span = this._array.AsSpan(0, this._count);
            span.CopyTo(array.AsSpan(arrayIndex));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var item in this) yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}
