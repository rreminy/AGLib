using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AG
{
    public static class MathUtils
    {
        // https://www.geeksforgeeks.org/lucas-primality-test/
        public static int ModPow(int value, int exponent, int modulus)
        {
            var total = value;
            for (var count = 1; count < exponent; count++) total = (total * value) % modulus;
            return total;
        }
    }
}
