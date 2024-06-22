using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AG.Collections.Concurrent.Internal
{
    /// <summary>A very simple list with a struct ref enumerator, ref indexer, direct access to its internal array and O(1) insertion and removal</summary>
    /// <remarks>Insertion and Removal differently than other lists</remarks>
    internal sealed partial class RefList<T>
    {
        private const int InitialSize = 4;
        private readonly IEqualityComparer<T> s_defaultComparer = EqualityComparer<T>.Default;

        private T[] _array;
        private int _count;

        public RefList()
        {
            this._array = Array.Empty<T>();
        }

        public RefList(int capacity)
        {
            this._array = capacity == 0 ? Array.Empty<T>() : new T[capacity];
        }

        public RefList(IEnumerable<T> source)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var array = GC.AllocateUninitializedArray<T>((int)BitOperations.RoundUpToPowerOf2((uint)count));
                var index = 0;
                foreach (var item in source)
                {
                    array[index++] = item;
                }
                Array.Clear(array, count, array.Length - count);
                this._array = array;
                this._count = count;
            }
            else
            {
                this._array = Array.Empty<T>();
                this.AddRange(source);
            }
        }

        public T[] InternalArray => this._array;
        public int Count
        {
            get => this._count;
            set
            {
                var length = this._array.Length;
                if (value > length)
                {
                    var newLength = (int)BitOperations.RoundUpToPowerOf2((uint)value);
                    Array.Resize(ref this._array, newLength);
                    this._count = value;
                }
                else 
                {
                    var count = this._count;
                    if (value < count) Array.Clear(this._array, value, count - value);
                    this._count = value;
                }
            }
        }

        public int Capacity
        {
            get => this._array.Length;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, this._count);
                Array.Resize(ref this._array, value);
            }
        }

        public void Add(in T item)
        {
            var index = this._count++;
            if (index >= this._array.Length)
            {
                var newLength = Math.Max(InitialSize, checked(this._array.Length * 2));
                Array.Resize(ref this._array, newLength);
            }
            this._array[index] = item;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items) this.Add(item);
        }

        public int IndexOf(in T item, IEqualityComparer<T> comparer)
        {
            //var hash = comparer.GetHashCode(item!);
            var span = this._array.AsSpan(0, this._count);
            for (int index = 0; index < span.Length; index++)
            {
                ref var spanItem = ref span[index];
                if (comparer.Equals(item, spanItem)) return index;
            }
            return -1;
        }
        public int IndexOf(in T item) => this.IndexOf(in item, s_defaultComparer);

        public void Insert(int index, in T item)
        {
            this.Add(in this[index]);
            this[index] = item;
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            foreach (var item in items) this.Insert(index++, item);
        }

        public bool Contains(in T item, IEqualityComparer<T> comparer) => this.IndexOf(in item, comparer) != -1;
        public bool Contains(in T item) => this.Contains(in item, s_defaultComparer);

        public bool Remove(in T item, IEqualityComparer<T> comparer)
        {
            var index = this.IndexOf(item, comparer);
            if (index == -1) return false;
            this.RemoveAt(index);
            return true;
        }
        public bool Remove(in T item) => this.Remove(in item, s_defaultComparer);

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            foreach (var item in items) this.Remove(item, comparer);
        }
        public void RemoveRange(IEnumerable<T> items) => this.RemoveRange(items, s_defaultComparer);

        public void RemoveAt(int index)
        {
            this._array[index] = this._array[--this._count];
            this._array[this._count] = default!;
        }

        public ref T this[int index] => ref this._array[index];

        public Span<T> AsSpan() => this.AsSpan(0, this._count);
        public Span<T> AsSpan(int start) => this.AsSpan(start, this._count - start);
        public Span<T> AsSpan(int start, int length) => this._array.AsSpan(start, length);

        public Memory<T> AsMemory() => this._array.AsMemory(0, this._count);
        public Memory<T> AsMemory(int start) => this._array.AsMemory(start, this._count - start);
        public Memory<T> AsMemory(int start, int length) => this._array.AsMemory(start, length);

        public void Clear()
        {
            Array.Clear(this._array, 0, this._count);
            this._count = 0;
        }

        /// <summary>Same as <see cref="Clear"/> but also sets an empty array</summary>
        public void Reset()
        {
            this._array = Array.Empty<T>();
            this._count = 0;
        }

        public struct OverflowEnumerator(RefList<T> _list)
        {
            private readonly int _count = _list._count;
            private int _index = -1;

            public bool MoveNext() => ++this._index < _count;
            public readonly ref T Current => ref _list[this._index];
            public void Reset() => this._index = -1;
        }

        public OverflowEnumerator GetEnumerator() => new OverflowEnumerator(this);
    }
}
