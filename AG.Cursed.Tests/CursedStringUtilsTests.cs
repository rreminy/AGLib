using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace AG.Cursed.Tests
{
    public static class CursedStringUtilsTests
    {
        private static readonly Random _random = new(42);

        // Don't ask me over my choice of strings
        private static readonly string[] s_shortStrings = ["Hello, World!", string.Empty, "Bye, World!", new string("Gem was here."), $"The value of PI is {_random.NextDouble() * Math.PI * 2:F2}", new string('A', 13), new string(new string("Hi~ :3").AsSpan()), _random.NextDouble().ToString("F5")];
        private static readonly string[] s_longStrings = ["Lousy Gem attempted to do cursed things", "The sky is falling apart", "Mercury and The Moon swapped places!", "I'm cooking rice with chicken", "The sun and the moon are of the same size in the sky"];

        public static TheoryData<string> StringsAsData(string fieldName)
        {
            var fieldInfo = typeof(CursedStringUtilsTests).GetField(fieldName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(fieldInfo);

            var obj = fieldInfo.GetValue(null);
            Assert.IsType<string[]>(obj);

            var strings = obj as string[];
            Assert.NotNull(strings);

            return new TheoryData<string>(strings);
        }

        public static TheoryData<string, int, int, bool> AllSpansAsData(string fieldName) => AllSpansAsDataCore(fieldName, 1);

        public static unsafe TheoryData<string, int, int, bool> AllAlignedSpansAsData(string fieldName) => AllSpansAsDataCore(fieldName, sizeof(nint));

        public static unsafe TheoryData<string, int, int, bool> AllSpansAsDataCore(string fieldName, int step)
        {
            var data = new TheoryData<string, int, int, bool>();
            foreach (var str in StringsAsData(fieldName).Select(item => (string)item[0]))
            {
                for (var start = 0; start < str.Length; start += step)
                {
                    for (var end = start; end <= str.Length; end++)
                    {
                        data.Add(str, start, end, start == end || (Environment.Is64BitProcess && sizeof(nint) == 8 && start == 0 && end == str.Length));
                    }
                }
            }
            return data;
        }

        [Theory]
        [MemberData(nameof(StringsAsData), nameof(s_shortStrings))]
        [MemberData(nameof(StringsAsData), nameof(s_longStrings))]
        public static void CanGetString(string str)
        {
            var span = str.AsSpan();
            var success = CursedStringUtils.TryGetStringFromSpan(span, out var result);

            Assert.True(success);
            Assert.Same(str, result);
        }

        [Theory]
        [MemberData(nameof(AllSpansAsData), nameof(s_shortStrings))]
        [MemberData(nameof(AllAlignedSpansAsData), nameof(s_longStrings))]
        public static void SpanTest(string expectedString, int start, int end, bool expectedSuccess)
        {
            var span = expectedString.AsSpan()[start..end];
            var success = CursedStringUtils.TryGetStringFromSpan(span, out var result);

            if (expectedSuccess) Assert.True(success);
            else Assert.False(success);

            if (success)
            {
                if (span.Length == 0) Assert.Same(string.Empty, result);
                else Assert.Same(expectedString, result);
            }
            else
            {
                Assert.Null(result);
            }
        }
    }
}
