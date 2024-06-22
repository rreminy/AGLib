using System;

namespace AG.Collections.Concurrent
{
    internal static class MathUtils
    {
        public static bool IsPrime(int number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (int)Math.Sqrt(number);
            for (var i = 5; i * i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        public static bool IsPrime(uint number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (uint)Math.Sqrt(number);
            for (var i = 5; i * i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        public static bool IsPrime(long number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (long)Math.Sqrt(number);
            for (var i = 5L; i * i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        public static bool IsPrime(ulong number)
        {
            if (number is 2 or 3) return true;
            if (number <= 1 || number % 2 == 0 || number % 3 == 0) return false;

            var limit = (ulong)Math.Sqrt(number);
            for (var i = 5UL; i * i <= limit; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        public static int FindNextPrime(int number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < int.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            throw new OverflowException();
        }

        public static uint FindNextPrime(uint number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < uint.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            throw new OverflowException();
        }

        public static long FindNextPrime(long number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < long.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            throw new OverflowException();
        }

        public static ulong FindNextPrime(ulong number)
        {
            if (number is 2 or 3) return number;
            if (number <= 1) return 2;
            number |= 1; // Make sure its odd
            for (; number < ulong.MaxValue; number += 2)
            {
                if (IsPrime(number)) return number;
            }
            throw new OverflowException();
        }
    }
}
