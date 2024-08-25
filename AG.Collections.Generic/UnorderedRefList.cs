using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AG.Collections.Generic
{
    /// <summary>A very simple list with a struct ref enumerator, ref indexer, direct access to its internal array and <c>O(1)</c> insertion and removal.</summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Concrete methods and indexer makes use of refs.</item>
    /// <item>Insertion works by moving the item at index to the end.</item>
    /// <item>Removal works by moving the last item to index.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T"><see cref="UnorderedRefList{T}"/> element type.</typeparam>
    public sealed partial class UnorderedRefList<T>
    {
        private const int InitialSize = 4;
        private static readonly IEqualityComparer<T> s_defaultComparer = EqualityComparer<T>.Default;

        private T[] _array;
        private int _count;

        public UnorderedRefList()
        {
            this._array = [];
        }

        public UnorderedRefList(int capacity)
        {
            this._array = capacity == 0 ? [] : new T[capacity];
        }

        public UnorderedRefList(IEnumerable<T> source)
        {
            if (source.TryGetNonEnumeratedCount(out var count)) // ASSERT: Returned count is correct
            {
                if (count is 0)
                {
                    this._array = [];
                    return;
                }
                var array = GC.AllocateUninitializedArray<T>((int)BitOperations.RoundUpToPowerOf2((uint)count));
                var index = 0;
                foreach (var item in source)
                {
                    array[index++] = item;
                }
                Array.Clear(array, index, array.Length - index);
                this._array = array;
                this._count = index;
            }
            else
            {
                this._array = [];
                this.AddRange(source);
            }
        }

        public T[] InternalArray
        {
            [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Intentional.")]
            get => this._array;
            set
            {
                this._array = value;
                if (this._count > value.Length) this._count = value.Length;
            }
        }

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

        public ref T this[int index] => ref this._array[index];

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
            var span = this._array.AsSpan(0, this._count);
            for (var index = 0; index < span.Length; index++)
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

        /// <summary>Same as <see cref="Clear"/> but also sets an empty array.</summary>
        public void Reset()
        {
            this._array = [];
            this._count = 0;
        }

        public Span<T>.Enumerator GetEnumerator() => this._array.AsSpan(0, this._count).GetEnumerator();
    }
}
