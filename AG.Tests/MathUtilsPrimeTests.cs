using System.Collections.Frozen;

namespace AG.Tests
{
    public sealed class MathUtilsPrimeTests
    {
        // Found manually using https://www.dcode.fr/primality-test
        private const int LargestPrimeInt = 2147483647;
        private const uint LargestPrimeUInt = 4294967291u;
        private const long LargestPrimeLong = 9223372036854775783L;
        private const ulong LargestPrimeULong = 18446744073709551557uL;

        // Primes from 1 to 1000
        private static readonly uint[] s_primes = [
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
            101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
            211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
            307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
            401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
            503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
            601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
            701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
            809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
            907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997
        ];
        private static readonly IReadOnlyCollection<uint> s_primesCollection = s_primes.ToFrozenSet();


        [Fact]
        public void IsPrimeInt()
        {
            for (var value = 0; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(value) == s_primesCollection.Contains((uint)value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains((uint)value)}");
            }
        }

        [Fact]
        public void IsPrimeNegativeInt()
        {
            for (var value = 0; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(-value) == s_primesCollection.Contains((uint)value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains((uint)value)}");
            }
        }

        [Fact]
        public void IsPrimeUInt()
        {
            for (var value = 0u; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(value) == s_primesCollection.Contains(value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains(value)}");
            }
        }

        [Fact]
        public void IsPrimeLong()
        {
            for (var value = 0L; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(value) == s_primesCollection.Contains((uint)value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains((uint)value)}");
            }
        }

        [Fact]
        public void IsPrimeNegativeLong()
        {
            for (var value = 0L; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(-value) == s_primesCollection.Contains((uint)value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains((uint)value)}");
            }
        }

        [Fact]
        public void IsPrimeULong()
        {
            for (var value = 0uL; value < 1000; value++)
            {
                Assert.True(PrimeUtils.IsPrime(value) == s_primesCollection.Contains((uint)value), $"{value}: {PrimeUtils.IsPrime(value)} != {s_primesCollection.Contains((uint)value)}");
            }
        }

        [Fact]
        public void FindNextPrimeInt()
        {
            var maxValue = (int)s_primes.Max();
            for (var value = 0; value <= maxValue; value++)
            {
                var expected = (int)s_primes.Where(prime => prime >= value).Min();
                Assert.Equal(expected, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeUInt()
        {
            var maxValue = (uint)s_primes.Max();
            for (var value = 0u; value <= maxValue; value++)
            {
                var expected = (uint)s_primes.Where(prime => prime >= value).Min();
                Assert.Equal(expected, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeLong()
        {
            var maxValue = (long)s_primes.Max();
            for (var value = 0L; value <= maxValue; value++)
            {
                var expected = (long)s_primes.Where(prime => prime >= value).Min();
                Assert.Equal(expected, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeULong()
        {
            var maxValue = (ulong)s_primes.Max();
            for (var value = 0uL; value <= maxValue; value++)
            {
                var expected = (ulong)s_primes.Where(prime => prime >= value).Min();
                Assert.Equal(expected, PrimeUtils.FindNext(value));
            }
        }

        [Fact] public void LargestPrimeIsPrimeInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeInt));
        [Fact] public void LargestPrimeIsPrimeUInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeUInt));
        [Fact] public void LargestPrimeIsPrimeLong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeLong));
        [Fact] public void LargestPrimeIsPrimeULong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeULong + 1));

        [Fact]
        public void OutOfRangePrimesShouldThrowOverflowException()
        {
            //Assert.Throws<OverflowException>(() => MathUtils.FindNextPrime(LargestPrimeInt + 1)); // LargestPrimeInt is already the largest (int.MaxValue)
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeUInt + 1));
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeLong + 1));
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeULong + 1));
        }
    }
}