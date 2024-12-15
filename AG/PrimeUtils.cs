using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AG
{
    /// <summary>Contains utility methods for prime numbers.</summary>
    public static partial class PrimeUtils
    {
        private const int TinyPrimesBytes = 1024;
        private const int TinyPrimesMax = TinyPrimesBytes * 16;

        private static readonly byte[] s_tinyPrimes = GenerateTinyPrimes();
        private static readonly uint PerformanceThreshold = FindPerformanceThreshold();

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(int number)
        {
            return number >= 0 && IsPrime((uint)number);
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(uint number)
        {
            if (number < TinyPrimesMax) return IsPrimeCoreTiny((int)number);
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var i = number / 30 * 30;
            if (number != (i + 1) && number != (i + 7) && number != (i + 11) && number != (i + 13) && number != (i + 17) && number != (i + 19) && number != (i + 23) && number != (i + 29)) return false;

            var limit = (uint)Math.Sqrt(number);
            if (limit * limit == number) return false;
            if (number < PerformanceThreshold) return IsPrimeCore(number, limit);
            return MillerRabin.IsPrimeInternal(number);
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(long number)
        {
            return number >= 0 && IsPrime((ulong)number);
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(ulong number)
        {
            if (number < TinyPrimesMax) return IsPrimeCoreTiny((int)number);
            if (number <= uint.MaxValue) return IsPrime((uint)number);
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var i = number / 30 * 30;
            if (number != (i + 1) && number != (i + 7) && number != (i + 11) && number != (i + 13) && number != (i + 17) && number != (i + 19) && number != (i + 23) && number != (i + 29)) return false;

            var limit = (ulong)Math.Sqrt(number);
            if (limit * limit == number) return false;
            if (number < PerformanceThreshold) return IsPrimeCore(number, limit);
            return MillerRabin.IsPrimeInternal(number);
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(BigInteger number)
        {
            if (number < TinyPrimesMax) return IsPrimeCoreTiny((int)number);
            if (number <= uint.MaxValue) return IsPrime((uint)number);
            if (number <= ulong.MaxValue) return IsPrime((ulong)number);
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var i = number / 30 * 30;
            if (number != (i + 1) && number != (i + 7) && number != (i + 11) && number != (i + 13) && number != (i + 17) && number != (i + 19) && number != (i + 23) && number != (i + 29)) return false;

            var limit = PrimeInternals.Sqrt(number);
            if (limit * limit == number) return false;
            if (number < PerformanceThreshold) return IsPrimeCore(number, limit);
            return MillerRabin.IsPrimeInternal(number);
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindNext(int number)
        {
            if (number is <= 2) return 2;
            return (int)FindNextCore((uint)number, int.MaxValue);
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FindNext(uint number)
        {
            if (number is <= 2) return 2;
            return FindNextCore(number, uint.MaxValue);
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FindNext(long number)
        {
            if (number is <= 2) return 2;
            return (long)FindNextCore((ulong)number, long.MaxValue);
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FindNext(ulong number)
        {
            if (number is <= 2) return 2;
            return FindNextCore(number, ulong.MaxValue);
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger FindNext(BigInteger number)
        {
            if (number <= 2) return 2;
            return FindNextCore(number);
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindPrevious(int number)
        {
            if (number is < 2) ThrowHelper.Throw<OverflowException>();
            return (int)FindPreviousCore((uint)number);
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FindPrevious(uint number)
        {
            if (number is < 2) ThrowHelper.Throw<OverflowException>();
            return FindPreviousCore(number);
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FindPrevious(long number)
        {
            if (number is < 2) ThrowHelper.Throw<OverflowException>();
            return (long)FindPreviousCore((ulong)number);
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FindPrevious(ulong number)
        {
            if (number is < 2) ThrowHelper.Throw<OverflowException>();
            return FindPreviousCore(number);
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger FindPrevious(BigInteger number)
        {
            if (number < 2) ThrowHelper.Throw<OverflowException>();
            return FindPreviousCore(number);
        }

        private static byte[] GenerateTinyPrimes()
        {
            var primesArray = GC.AllocateArray<byte>(TinyPrimesBytes, true);
            var primes = primesArray.AsSpan();
            var candidates = new BitArray(TinyPrimesMax);
            candidates[0] = true;
            for (var index = 0; index < TinyPrimesBytes; index++)
            {
                var primesByte = primes[index];
                for (var bit = 1; bit < 16; bit += 2)
                {
                    Debug.Assert((bit & 1) == 1, "First bit is expected to be 1");
                    var number = index * 16 + bit;
                    if (candidates[number >> 1]) continue;

                    primesByte |= (byte)(1 << (bit >> 1));
                    for (var multiple = number; multiple < TinyPrimesMax; multiple += number)
                    {
                        if ((multiple & 1) == 0) continue;
                        candidates[multiple >> 1] = true;
                    }
                }
                primes[index] = primesByte;
            }
            return primesArray;
        }

        /// <summary>Determine if a number is prime, large scale scan.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static bool IsPrimeCoreTiny(int number)
        {
            Debug.Assert(number >= 0, $"{nameof(number)} must be positive");
            Debug.Assert(number < TinyPrimesMax, $"{nameof(number)} must be less than {nameof(TinyPrimesMax)}");
            if (number == 2) return true;
            if ((number & 1) == 0) return false;
            (var index, var bit) = Math.DivRem(number, 16);
            return (s_tinyPrimes[index] & (byte)(1 << (bit >> 1))) != 0;
        }

        /// <summary>Determine if a number is prime, large scale scan.</summary>
        /// <param name="number">Number to check.</param>
        /// <param name="limit">Search limit.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        private static bool IsPrimeCore(uint number, uint limit)
        {
            Debug.Assert(number >= TinyPrimesMax, $"{nameof(number)} must be equal or greater than {TinyPrimesMax}");
            for (var i = 30u; i <= limit; i += 30)
            {
                if (number % (i + 1) == 0 || number % (i + 7) == 0 || number % (i + 11) == 0 || number % (i + 13) == 0 || number % (i + 17) == 0 || number % (i + 19) == 0 || number % (i + 23) == 0 || number % (i + 29) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime, large scale scan.</summary>
        /// <param name="number">Number to check.</param>
        /// <param name="limit">Search limit.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        private static bool IsPrimeCore(ulong number, ulong limit)
        {
            Debug.Assert(number >= TinyPrimesMax, $"{nameof(number)} must be equal or greater than {uint.MaxValue}");
            for (var i = 30uL; i <= limit; i += 30)
            {
                if (number % (i + 1) == 0 || number % (i + 7) == 0 || number % (i + 11) == 0 || number % (i + 13) == 0 || number % (i + 17) == 0 || number % (i + 19) == 0 || number % (i + 23) == 0 || number % (i + 29) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime, large scale scan.</summary>
        /// <param name="number">Number to check.</param>
        /// <param name="limit">Search limit.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        private static bool IsPrimeCore(BigInteger number, BigInteger limit)
        {
            Debug.Assert(number >= TinyPrimesMax, $"{nameof(number)} must be equal or greater than {ulong.MaxValue}");
            for (var i = 30uL; i <= limit; i += 30)
            {
                if (number % (i + 1) == 0 || number % (i + 7) == 0 || number % (i + 11) == 0 || number % (i + 13) == 0 || number % (i + 17) == 0 || number % (i + 19) == 0 || number % (i + 23) == 0 || number % (i + 29) == 0) return false;
            }
            return true;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <param name="limit">Search limit.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static uint FindNextCore(uint number, uint limit)
        {
            if (number <= 2) return 2;
            if (number is 3) return 3;
            number |= 1; // Make sure its odd
            for (; ; number += 2)
            {
                if (IsPrime(number)) return number;
                if (number == limit) ThrowHelper.Throw(new OverflowException());
            }
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <param name="limit">Search limit.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static ulong FindNextCore(ulong number, ulong limit)
        {
            if (number <= 2) return 2;
            if (number is 3) return 3;
            number |= 1; // Make sure its odd
            for (; ; number += 2)
            {
                if (IsPrime(number)) return number;
                if (number == limit) ThrowHelper.Throw(new OverflowException());
            }
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static BigInteger FindNextCore(BigInteger number)
        {
            if (number <= 2) return 2;
            if (number == 3) return 3;
            number |= 1; // Make sure its odd
            for (; ; number += 2)
            {
                if (IsPrime(number)) return number;
            }
        }


        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static uint FindPreviousCore(uint number)
        {
            if (number is 2) return 2;
            if (number % 2 == 0) --number;
            if (number is 3 or 5) return number;
            for (; ; number -= 2)
            {
                if (IsPrime(number)) return number;
                if (number == 1) ThrowHelper.Throw(new OverflowException());
            }
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static ulong FindPreviousCore(ulong number)
        {
            if (number is 2) return 2;
            if (number % 2 == 0) --number;
            if (number is 3 or 5) return number;
            for (; ; number -= 2)
            {
                if (IsPrime(number)) return number;
                if (number == 1) ThrowHelper.Throw(new OverflowException());
            }
        }

        /// <summary>Finds the previous prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Previous numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        [SuppressMessage("Critical Code Smell", "S1994", Justification = "Intended.")]
        private static BigInteger FindPreviousCore(BigInteger number)
        {
            if (number == 2) return 2;
            if (number % 2 == 0) --number;
            if (number == 3 || number == 5) return number;
            for (; ; number -= 2)
            {
                if (IsPrime(number)) return number;
                if (number == 1) ThrowHelper.Throw(new OverflowException());
            }
        }

        private static uint FindPerformanceThreshold()
        {
            for (var threshold = TinyPrimesMax * 2; threshold < 1024 * 1024 * 1024; threshold += 1024)
            {
                var number = (uint)FindNext(threshold);

                for (var attempt = 1; attempt <= 5; attempt++)
                {
                    var poly = Measure(number => IsPrimeCore(number, (uint)Math.Sqrt(number)), number);
                    var miller = Measure(number => MillerRabin.IsPrimeInternal(number), number);
                    if (miller <= poly)
                    {
                        if (attempt == 3) return (uint)threshold;
                    }
                    else break;
                }
            }
            return 1024 * 1024 * 1024;
        }

        private static long Measure(Func<uint, bool> func, uint number)
        {
            const int trials = 15;

            var results = new long[trials];
            for (var index = 0; index < trials; index++)
            {
                var sw = Stopwatch.StartNew();
                _ = func(number);
                sw.Stop();
                results[index] = sw.Elapsed.Ticks;
            }
            Array.Sort(results);
            return (long)results.Skip(6).Take(3).Average();
        }
    }
}
