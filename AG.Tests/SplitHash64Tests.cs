using Xunit;

namespace AG.Tests
{
    public static class SplitHash64Tests
    {
        [Theory]
        [InlineData("Hello, World!", unchecked((long)0xc5ceb86928c7506d))]
        [InlineData("poke poke", unchecked(0x06f32b891953e813))]
        [InlineData("Gem was here", unchecked((long)0xb373f28b463ac33e))]
        [InlineData("Lousy Gem!", unchecked((long)0x88eb7269c93a863e))]
        [InlineData("", unchecked((long)0xe220a8397b1dcdaf))]
        public static void TestStringHashes(string text, long hash)
        {
            Assert.StrictEqual(hash, SplitHash64.Compute(text));
        }

        [Theory]
        [InlineData(0, unchecked((long)0xe220a8397b1dcdaf))]
        [InlineData(unchecked((ulong)-1), unchecked((long)0xe4d971771b652c20))]
        [InlineData(1337, unchecked((long)0xb6a8a9b313caa00b))]
        [InlineData(1234567890, unchecked(0x476948b80f74962f))]
        [InlineData(3860353541804980865, unchecked((long)0x83cc682e0acc1678))]
        public static void TestNumericalHash(ulong value, long hash)
        {
            Assert.StrictEqual(hash, SplitHash64.Compute(value));
        }
    }
}
