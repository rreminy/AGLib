using Xunit;

namespace AG.Tests
{
    public static class SplitHash32Tests
    {
        [Theory]
        [InlineData("Hello, World!", unchecked(0x6add5626))]
        [InlineData("poke poke", unchecked(0x432fc354))]
        [InlineData("Gem was here", unchecked(0x2ec3968d))]
        [InlineData("Lousy Gem!", unchecked((int)0x8d7314eb))]
        [InlineData("", unchecked(0x00000000))] // NOTE: Not an error
        public static void TestStringHashes(string text, int hash)
        {
            Assert.StrictEqual(hash, SplitHash32.Compute(text));
        }

        [Theory]
        [InlineData(0, unchecked((int)0x92ca2f0e))]
        [InlineData(unchecked((uint)-1), unchecked(0x36deb503))]
        [InlineData(1337, unchecked((int)0xfee39a8f))]
        [InlineData(1234567890, unchecked((int)0xb8ba284d))]
        [InlineData(1804980865, unchecked(0x4f1e5822))]
        public static void TestNumericalHash(uint value, int hash)
        {
            Assert.StrictEqual(hash, SplitHash32.Compute(value));
        }
    }
}
