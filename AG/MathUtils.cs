using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

namespace AG
{
    /// <summary>Math utilities.</summary>
    public static class MathUtils
    {
        #region Triangular
        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is negative.</exception>
        /// <returns>Triangular result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Triangular(int n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (int)Triangular((uint)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence.</param>
        /// <returns>Triangular result.</returns>
        public static uint Triangular(uint n) => n * (n + 1) / 2;

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is negative.</exception>
        /// <returns>Triangular result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Triangular(long n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (long)Triangular((ulong)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence.</param>
        /// <returns>Triangular result.</returns>
        public static ulong Triangular(ulong n) => n * (n + 1) / 2;

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is negative.</exception>
        /// <returns>Triangular result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Triangular(nint n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (nint)Triangular((nuint)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence nth sequence to compute.</param>
        /// <returns>Triangular result.</returns>
        public static nuint Triangular(nuint n) => n * (n + 1) / 2;

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/1_%2B_2_%2B_3_%2B_4_%2B_%E2%8B%AF">triangular sequence</see> of <paramref name="n"/> (0, 1, 3, 6, 10, 15, 21, 28, 36, 45...).</summary>
        /// <param name="n">Triangular sequence nth sequence to compute.</param>
        /// <returns>Triangular result.</returns>
        public static BigInteger Triangular(BigInteger n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return n * (n + 1) / 2;
        }
        #endregion

        #region Fibonacci
        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Fibonacci(int n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (int)Fibonacci((uint)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        public static uint Fibonacci(uint n)
        {
            var h = int.CreateChecked(32 - BitOperations.LeadingZeroCount(n));
            var a = 0u;
            var b = 1u;

            for (var mask = 1u << h - 1; mask > 1; mask >>= 1)
            {
                var c = a * ((b << 1) - a);
                var d = a * a + b * b;
                if ((mask & n) > 0)
                {
                    a = d;
                    b = c + d;
                }
                else
                {
                    a = c;
                    b = d;
                }
            }

            // Last step done separately to remove one uint computation (as only either c or d is needed)
            return (n & 1u) is 0 ? a * ((b << 1) - a) : a * a + b * b;
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Fibonacci(long n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (long)Fibonacci((ulong)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        public static ulong Fibonacci(ulong n)
        {
            var h = int.CreateChecked(32 - BitOperations.LeadingZeroCount(n));
            var a = 0uL;
            var b = 1uL;

            for (var mask = 1uL << h - 1; mask > 1; mask >>= 1)
            {
                var c = a * ((b << 1) - a);
                var d = a * a + b * b;
                if ((mask & n) > 0)
                {
                    a = d;
                    b = c + d;
                }
                else
                {
                    a = c;
                    b = d;
                }
            }

            // Last step done separately to remove one ulong computation (as only either c or d is needed)
            return (n & 1uL) is 0 ? a * ((b << 1) - a) : a * a + b * b;
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint Fibonacci(nint n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            return (nint)Fibonacci((nuint)n);
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        public static nuint Fibonacci(nuint n)
        {
            var h = int.CreateChecked(32 - BitOperations.LeadingZeroCount(n));
            var a = (nuint)0;
            var b = (nuint)1;

            for (var mask = (nuint)1 << h - 1; mask > 1; mask >>= 1)
            {
                var c = a * ((b << 1) - a);
                var d = a * a + b * b;
                if ((mask & n) > 0)
                {
                    a = d;
                    b = c + d;
                }
                else
                {
                    a = c;
                    b = d;
                }
            }

            // Last step done separately to remove one nuint computation (as only either c or d is needed)
            return (n & (nuint)1) is 0 ? a * ((b << 1) - a) : a * a + b * b;
        }

        /// <summary>Compute the <see href="https://en.wikipedia.org/wiki/Fibonacci_sequence">fibonacci sequence</see> of <paramref name="n"/> (0, 1, 1, 2, 3, 5, 8, 13, 21, 34...).</summary>
        /// <remarks>Adopted from <see href="https://chunminchang.github.io/blog/post/calculating-fibonacci-numbers-by-fast-doubling"/> and <see href="https://chunminchang.github.io/blog/post/master-fibonacci>"/> with some modifications.</remarks>
        /// <param name="n">Fibonacci nth sequence to compute.</param>
        /// <returns>Fibonacci result.</returns>
        public static BigInteger Fibonacci(BigInteger n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);
            var h = int.CreateChecked(n.GetBitLength());
            var a = BigInteger.Zero;
            var b = BigInteger.One;

            for (var mask = BigInteger.One << h - 1; mask > 1; mask >>= 1)
            {
                var c = a * ((b << 1) - a);
                var d = a * a + b * b;
                if ((mask & n) > 0)
                {
                    a = d;
                    b = c + d;
                }
                else
                {
                    a = c;
                    b = d;
                }
            }

            // Last step done separately to remove one BigInteger computation (as only either c or d is needed)
            return n.IsEven ? a * ((b << 1) - a) : a * a + b * b;
        }
        #endregion
    }
}
