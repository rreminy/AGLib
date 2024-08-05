using System;

namespace AG
{
    /// <summary>Contains utility methods for prime numbers.</summary>
    public static class PrimeUtils
    {
        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        public static bool IsPrime(int number)
        {
            if (number < 0) number = -number;
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (int)Math.Sqrt(number);
            for (var i = 5; i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        public static bool IsPrime(uint number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (uint)Math.Sqrt(number);
            for (var i = 5; i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        public static bool IsPrime(long number)
        {
            if (number < 0) number = -number;
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (long)Math.Sqrt(number);
            for (var i = 5L; i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        /// <summary>Determine if a number is prime.</summary>
        /// <param name="number">Number to check.</param>
        /// <returns><see langword="true"/> if <paramref name="number"/> is prime; <see langword="false"/> otherwise.</returns>
        public static bool IsPrime(ulong number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (ulong)Math.Sqrt(number);
            for (var i = 5UL; i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        /// <summary>Finds the next prime number starting from a specified <paramref name="number"/>.</summary>
        /// <param name="number">Number to start looking from.</param>
        /// <returns>Next numerical prime.</returns>
        /// <exception cref="OverflowException">Searching overflow.</exception>
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
