using System;

namespace AG.EnumLocalization.Attributes
{
    /// <summary>Identifies a language key associated with this value.</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumLocAttribute : Attribute
    {
        /// <summary>Gets the language string key for this enum value. Enum value name if <see langword="null"/>.</summary>
        public string? Key { get; init; }

        /// <summary>Gets or initiaslizes the fallback string for this enum value. Enum value name if <see langword="null"/>.</summary>
        public string? Fallback { get; init; }

        /// <summary>Initializes a new instance of the <see cref="EnumLocAttribute"/> class with a specified <paramref name="key"/> and <paramref name="fallback"/>.</summary>
        /// <param name="key">Laguage string key. Enum value name if <see langword="null"/>.</param>
        /// <param name="fallback">Language string fallback. Enum value name if <see langword="null"/>.</param>
        public EnumLocAttribute(string? key = null, string? fallback = null)
        {
            this.Key = key;
            this.Fallback = fallback;
        }
    }
}
