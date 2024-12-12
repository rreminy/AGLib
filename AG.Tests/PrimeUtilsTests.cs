using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var capacity = 100000;
            var bits = new bool[capacity];
            bits[0] = bits[1] = true;

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
                var expected = s_primesCollection.Contains((uint)value);
                if (expected) Assert.True(PrimeUtils.IsPrime(value));
                else Assert.False(PrimeUtils.IsPrime(value));
            }
        }

        [Fact]
        public void IsPrimeNegativeInt()
        {
            var maxValue = (int)s_primes.Max();
            for (var value = 0; value < maxValue; value++)
            {
                var expected = s_primesCollection.Contains((uint)value);
                if (expected) Assert.True(PrimeUtils.IsPrime(-value));
                else Assert.False(PrimeUtils.IsPrime(value));
            }
        }

        [Fact]
        public void IsPrimeUInt()
        {
            var maxValue = s_primes.Max();
            for (var value = 0u; value < maxValue; value++)
            {
                var expected = s_primesCollection.Contains(value);
                if (expected) Assert.True(PrimeUtils.IsPrime(value));
                else Assert.False(PrimeUtils.IsPrime(value));
            }
        }

        [Fact]
        public void IsPrimeLong()
        {
            var maxValue = (long)s_primes.Max();
            for (var value = 0L; value < maxValue; value++)
            {
                var expected = s_primesCollection.Contains((uint)value);
                if (expected) Assert.True(PrimeUtils.IsPrime(value));
                else Assert.False(PrimeUtils.IsPrime(value));
            }
        }

        [Fact]
        public void IsPrimeNegativeLong()
        {
            var maxValue = (long)s_primes.Max();
            for (var value = 0L; value < maxValue; value++)
            {
                var expected = s_primesCollection.Contains((uint)value);
                if (expected) Assert.True(PrimeUtils.IsPrime(-value));
                else Assert.False(PrimeUtils.IsPrime(value));
            }
        }

        [Fact]
        public void IsPrimeULong()
        {
            var maxValue = (ulong)s_primes.Max();
            for (var value = 0uL; value < maxValue; value++)
            {
                var expected = s_primesCollection.Contains((uint)value);
                if (expected) Assert.True(PrimeUtils.IsPrime(value));
                else Assert.False(PrimeUtils.IsPrime(value));
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
            var maxValue = (uint)s_primes.Max();
            var queue = new Queue<uint>(s_primes);
            var nextValue = (uint)queue.Dequeue();
            for (var value = 0u; value <= maxValue; value++)
            {
                if (value > nextValue) nextValue = (uint)queue.Dequeue();
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

        [Fact] public void LargestPrimeIsPrimeInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeInt));
        [Fact] public void LargestPrimeIsPrimeUInt() => Assert.True(PrimeUtils.IsPrime(LargestPrimeUInt));
        [Fact] public void LargestPrimeIsPrimeLong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeLong));
        [Fact] public void LargestPrimeIsPrimeULong() => Assert.True(PrimeUtils.IsPrime(LargestPrimeULong));

        [Fact]
        public void OutOfRangePrimesShouldThrowOverflowException()
        {
            //Assert.Throws<OverflowException>(() => MathUtils.FindNextPrime(LargestPrimeInt + 1)); // LargestPrimeInt is already the largest int (int.MaxValue)
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeUInt + 1));
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeLong + 1));
            Assert.Throws<OverflowException>(() => PrimeUtils.FindNext(LargestPrimeULong + 1));
        }
    }
}