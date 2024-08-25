using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AG
{
    /// <summary>Contains utility methods operating on bits.</summary>
    public static class BitUtils
    {
        /// <summary>Check a value for a particular bit.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(int bits, int index) => unchecked((bits & (0x1 << index)) != 0);

        /// <summary>Check a value for a particular bit.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(uint bits, int index) => unchecked((bits & (0x1u << index)) != 0);

        /// <summary>Check a value for a particular bit.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(long bits, int index) => unchecked((bits & (0x1L << index)) != 0);

        /// <summary>Check a value for a particular bit.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns><see langword="true"/> if bit is set; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(ulong bits, int index) => unchecked((bits & (0x1uL << index)) != 0);

        /// <summary>Set a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is set.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Set(int bits, int index) => unchecked(bits | (0x1 << index));

        /// <summary>Set a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is set.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint bits, int index) => unchecked(bits | (0x1u << index));

        /// <summary>Set a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is set.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Set(long bits, int index) => unchecked(bits | (0x1L << index));

        /// <summary>Set a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is set.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Set(ulong bits, int index) => unchecked(bits | (0x1uL << index));

        /// <summary>Unset a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is unset.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnSet(int bits, int index) => unchecked(bits & ~(0x1 << index));

        /// <summary>Unset a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is unset.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UnSet(uint bits, int index) => unchecked(bits & ~(0x1u << index));

        /// <summary>Unset a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is unset.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long UnSet(long bits, int index) => unchecked(bits & ~(0x1L << index));

        /// <summary>Unset a bit within a value.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="index">Bit index.</param>
        /// <returns>Value whose specified bit is unset.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong UnSet(ulong bits, int index) => unchecked(bits & ~(0x1uL << index));

        /// <summary>Find the next bit set within a value starting from a specified index.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="startIndex">Starting bit index.</param>
        /// <returns>Index whose bit is set from <paramref name="bits"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(int bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked((uint)(bits & ~((0x1 << startIndex) - 1))));

        /// <summary>Find the next bit set within a value starting from a specified index.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="startIndex">Starting bit index.</param>
        /// <returns>Index whose bit is set from <paramref name="bits"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(uint bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked(bits & ~((0x1u << startIndex) - 1)));

        /// <summary>Find the next bit set within a value starting from a specified index.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="startIndex">Starting bit index.</param>
        /// <returns>Index whose bit is set from <paramref name="bits"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(long bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked((uint)(bits & ~((0x1L << startIndex) - 1))));

        /// <summary>Find the next bit set within a value starting from a specified index.</summary>
        /// <param name="bits">Value containing the bits.</param>
        /// <param name="startIndex">Starting bit index.</param>
        /// <returns>Index whose bit is set from <paramref name="bits"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNextBitSet(ulong bits, int startIndex) => BitOperations.TrailingZeroCount(unchecked(bits & ~((0x1uL << startIndex) - 1)));
    }
}
