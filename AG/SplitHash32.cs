using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AG
{
    /// <summary>32-bit Hash generators based on SplitMix32.</summary>
    /// <remarks>Adapted from: <c>https://github.com/umireon/my-random-stuff/blob/master/xorshift/splitmix32.c</c>.</remarks>
    public static class SplitHash32
    {
        #region Primitive Types
        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(uint x) // This is the main method
        {
            x += 0x9e3779b9u;
            x = (x ^ (x >> 16)) * 0x85ebca6bu;
            x = (x ^ (x >> 13)) * 0xc2b2ae35u;
            return (int)(x ^ (x >> 16));
        }

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(int x) => Compute((uint)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(ulong x) => Compute((uint)x) ^ Compute((uint)(x >> 32));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(long x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(ushort x) => Compute((uint)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(short x) => Compute((uint)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(byte x) => Compute((uint)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(sbyte x) => Compute((uint)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(float x) => Compute(Unsafe.BitCast<float, uint>(x));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(double x) => Compute(Unsafe.BitCast<double, ulong>(x));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(Half x) => Compute(Unsafe.BitCast<Half, ushort>(x));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(UInt128 x) => Compute((ulong)x) ^ Compute((ulong)(x >> 64));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static int Compute(Int128 x) => Compute((UInt128)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute(nuint x) => Compute(MemoryMarshal.CreateReadOnlySpan(ref x, 1));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(nint x) => Compute(MemoryMarshal.CreateReadOnlySpan(ref x, 1));
        #endregion

        #region Arrays, strings and bytes
        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [Pure]
        public static unsafe int Compute(BigInteger x)
        {
            var bytes = x.ToByteArray();
            fixed (byte* ptr = bytes)
            {
                return Compute(ptr, (ulong)bytes.Length);
            }
        }

        /// <summary>Compute the hash of a <see cref="byte"/> array.</summary>
        /// <param name="start">Starting pointer to compute hash from.</param>
        /// <param name="length">Array length.</param>
        /// <returns>Hash result.</returns>
        [Pure]
        public static unsafe int Compute(byte* start, ulong length)
        {
            var hash = Compute(length);
            while (length >= 4)
            {
                hash ^= Compute(*(uint*)start);
                start += 4; length -= 4;
            }
            if (length >= 2)
            {
                hash ^= Compute(*(ushort*)start);
                start += 2; length -= 2;
            }
            if (length is 1)
            {
                hash ^= Compute(*start);
            }
            return hash;
        }

        /// <summary>Compute the hash of a <see cref="byte"/> array.</summary>
        /// <param name="start">Starting pointer to compute hash from.</param>
        /// <param name="end">Ending pointer to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe int Compute(byte* start, byte* end)
        {
            var length = (ulong)(end - start);
            return Compute(start, length);
        }

        /// <summary>Compute the hash of a <typeparamref name="T"/> array.</summary>
        /// <typeparam name="T">Pointer's underlying data type.</typeparam>
        /// <param name="start">Starting pointer to compute hash from.</param>
        /// <param name="end">Ending pointer to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe int Compute<T>(T* start, T* end) where T : unmanaged
        {
            var count = (ulong)(end - start);
            return Compute(start, count);
        }

        /// <summary>Compute the hash of <paramref name="span"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <typeparam name="T">Span's underlying data type.</typeparam>
        /// <param name="span"><see cref="ReadOnlySpan{T}"/> to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [Pure]
        public static unsafe int Compute<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            fixed (T* ptr = span)
            {
                return Compute(ptr, (ulong)span.Length);
            }
        }

        /// <summary>Compute the hash of <paramref name="memory"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <typeparam name="T">Memory's underlying data type.</typeparam>
        /// <param name="memory"><see cref="ReadOnlyMemory{T}"/> to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe int Compute<T>(ReadOnlyMemory<T> memory) where T : unmanaged
        {
            return Compute(memory.Span);
        }

        /// <summary>Compute the hash of an array using a <paramref name="start"/> pointer and <paramref name="count"/>.</summary>
        /// <typeparam name="T">Pointer's underlying data type.</typeparam>
        /// <param name="start">Starting pointer.</param>
        /// <param name="count">Elements from pointer.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe int Compute<T>(T* start, ulong count) where T : unmanaged
        {
            return Compute((byte*)start, (ulong)sizeof(T) * count);
        }

        /// <summary>Compute the hash of <paramref name="str"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="str"><see cref="string"/> to compute hash from.</param>
        /// <returns>Hash of <paramref name="str"/>.</returns>
        [Pure]
        public static unsafe int Compute(string str)
        {
            fixed (char* ptr = str)
            {
                return Compute(ptr, (ulong)str.Length);
            }
        }
        #endregion
    }
}
