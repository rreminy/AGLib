using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Xunit;

namespace AG.Tests
{
    public sealed class PrimeUtilsTests
    {
        // Found manually using https://www.dcode.fr/primality-test
        private const int LargestPrimeInt = 2147483647;
        private const uint LargestPrimeUInt = 4294967291u;
        private const long LargestPrimeLong = 9223372036854775783L;
        private const ulong LargestPrimeULong = 18446744073709551557uL;

        // Primes from 1 to 1000
        private static readonly uint[] s_primes;
        private static readonly FrozenSet<uint> s_primesCollection;

        static PrimeUtilsTests()
        {
            var capacity = 1000000;
            var bits = new bool[capacity];
            bits[0] = bits[1] = true;

            // Generate primes for testing using Sieve of Eratosthenes
            // https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes

            var primes = new List<uint>();
            for (var number = 2u; number < capacity; number++)
            {
                ref var bit = ref bits[number];
                if (bit) continue;
                bit = true;

                primes.Add(number);
                for (var index = number * 2; index < capacity; index += number)
                {
                    bits[index] = true;
                }
            }

            var primesArray = primes.ToArray();
            var primesSet = primesArray.ToFrozenSet();

            // Assert: primesArray is sorted

            s_primes = primesArray;
            s_primesCollection = primesSet;
        }


        [Fact]
        public void IsPrimeInt()
        {
            var maxValue = (int)s_primes.Max();
            for (var value = 0; value < maxValue; value++)
            {
                var isPrime = PrimeUtils.IsPrime(value);
                if (isPrime) Assert.Contains((uint)value, (IReadOnlySet<uint>)s_primesCollection);
                else Assert.DoesNotContain((uint)value, (IReadOnlySet<uint>)s_primesCollection);
            }
        }

        [Fact]
        public void IsPrimeUInt()
        {
            var maxValue = s_primes.Max();
            for (var value = 0u; value < maxValue; value++)
            {
                var isPrime = PrimeUtils.IsPrime(value);
                if (isPrime) Assert.Contains(value, (IReadOnlySet<uint>)s_primesCollection);
                else Assert.DoesNotContain(value, (IReadOnlySet<uint>)s_primesCollection);
            }
        }

        [Fact]
        public void IsPrimeLong()
        {
            var maxValue = (long)s_primes.Max();
            for (var value = 0L; value < maxValue; value++)
            {
                var isPrime = PrimeUtils.IsPrime(value);
                if (isPrime) Assert.Contains((uint)value, (IReadOnlySet<uint>)s_primesCollection);
                else Assert.DoesNotContain((uint)value, (IReadOnlySet<uint>)s_primesCollection);
            }
        }

        [Fact]
        public void IsPrimeULong()
        {
            var maxValue = (ulong)s_primes.Max();
            for (var value = 0uL; value < maxValue; value++)
            {
                var isPrime = PrimeUtils.IsPrime(value);
                if (isPrime) Assert.Contains((uint)value, (IReadOnlySet<uint>)s_primesCollection);
                else Assert.DoesNotContain((uint)value, (IReadOnlySet<uint>)s_primesCollection);
            }
        }

        [Fact]
        public void IsPrimeBig()
        {
            var maxValue = (BigInteger)s_primes.Max();
            for (var value = BigInteger.Zero; value < maxValue; value++)
            {
                var isPrime = PrimeUtils.IsPrime(value);
                if (isPrime) Assert.Contains((uint)value, (IReadOnlySet<uint>)s_primesCollection);
                else Assert.DoesNotContain((uint)value, (IReadOnlySet<uint>)s_primesCollection);
            }
        }

