using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AG;

namespace AG.Collections.Generic
{
    /// <summary>A, unreliable set forgetting items at random.</summary>
    /// <remarks>Implemented internally as a hash list. <see cref="Add(T)"/> will replace items on collisions.</remarks>
    /// <typeparam name="T">Items type.</typeparam>
    public sealed partial class ForgetfulSet<T> : ISet<T>, IReadOnlySet<T>
    {
        private readonly T[] _array;
        private readonly bool[] _occupied;
        private readonly ulong _fastModMultiplier;
        private readonly IEqualityComparer<T> _comparer;
        private readonly int _capacity;
        private int _count;

        /// <summary>Initializes a new instance of the <see cref="ForgetfulSet{T}"/> class.</summary>
        /// <param name="capacity"><see cref="ForgetfulSet{T}"/> capacity.</param>
        /// <param name="items">Starting items.</param>
        /// <param name="comparer">Equality comparer.</param>
        public ForgetfulSet(int capacity, IEnumerable<T>? items = null, IEqualityComparer<T>? comparer = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, 0);
            this._fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)capacity);
            this._capacity = capacity;

            this._array = new T[capacity];
            this._occupied = new bool[capacity];
            this._comparer = comparer ?? EqualityComparer<T>.Default;

            if (items is not null) this.AddRange(items);
        }

        /// <summary>Initializes a new instance of the <see cref="ForgetfulSet{T}"/> class.</summary>
        /// <param name="capacity"><see cref="ForgetfulSet{T}"/> capacity.</param>
        /// <param name="comparer">Equality comparer.</param>
        public ForgetfulSet(int capacity, IEqualityComparer<T>? comparer) : this(capacity, null, comparer) { }

        /// <inheritdoc/>
        public int Count => this._count;

        /// <summary>Gets a value indicating this <see cref="ForgetfulSet{T}"/>'s capacity.</summary>
        public int Capacity => this._capacity;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public bool Add(T item)
        {
            this.FindInternal(item, FindMode.Add, out var added);
            return added;
        }

        /// <summary>Adds multiple items into this <see cref="ForgetfulSet{T}"/>.</summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items) this.Add(item);
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            this.RemoveRange(other);
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Any(this.Contains);
        }

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            this.AddRange(other);
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <inheritdoc/>
        public void Clear()
        {
            Array.Clear(this._array);
            Array.Clear(this._occupied);
            this._count = 0;
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            this.FindInternal(item, FindMode.Get, out var exists);
            return exists;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            this.FindInternal(item, FindMode.Remove, out var removed);
            return removed;
        }

        /// <summary>Removes multiple items from this <see cref="ForgetfulSet{T}"/>.</summary>
        /// <param name="items">Items to remove.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items) this.Remove(item);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            var capacity = this._capacity;
            for (var index = 0; index < capacity; index++)
            {
                if (this._occupied[index]) yield return this._array[index];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetHashIndex(int hashCode) => (int)HashHelpers.FastMod((uint)hashCode, (uint)this._capacity, this._fastModMultiplier);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref T FindInternal(in T value, FindMode mode, out bool result)
        {
            var index = this.GetHashIndex(this._comparer.GetHashCode(value!));
            ref var item = ref this._array[index];
            ref var occupied = ref this._occupied[index];
            if (mode == FindMode.Get) result = occupied && this._comparer.Equals(value, item);
            else if (mode == FindMode.Add)
            {
                if (!occupied || this._comparer.Equals(value, item))
                {
                    this._count++;
                    result = true;
                    item = value;
                    occupied = true;
                }
                else result = false;
            }
            else if (mode == FindMode.Remove)
            {
                if (occupied && this._comparer.Equals(value, item))
                {
                    this._count--;
                    result = true;
                    item = default;
                    occupied = false;
                }
                else result = false;
            }
            else
            {
                result = false;
            }
            return ref item!;
        }
    }
}
