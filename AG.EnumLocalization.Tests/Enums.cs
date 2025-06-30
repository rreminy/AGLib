using AG.EnumLocalization.Attributes;

namespace AG.EnumLocalization.Tests
{
    [EnumLocStrings]
    public enum Color : byte // Enumerator using byte and manually randomized numbers
    {
        Red = 3, // Color.Red
        Green = 4, // Color.Green
        Blue = 8, // Color.Blue

        Cyan = 14, // Color.Cyan
        Magenta = 12, // Color.Magenta
        Yellow = 18, // Color.Yellow
        Black = 10, // Color.Black

        White = 11, // Color.White
    }

    [EnumLocStrings("Compass")]
    public enum Direction : long // Enumerator using long, with prefix and keys overriden
    {
        [EnumLoc("N")] // Compass.N
        North = 100,

        [EnumLoc("NE")]
        Northeast = 80,

        [EnumLoc("E")]
        East = 60,

        [EnumLoc("SE")]
        Southeast = 40,

        [EnumLoc("S")]
        South = 20,

        [EnumLoc("SW")]
        Southwest = 0,

        [EnumLoc("W")]
        West = -20,

        [EnumLoc("NW")]
        Northwest = -40,

        Up = -60,

        Down = -80,

        [EnumLoc(Fallback = "Front")] // Compass.Forward = Front
        Forward = -100,

        [EnumLoc(Fallback = "Back")]
        Backward = long.MaxValue,

        [EnumLoc("Middle", Fallback = "Somewhere")] // Compass.Middle = Somewhere
        Center = long.MinValue,

        [EnumLocAlias<Direction>(North)]
        N,

        [EnumLocAlias<Direction>(Northeast)]
        NE,

        [EnumLocAlias<Direction>(East)]
        E,

        [EnumLocAlias<Direction>(Southeast)]
        SE,

        [EnumLocAlias<Direction>(South)]
        S,

        [EnumLocAlias<Direction>(Southwest)]
        SW,

        [EnumLocAlias<Direction>(West)]
        W,

        [EnumLocAlias<Direction>(Northwest)]
        NW
    }

    public enum Aliases
    {
        [EnumLocAlias<Mood>(Mood.Happy)]
        Fun,
    }

    public enum Mood
    {
        [EnumLoc] // Mood.Happy
        Happy,

        [EnumLoc("Saddened", Fallback = "T_T")] // Mood.Saddened = T_T
        Sad,

        Crying, // Should not appear

        [EnumLoc(Fallback = "Fun Fun!")] // Mood.Funny = Fun Fun!
        Funny,

        [EnumLoc("")] // Should not appear
        Fan,

        [EnumLoc("", Fallback = "Should also not appear")]
        Smile,
    }

    [EnumLocStrings("")]
    public enum Number
    {
        [EnumLoc(Fallback = "1")]
        One,

        [EnumLoc(Fallback = "2")]
        Two,

        [EnumLoc(Fallback = "3")]
        Three,

        [EnumLoc(Fallback = "4")]
        Four,

        [EnumLoc(Fallback = "5")]
        Five,

        [EnumLoc(Fallback = "6")]
        Six,

        [EnumLoc(Fallback = "7")]
        Seven,

        [EnumLoc(Fallback = "8")]
        Eight,

        [EnumLoc(Fallback = "9")]
        Nine,

        [EnumLoc(Fallback = "10")]
        Ten,

        [EnumLoc(Fallback = "11")]
        Eleven,

        [EnumLoc(Fallback = "12")]
        Twelve,
    }
}
