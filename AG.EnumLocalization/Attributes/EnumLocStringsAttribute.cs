using System;

namespace AG.EnumLocalization.Attributes
{
    /// <summary>Identifies a language key prefix associated with all keys from this enum.</summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Enum value keys will be in the form of <c>{prefix}.{key}</c>.</item>
    /// <item>Empty string will result in no prefix: <c>{key}</c>.</item>
    /// <item><see langword="null"/> will result in the type name being the prefix: <c>{type}.{key}</c>, same behavior as if this attribute isn't attached.</item>
    /// <item>Type name is the bare name of the type without namespaces.</item>
    /// </list></remarks>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class EnumLocStringsAttribute : Attribute
    {
        /// <summary>Language string key prefix. Enum type name if <see langword="null"/>.</summary>
        public string? KeyPrefix { get; init; }

        /// <summary>Initializes an <see cref="EnumLocStringsAttribute"/> istance.</summary>
        /// <param name="keyPrefix">Language string key prefix. Enum type name if <see langword="null"/>.</param>
        public EnumLocStringsAttribute(string? keyPrefix = null)
        {
            this.KeyPrefix = keyPrefix;
        }
    }
}
