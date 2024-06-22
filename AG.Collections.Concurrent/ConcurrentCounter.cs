using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace AG.Collections.Concurrent
{
    // Loosely based on: https://github.com/VSadov/NonBlocking/blob/main/src/NonBlocking/Counter/Counter64.cs
    // - Additional method that uses a hash value instead of deriving the address of the cellCount variable
    // - Expand cell counts to prime sizes

    /// <summary>A concurrent counter keeping track of its count using concurrent <see cref="Interlocked"/> based counters</summary>
    public sealed class ConcurrentCounter
    {
        private sealed class Cell
        {
            [InlineArray(16)] // Cheaty way to get a whole cacheline worth of longs
            internal struct CellStruct
            {
                [SuppressMessage("Major Code Smell", "S1144", Justification = "Inline Array")]
                public long _element0;
            }
            public CellStruct Counter = default!;
        }

        private Cell[]? _cells;
        [SuppressMessage("Minor Code Smell", "S2933", Justification = "Modified via ref")]
        private long _count = 0;

        private SpinLock _estimatedLock = new(false);
        private long _estimatedTimestamp;
        private long _estimatedCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref long GetCountRef()
        {
            var cells = this._cells;
            if (cells is null) return ref this._count;
            return ref cells[GetCountIndex(cells.Length)].Counter[7];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref long GetCountRef(int hash)
        {
            var cells = this._cells;
            if (cells is null) return ref this._count;
            return ref cells[(uint)hash % cells.Length].Counter[7];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetCountIndex(int cellCount)
        {
            var addr = (nuint)(&cellCount);
            return (int)(addr % (nuint)cellCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Increment(ref long count)
        {
            return -count - 1 + Interlocked.Increment(ref count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Decrement(ref long count)
        {
            return -count + 1 + Interlocked.Decrement(ref count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Add(ref long count, long value)
        {
            return -count - value + Interlocked.Add(ref count, value);
        }

        private static void TryAddCells(ref Cell[]? cellsRef)
        {
            var cells = cellsRef;
            var length = cells?.Length ?? 0;

            if (length > Environment.ProcessorCount * 2) return;

            Cell[]? newCells;
            var newLength = Math.Max(3, MathUtils.FindNextPrime(length + 2));

            newCells = new Cell[newLength];
            cells?.CopyTo(newCells, 0);
            for (var index = length; index < newLength; index++) newCells[index] = new();

            Interlocked.CompareExchange(ref cellsRef, newCells, cells);
        }

        /// <summary>Increments this <see cref="ConcurrentCounter"/></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Increment()
        {
            if (Increment(ref this.GetCountRef()) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Increments this <see cref="ConcurrentCounter"/></summary>
        /// <param name="hash">Hash to use for internal counter selection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Increment(int hash)
        {
            if (Increment(ref this.GetCountRef(hash)) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Decrements this <see cref="ConcurrentCounter"/></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Decrement()
        {
            if (Decrement(ref this.GetCountRef()) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Decrements this <see cref="ConcurrentCounter"/></summary>
        /// <param name="hash">Hash to use for internal counter selection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Decrement(int hash)
        {
            if (Decrement(ref this.GetCountRef(hash)) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Adds <paramref name="value"/> to this counter</summary>
        /// <param name="value">Value to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(long value)
        {
            if (Add(ref this.GetCountRef(), value) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Adds <paramref name="value"/> to this counter</summary>
        /// <param name="value">Value to add</param>
        /// <param name="hash">Hash to use for internal counter selection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(long value, int hash)
        {
            if (Add(ref this.GetCountRef(hash), value) != 0) TryAddCells(ref this._cells);
        }

        /// <summary>Substracts <paramref name="value"/> to this counter</summary>
        /// <param name="value">Value to substract</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sub(long value) => this.Add(-value);

        /// <summary>Substracts <paramref name="value"/> to this counter</summary>
        /// <param name="value">Value to substract</param>
        /// <param name="hash">Hash to use for internal counter selection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sub(long value, int hash) => this.Add(-value, hash);

        /// <summary>Gets count</summary>
        /// <returns>Count</returns>
        public long GetCount()
        {
            var count = 0L;
            count += this._count;
            var cells = this._cells;
            if (cells is not null)
            {
                for (var index = 0; index < cells.Length; index++)
                {
                    count += cells[index].Counter[7];
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetEstimatedCount()
        {
            var oldTimestamp = this._estimatedTimestamp;
            var oldCount = this._estimatedCount;

            var timestamp = Environment.TickCount64;
            var diff = timestamp - oldTimestamp;

            return diff > 0 ? GetEstimatedCountSlow(oldCount, timestamp) : oldCount;
        }

        private long GetEstimatedCountSlow(long giveUpCount, long timestamp)
        {
            var taken = false;
            this._estimatedLock.TryEnter(ref taken);
            if (!taken) return giveUpCount;
            try
            {
                var newCount = this.GetCount();
                var newTimestamp = timestamp + (this._cells?.Length ?? 0);

                this._estimatedTimestamp = newTimestamp;
                this._estimatedCount = newCount;

                return newCount;
            }
            finally
            {
                this._estimatedLock.Exit();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            this.Sub(this.GetCount());
        }
    }
}