        [Fact]
        public void FindNextPrimeInt()
        {
            var maxValue = (int)s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = (int)queue.Dequeue();
            for (var value = 0; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = (int)queue.Dequeue();
                Assert.Equal(nextValue, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeUInt()
        {
            var maxValue = s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = queue.Dequeue();
            for (var value = 0u; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = queue.Dequeue();
                Assert.Equal(nextValue, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeLong()
        {
            var maxValue = (long)s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = (long)queue.Dequeue();
            for (var value = 0L; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = (long)queue.Dequeue();
                Assert.Equal(nextValue, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeULong()
        {
            var maxValue = (ulong)s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = (ulong)queue.Dequeue();
            for (var value = 0uL; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = (ulong)queue.Dequeue();
                Assert.Equal(nextValue, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindNextPrimeBig()
        {
            var maxValue = (BigInteger)s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = (BigInteger)queue.Dequeue();
            for (var value = BigInteger.Zero; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = (BigInteger)queue.Dequeue();
                Assert.Equal(nextValue, PrimeUtils.FindNext(value));
            }
        }

        [Fact]
        public void FindPreviousPrimeInt()
        {
            var minValue = (int)s_primes.Min();
            var stack = new Stack<uint>(s_primes);
            var previousValue = (int)stack.Pop();
            for (var value = (int)s_primes.Max(); value >= minValue; value--)
            {
                if (value < previousValue) previousValue = (int)stack.Pop();
                Assert.Equal(previousValue, PrimeUtils.FindPrevious(value));
            }
        }

        [Fact]
        public void FindPreviousPrimeUInt()
        {
            var minValue = s_primes.Min();
            var stack = new Stack<uint>(s_primes);
            var previousValue = stack.Pop();
            for (var value = s_primes.Max(); value >= minValue; value--)
            {
                if (value < previousValue) previousValue = stack.Pop();
                Assert.Equal(previousValue, PrimeUtils.FindPrevious(value));
            }
        }

        [Fact]
        public void FindPreviousPrimeLong()
        {
            var minValue = (long)s_primes.Min();
            var stack = new Stack<uint>(s_primes);
            var previousValue = (long)stack.Pop();
            for (var value = (long)s_primes.Max(); value >= minValue; value--)
            {
                if (value < previousValue) previousValue = (long)stack.Pop();
                Assert.Equal(previousValue, PrimeUtils.FindPrevious(value));
            }
        }

        [Fact]
        public void FindPreviousPrimeULong()
        {
            var minValue = (ulong)s_primes.Min();
            var stack = new Stack<uint>(s_primes);
            var previousValue = (ulong)stack.Pop();
            for (var value = (ulong)s_primes.Max(); value >= minValue; value--)
            {
                if (value < previousValue) previousValue = (ulong)stack.Pop();
                Assert.Equal(previousValue, PrimeUtils.FindPrevious(value));
            }
        }

        [Fact]
        public void FindPreviousPrimeBig()
        {
            var minValue = (BigInteger)s_primes.Min();
            var stack = new Stack<uint>(s_primes);
            var previousValue = (BigInteger)stack.Pop();
            for (var value = new BigInteger((int)s_primes.Max()); value >= minValue; value--)
            {
                if (value < previousValue) previousValue = (BigInteger)stack.Pop();
                Assert.Equal(previousValue, PrimeUtils.FindPrevious(value));
            }
        }

        [Fact] public void LargestPrimeIsPrimeInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeInt));
        [Fact] public void LargestPrimeIsPrimeUInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeUInt));
        [Fact] public void LargestPrimeIsPrimeLong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeLong));
        [Fact] public void LargestPrimeIsPrimeULong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeULong));
        [Fact] public void LargestPrimeIsPrimeBig() => Assert.True(PrimeUtils.IsPrime(PrimeUtils.MillerRabinBases.MaxThreshold));

        //[Fact(Skip = "Largest int prime is already int.MaxValue")] public void OutOfRangeFindNextPrimesShouldThrowOverflowExceptionInt() => throw new NotImplementedException(); // int.MaxValue is prime
        [Fact] public void OutOfRangeFindNextPrimesShouldThrowOverflowExceptionUInt() => Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(uint.MaxValue));
        [Fact] public void OutOfRangeFindNextPrimesShouldThrowOverflowExceptionLong() => Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(long.MaxValue));
        [Fact] public void OutOfRangeFindNextPrimesShouldThrowOverflowExceptionULong() => Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(ulong.MaxValue));

        [Fact] public void OutOfRangeNegativePreviousPrimesShouldThrowOverflowExceptionInt() => Assert.Throws<OverflowException>(() => PrimeUtils.FindPrevious(1));
        [Fact] public void OutOfRangeNegativePreviousPrimesShouldThrowOverflowExceptionUInt() => Assert.Throws<OverflowException>(() => PrimeUtils.FindPrevious(1));
        [Fact] public void OutOfRangeNegativePreviousPrimesShouldThrowOverflowExceptionLong() => Assert.Throws<OverflowException>(() => PrimeUtils.FindPrevious(1));
        [Fact] public void OutOfRangeNegativePreviousPrimesShouldThrowOverflowExceptionULong() => Assert.Throws<OverflowException>(() => PrimeUtils.FindPrevious(1));

        [Fact] public void LargestFindNextPrimeShouldReturnItselfInt() => Assert.Equal(LargestPrimeInt, PrimeUtils.FindNext(LargestPrimeInt));
        [Fact] public void LargestFindNextPrimeShouldReturnItselfUInt() => Assert.Equal(LargestPrimeUInt, PrimeUtils.FindNext(LargestPrimeUInt));
        [Fact] public void LargestFindNextPrimeShouldReturnItselfLong() => Assert.Equal(LargestPrimeLong, PrimeUtils.FindNext(LargestPrimeLong));
        [Fact] public void LargestFindNextPrimeShouldReturnItselfULong() => Assert.Equal(LargestPrimeULong, PrimeUtils.FindNext(LargestPrimeULong));

        [Fact] public void LargestFindPreviousPrimeShouldReturnItselfInt() => Assert.Equal(LargestPrimeInt, PrimeUtils.FindPrevious(LargestPrimeInt));
        [Fact] public void LargestFindPreviousPrimeShouldReturnItselfUInt() => Assert.Equal(LargestPrimeUInt, PrimeUtils.FindPrevious(LargestPrimeUInt));
        [Fact] public void LargestFindPreviousPrimeShouldReturnItselfLong() => Assert.Equal(LargestPrimeLong, PrimeUtils.FindPrevious(LargestPrimeLong));
        [Fact] public void LargestFindPreviousPrimeShouldReturnItselfULong() => Assert.Equal(LargestPrimeULong, PrimeUtils.FindPrevious(LargestPrimeULong));

        [Fact] public void SmallestFindNextPrimeShouldReturnItselfInt() => Assert.Equal(2, PrimeUtils.FindNext(2));
        [Fact] public void SmallestFindNextPrimeShouldReturnItselfUInt() => Assert.Equal(2u, PrimeUtils.FindNext(2u));
        [Fact] public void SmallestFindNextPrimeShouldReturnItselfLong() => Assert.Equal(2L, PrimeUtils.FindNext(2L));
        [Fact] public void SmallestFindNextPrimeShouldReturnItselfULong() => Assert.Equal(2uL, PrimeUtils.FindNext(2uL));
        [Fact] public void SmallestFindNextPrimeShouldReturnItselfBig() => Assert.Equal(new BigInteger(2), PrimeUtils.FindNext(new BigInteger(2)));

        [Fact] public void SmallestFindPreviousPrimeShouldReturnItselfInt() => Assert.Equal(2, PrimeUtils.FindPrevious(2));
        [Fact] public void SmallestFindPreviousPrimeShouldReturnItselfUInt() => Assert.Equal(2u, PrimeUtils.FindPrevious(2u));
        [Fact] public void SmallestFindPreviousPrimeShouldReturnItselfLong() => Assert.Equal(2L, PrimeUtils.FindPrevious(2L));
        [Fact] public void SmallestFindPreviousPrimeShouldReturnItselfULong() => Assert.Equal(2uL, PrimeUtils.FindPrevious(2uL));
        [Fact] public void SmallestFindPreviousPrimeShouldReturnItselfBig() => Assert.Equal(new BigInteger(2), PrimeUtils.FindPrevious(new BigInteger(2)));
    }
}