using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.EnumLocalization.Internal;

namespace AG.EnumLocalization.Attributes
{
    /// <summary>Identifies an alias for this value.</summary>
    /// <typeparam name="T">Alias <see cref="Enum"/>.</typeparam>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402", Justification = "Related.")]
    public sealed class EnumLocAliasAttribute<T> : Attribute where T : struct, Enum
    {
        /// <summary>Gets or initializes a the alias value.</summary>
        public required T Value { get; init; }

        /// <summary>Initializes a new instance of the <see cref="EnumLocAliasAttribute{T}"/> class with a specified <paramref name="value"/>.</summary>
        [SetsRequiredMembers]
        public EnumLocAliasAttribute(T value)
        {
            this.Value = value;
        }
    }
}
