using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace AG
{
    /// <content>Miller Rabin algorithm.</content>
    public static partial class PrimeUtils
    {
        // Taken from: https://github.com/Open-NET-Libraries/Open.Numeric.Primes/blob/master/source/MillerRabin.cs
        // With some changes to extend prime processing range.

        internal static class Big
        {
            public static readonly BigInteger Two = 2;
            public static readonly BigInteger Three = 3;
            public static readonly BigInteger Six = 6;
        }

        internal static class MillerRabinBases
        {
            private static readonly ReadOnlyMemory<uint>[] s_arrays =
            [
                new uint[] { 2, 7, 61 }, // Up to 4,759,123,141 uint
                new uint[] { 2, 3, 5, 7, 11, 13, 17 }, // Up to 341,550,071,728,321 ulong
                new uint[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 }, // Up to 3,825,123,056,546,413,051 ulong
                new uint[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }, // Up to 318,665,857,834,031,151,167,461 BigInteger
                new uint[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }, // Up to 3,317,044,064,679,887,385,961,981 BigInteger
            ];

            private static readonly BigInteger[] s_bigThresholds =
            [
                4759123141u,
                341550071728321uL,
                3825123056546413051uL,
                BigInteger.Parse("318665857834031151167461", NumberStyles.None, CultureInfo.InvariantCulture),
                BigInteger.Parse("3317044064679887385961981", NumberStyles.None, CultureInfo.InvariantCulture),
            ];

            public static BigInteger MaxThreshold { get; } = s_bigThresholds.Max();

            public static ReadOnlySpan<uint> GetBases(ulong number)
            {
                if (number <= 4759123141) return s_arrays[0].Span;
                if (number <= 341550071728321) return s_arrays[1].Span;
                if (number <= 3825123056546413051) return s_arrays[2].Span;
                return s_arrays[3].Span;
            }

            public static ReadOnlySpan<uint> GetBases(BigInteger number, bool probabilistic = false)
            {
                if (number <= 4759123141) return s_arrays[0].Span;
                if (number <= 341550071728321) return s_arrays[1].Span;
                if (number <= 3825123056546413051) return s_arrays[2].Span;
                if (number <= s_bigThresholds[3]) return s_arrays[3].Span;
                if (number <= s_bigThresholds[4]) return s_arrays[4].Span;
                return probabilistic ? s_arrays[^0].Span : throw new ArgumentOutOfRangeException($"{nameof(number)} is too big, maximum value is {MaxThreshold}");
            }
        }

        /* Based on: https://stackoverflow.com/questions/4236673/sample-code-for-fast-primality-testing-in-c-sharp#4236870 */
        /// <summary>
        /// Miller-Rabin prime utility.
        /// </summary>
        internal static class MillerRabin
        {
            internal static bool IsProbablePrimeInternal(ulong value) => IsPrimeInternal(value, MillerRabinBases.GetBases(1));

            internal static bool IsProbablePrimeInternal(BigInteger value) => IsPrimeInternal(value, MillerRabinBases.GetBases(1));

            internal static bool IsPrimeInternal(ulong value) => IsPrimeInternal(value, MillerRabinBases.GetBases(value));

            internal static bool IsPrimeInternal(BigInteger value) => IsPrimeInternal(value, MillerRabinBases.GetBases(value));

            internal static bool IsPrimeInternal(ulong value, ReadOnlySpan<uint> ar)
            {
                var d = value - 1;
                var s = 0;

                while ((d & 1) == 0)
                {
                    d >>= 1;
                    s++;
                }

                foreach (ref readonly var b in ar)
                {
                    var a = value - 2;
                    var now = a > b
                        ? (ulong)BigInteger.ModPow(b, d, value)
                        : (ulong)BigInteger.ModPow(a, d, value);

                    if (now == 1 || now == value - 1) continue;

                    var j = 1;
                    for (; j < s; j++)
                    {
                        now = (ulong)BigInteger.ModPow(now, 2, value);
                        if (now == value - 1) break;
                    }
                    if (j == s) return false;
                }

                return true;
            }

            internal static bool IsPrimeInternal(BigInteger value, ReadOnlySpan<uint> ar)
            {
                var d = value - 1;
                var s = 0;

                while ((d & 1) == 0)
                {
                    d >>= 1;
                    s++;
                }

                foreach (ref readonly var b in ar)
                {
                    var a = value - 2;
                    var now = a > b
                        ? BigInteger.ModPow(b, d, value)
                        : BigInteger.ModPow(a, d, value);

                    if (now == 1 || now == value - 1) continue;

                    var j = 1;
                    for (; j < s; j++)
                    {
                        now = BigInteger.ModPow(now, 2, value);
                        if (now == value - 1) break;
                    }
                    if (j == s) return false;
                }

                return true;
            }
        }
    }
}
