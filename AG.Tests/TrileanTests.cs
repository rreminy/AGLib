using System;
using Xunit;

namespace AG.Tests
{
    public static class TrileanTests
    {
        [Fact]
        public static void DefaultConstructionIsNull()
        {
            PerformAssertionTests(new(), false, false, true, false, 0);
        }

        [Fact]
        public static void StaticFields()
        {
            PerformAssertionTests(Trilean.Null, false, false, true, false, 0);
            PerformAssertionTests(Trilean.True, true, false, false, false, 1);
            PerformAssertionTests(Trilean.False, false, true, false, false, -1);
        }

        [Theory]
        [InlineData((sbyte)0, false, false, true, false, (sbyte)0)]
        [InlineData((sbyte)-1, false, true, false, false, (sbyte)-1)]
        [InlineData((sbyte)1, true, false, false, false, (sbyte)1)]
        public static void ConstructionWithSByte(sbyte input, bool isTrue, bool isFalse, bool isNull, bool isInvalid, sbyte value)
        {
            PerformAssertionTests(new(input), isTrue, isFalse, isNull, isInvalid, value);
        }

        [Theory]
        [InlineData(false, false, true, false, false, (sbyte)-1)]
        [InlineData(true, true, false, false, false, (sbyte)1)]
        public static void ConstructionWithBool(bool input, bool isTrue, bool isFalse, bool isNull, bool isInvalid, sbyte value)
        {
            PerformAssertionTests(new(input), isTrue, isFalse, isNull, isInvalid, value);
        }

        [Theory]
        [InlineData(null, false, false, true, false, (sbyte)0)]
        [InlineData(false, false, true, false, false, (sbyte)-1)]
        [InlineData(true, true, false, false, false, (sbyte)1)]
        public static void ConstructionWithNullableBool(bool? input, bool isTrue, bool isFalse, bool isNull, bool isInvalid, sbyte value)
        {
            PerformAssertionTests(new(input), isTrue, isFalse, isNull, isInvalid, value);
        }

        [Theory]
        [InlineData(-7, false, false, false, true)]
        [InlineData(-3, false, false, false, true)]
        [InlineData(-1, false, true, false, false)]
        [InlineData(0, false, false, true, false)]
        [InlineData(1, true, false, false, false)]
        [InlineData(3, false, false, false, true)]
        [InlineData(7, false, false, false, true)]
        public static void RawValues(sbyte input, bool isTrue, bool isFalse, bool isNull, bool isInvalid)
        {
            // By all means, if you want to break the rules then sure!
            PerformAssertionTests(Trilean.FromRawValueUnsafe(input), isTrue, isFalse, isNull, isInvalid, input);

            // Conversion to bools shall remain as undefined behavior
        }

        [Fact]
        public static void ConstructionBadValuesThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Trilean((sbyte)4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Trilean((sbyte)-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Trilean((sbyte)123));
        }

        private static void PerformAssertionTests(Trilean result, bool isTrue, bool isFalse, bool isNull, bool isInvalid, sbyte value)
        {
            Assert.Equal(isTrue, result.IsTrue);
            Assert.Equal(isFalse, result.IsFalse);
            Assert.Equal(isNull, result.IsNull);
            Assert.Equal(isInvalid, result.IsInvalid);
            Assert.Equal(value, result.Value);
        }
    }
}
