using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace AG
{
    /// <summary>Contains utility methods for prime numbers.</summary>
    public static class PrimeUtils
    {
        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(int number)
        {
            if (number < 0) number = -number;
            if (number is 1) return false;
            if (number is 2 or 3 or 5 or 7 or 11 or 13 or 17 or 19 or 23 or 29) return true;
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var limit = (int)Math.Sqrt(number);
            for (var i = 31; i <= limit; i += 30)
            {
                if (number % i == 0 || number % (i + 6) == 0 || number % (i + 10) == 0 || number % (i + 12) == 0 || number % (i + 16) == 0 || number % (i + 18) == 0 || number % (i + 22) == 0 || number % (i + 28) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(uint number)
        {
            if (number is 1) return false;
            if (number is 2 or 3 or 5 or 7 or 11 or 13 or 17 or 19 or 23 or 29) return true;
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var limit = (uint)Math.Sqrt(number);
            for (var i = 30u; i <= limit; i += 30)
            {
                if (number % (i + 1) == 0 || number % (i + 7) == 0 || number % (i + 11) == 0 || number % (i + 13) == 0 || number % (i + 17) == 0 || number % (i + 19) == 0 || number % (i + 23) == 0 || number % (i + 29) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(long number)
        {
            if (number < 0) number = -number;
            if (number is 1) return false;
            if (number is 2 or 3 or 5 or 7 or 11 or 13 or 17 or 19 or 23 or 29) return true;
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var limit = (long)Math.Sqrt(number);
            for (var i = 31L; i <= limit; i += 30)
            {
                if (number % i == 0 || number % (i + 6) == 0 || number % (i + 10) == 0 || number % (i + 12) == 0 || number % (i + 16) == 0 || number % (i + 18) == 0 || number % (i + 22) == 0 || number % (i + 28) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        [Pure]
        public static bool IsPrime(ulong number)
        {
            if (number is 1) return false;
            if (number is 2 or 3 or 5 or 7 or 11 or 13 or 17 or 19 or 23 or 29) return true;
            if ((number & 1) == 0 || number % 3 == 0 || number % 5 == 0 || number % 7 == 0 || number % 11 == 0 || number % 13 == 0 || number % 17 == 0 || number % 19 == 0 || number % 23 == 0 || number % 29 == 0) return false;

            var limit = (ulong)Math.Sqrt(number);
            for (var i = 31UL; i <= limit; i += 30)
            {
                if (number % i == 0 || number % (i + 6) == 0 || number % (i + 10) == 0 || number % (i + 12) == 0 || number % (i + 16) == 0 || number % (i + 18) == 0 || number % (i + 22) == 0 || number % (i + 28) == 0) return false;
            }
            return true;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        public static int FindNext(int number)
        {
            if (number < 0)
            {
                if (number > int.MinValue) return -FindNext(-number);
                else ThrowHelper.Throw(new OverflowException());
            }
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < int.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            ThrowHelper.Throw(new OverflowException());
            return 0;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        public static uint FindNext(uint number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < uint.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            ThrowHelper.Throw(new OverflowException());
            return 0;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        public static long FindNext(long number)
        {
            if (number < 0)
            {
                if (number > long.MinValue) return -FindNext(-number);
                else ThrowHelper.Throw(new OverflowException());
            }
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < long.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            ThrowHelper.Throw(new OverflowException());
            return 0;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
        [Pure]
        public static ulong FindNext(ulong number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < ulong.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            ThrowHelper.Throw(new OverflowException());
            return 0;
        }
    }
}
