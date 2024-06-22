using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Concurrent
{
    internal static class BitUtils
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(int bits, int index) => unchecked((bits & (0x1 << index)) != 0);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(uint bits, int index) => unchecked((bits & (0x1 << index)) != 0);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Set(int bits, int index) => unchecked(bits | (0x1 << index));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint bits, int index) => unchecked(bits | (0x1u << index));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnSet(int bits, int index) => unchecked(bits & ~(0x1 << index));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UnSet(uint bits, int index) => unchecked(bits & ~(0x1u << index));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(int bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked((uint)(bits & ~((0x1 << startIndex) - 1))));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(uint bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked(bits & ~((0x1u << startIndex) - 1)));
    }
}
