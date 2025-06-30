using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace AG.EnumLocalization.Tests
{
    public class BasicTests
    {
        private readonly Dictionary<string, string> _fallbacks;

        private ITestOutputHelper Output { get; }

        public BasicTests(ITestOutputHelper output)
        {
            EnumLoc.SetupAssembly();
            this.Output = output;
            this._fallbacks = new(EnumLoc.GetKeysAndFallbacks());
        }

        [Theory]
        [InlineData(Color.Red, "Color.Red", "Red")]
        [InlineData(Color.Green, "Color.Green", "Green")]
        [InlineData(Color.Blue, "Color.Blue", "Blue")]
        [InlineData(Color.Cyan, "Color.Cyan", "Cyan")]
        [InlineData(Color.Magenta, "Color.Magenta", "Magenta")]
        [InlineData(Color.Yellow, "Color.Yellow", "Yellow")]
        [InlineData(Color.Black, "Color.Black", "Black")]
        [InlineData(Color.White, "Color.White", "White")]
        [InlineData(Direction.North, "Compass.N", "N")]
        [InlineData(Direction.Northeast, "Compass.NE", "NE")]
        [InlineData(Direction.East, "Compass.E", "E")]
        [InlineData(Direction.Southeast, "Compass.SE", "SE")]
        [InlineData(Direction.South, "Compass.S", "S")]
        [InlineData(Direction.Southwest, "Compass.SW", "SW")]
        [InlineData(Direction.West, "Compass.W", "W")]
        [InlineData(Direction.Northwest, "Compass.NW", "NW")]
        [InlineData(Direction.N, "Compass.N", "N")] // Alias
        [InlineData(Direction.NE, "Compass.NE", "NE")] // Alias
        [InlineData(Direction.E, "Compass.E", "E")] // Alias
        [InlineData(Direction.SE, "Compass.SE", "SE")] // Alias
        [InlineData(Direction.S, "Compass.S", "S")] // Alias
        [InlineData(Direction.SW, "Compass.SW", "SW")] // Alias
        [InlineData(Direction.W, "Compass.W", "W")] // Alias
        [InlineData(Direction.NW, "Compass.NW", "NW")] // Alias
        [InlineData(Direction.Up, "Compass.Up", "Up")] // Alias
        [InlineData(Direction.Down, "Compass.Down", "Down")]
        [InlineData(Direction.Forward, "Compass.Forward", "Front")]
        [InlineData(Direction.Backward, "Compass.Backward", "Back")]
        [InlineData(Direction.Center, "Compass.Middle", "Somewhere")]
        [InlineData(Mood.Happy, "Mood.Happy", "Happy")]
        [InlineData(Mood.Sad, "Mood.Saddened", "T_T")]
        [InlineData(Mood.Funny, "Mood.Funny", "Fun Fun!")]
        [InlineData(Mood.Crying, "Mood.Crying", "Mood.Crying")] // Undefined
        [InlineData(Mood.Fan, "Mood.Fan", "Mood.Fan")] // Undefined
        [InlineData(Mood.Smile, "Mood.Smile", "Mood.Smile")] // Undefined
        [InlineData(Number.One, "One", "1")]
        [InlineData(Number.Two, "Two", "2")]
        [InlineData(Number.Three, "Three", "3")]
        [InlineData(Number.Four, "Four", "4")]
        [InlineData(Number.Five, "Five", "5")]
        [InlineData(Number.Six, "Six", "6")]
        [InlineData(Number.Seven, "Seven", "7")]
        [InlineData(Number.Eight, "Eight", "8")]
        [InlineData(Number.Nine, "Nine", "9")]
        [InlineData(Number.Ten, "Ten", "10")]
        [InlineData(Number.Eleven, "Eleven", "11")]
        [InlineData(Number.Twelve, "Twelve", "12")]
        [InlineData(Aliases.Fun, "Mood.Happy", "Happy")] // Alias
        public void KeyAndFallback<T>(T value, string key, string fallback) where T : struct, Enum
        {
            // Notes:
            // - Undefined: Enum values that are undefined, either by missing EnumLocStrings or having an EnumLoc with an empty key.
            Assert.Equal(key, value.GetLocKey());
            Assert.Equal(fallback, value.GetLocFallback());
        }

        [Fact]
        public void Dump()
        {
            this.Output.WriteLine("Dumping keys and fallbacks. This test always succeed.");
            foreach (var (key, fallback) in this._fallbacks.OrderBy(kvp => kvp.Key))
            {
                this.Output.WriteLine($" - {key}: {fallback}");
            }
            Assert.True(true);
        }
    }
}
