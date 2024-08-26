using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AG
{
    /// <summary>64-bit Hash generators based on SplitMix64.</summary>
    /// <remarks>Adapted from: <c>https://xoshiro.di.unimi.it/splitmix64.c</c>.</remarks>
    public static class SplitHash64
    {
        #region Primitive Types
        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(ulong x) // This is the main method
        {
            x += 0x9e3779b97f4a7c15U;
            x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9U;
            x = (x ^ (x >> 27)) * 0x94d049bb133111ebU;
            return (long)(x ^ (x >> 31));
        }

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(long x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(uint x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(int x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(ushort x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(short x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(byte x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(sbyte x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(nuint x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(nint x) => Compute((ulong)x);

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute(double x) => Compute(*(ulong*)&x); // Of course, some ugly trickery had to happen here

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute(float x) => Compute(*(uint*)&x); // Don't ask Lousy Gem... All this stuff is pretty safe, okay?

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute(Half x) => Compute(*(ushort*)&x); // Its 100% safe!

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(UInt128 x) => Compute((ulong)x) ^ Compute((ulong)(x >> 64));

        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static long Compute(Int128 x) => Compute((UInt128)x);
        #endregion

        #region Arrays, strings and bytes
        /// <summary>Compute the hash of <paramref name="x"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="x">Value to compute hash from.</param>
        /// <returns>Hash of <paramref name="x"/>.</returns>
        [Pure]
        public static unsafe long Compute(BigInteger x)
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
        public static unsafe long Compute(byte* start, ulong length)
        {
            var hash = Compute(length);
            while (length >= 8)
            {
                hash ^= Compute(*(ulong*)start);
                start += 8; length -= 8;
            }
            if (length >= 4)
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
        public static unsafe long Compute(byte* start, byte* end)
        {
            var length = (ulong)(end - start);
            return Compute(start, length);
        }

        /// <summary>Compute the hash of a <see cref="T"/> array.</summary>
        /// <param name="start">Starting pointer to compute hash from.</param>
        /// <param name="end">Ending pointer to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute<T>(T* start, T* end) where T : unmanaged
        {
            var count = (ulong)(end - start);
            return Compute(start, count);
        }

        /// <summary>Compute the hash of <paramref name="span"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="span"><see cref="ReadOnlySpan{T}"/> to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [Pure]
        public static unsafe long Compute<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            fixed (T* ptr = span)
            {
                return Compute(ptr, (ulong)span.Length);
            }
        }

        /// <summary>Compute the hash of <paramref name="memory"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="memory"><see cref="ReadOnlyMemory{T}"/> to compute hash from.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute<T>(ReadOnlyMemory<T> memory) where T : unmanaged
        {
            return Compute(memory.Span);
        }

        /// <summary>Compute the hash of an array using a <paramref name="start"/> pointer and <paramref name="count"/>.</summary>
        /// <typeparam name="T">Pointer type.</typeparam>
        /// <param name="start">Starting pointer.</param>
        /// <param name="count">Elements from pointer.</param>
        /// <returns>Hash result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe long Compute<T>(T* start, ulong count) where T : unmanaged
        {
            return Compute((byte*)start, (ulong)sizeof(T) * count);
        }

        /// <summary>Compute the hash of <paramref name="str"/>.</summary>
        /// <remarks>This method incurs pinning overhead.</remarks>
        /// <param name="str"><see cref="string"/> to compute hash from.</param>
        /// <returns>Hash of <paramref name="str"/>.</returns>
        [Pure]
        public static unsafe long Compute(string str)
        {
            fixed (char* ptr = str)
            {
                return Compute(ptr, (ulong)str.Length);
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>Mixes <paramref name="hash"/>'s low and high bits.</summary>
        /// <remarks>This simply xor the low and high bits of <paramref name="hash"/>.</remarks>
        /// <param name="hash">Hash to convert.</param>
        /// <returns>Result hash.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static unsafe int ConvertTo32Bit(long hash)
        {
            return (int)((uint)hash ^ (uint)((ulong)hash >> 32));
        }
        #endregion
    }
}
