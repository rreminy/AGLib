using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AG
{
    /// <summary>Represents a value with three states: <see langword="true"/>, <see langword="false"/> and <see langword="null"/>.</summary>
    /// <remarks>
    /// Values are represented internally using an <see cref="sbyte"/>:
    /// <list type="bullet">
    /// <item><c>0</c> represents <see langword="null"/></item>
    /// <item><c>1</c> represents <see langword="true"/></item>
    /// <item><c>-1</c> represents <see langword="false"/></item>
    /// </list>
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204", Justification = "Reviewed.")]
    [DebuggerDisplay("{ToString()}")]
    public readonly struct Trilean : IEquatable<Trilean>, IEquatable<bool>, IEquatable<bool?>
    {
        /// <summary>A <seealso cref="Trilean"/> whose value is <see langword="null"/>.</summary>
        public static readonly Trilean Null = new((sbyte)0);

        /// <summary>A <seealso cref="Trilean"/> whose value is <see langword="true"/>.</summary>
        public static readonly Trilean True = new((sbyte)1);

        /// <summary>A <seealso cref="Trilean"/> whose value is <see langword="false"/>.</summary>
        public static readonly Trilean False = new((sbyte)-1);

        /// <summary>Internal value for this <see cref="Trilean"/>.</summary>
        private readonly sbyte _value;

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <c>0</c> as its value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Trilean() { /* Empty */ }

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Trilean(bool value)
        {
            this._value = (sbyte)(value ? 1 : -1);
        }

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Trilean(bool? value)
        {
            this._value = (sbyte)(value is null ? 0 : value.Value ? 1 : -1);
        }

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Trilean(Trilean value)
        {
            this._value = value._value;
        }

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Trilean(sbyte value)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 1);
            this._value = value;
        }

        /// <summary>Gets the internal value of this trilean.</summary>
        /// <returns><see cref="Trilean"/>'s value.</returns>
        public readonly sbyte Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._value;
        }

        /// <summary>Gets a value indicating whether this <see cref="Trilean"/> is set to <see langword="true"/> or <c>1</c>.</summary>
        /// <returns><see cref="Trilean"/> is true.</returns>
        public readonly bool IsTrue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._value is 1;
        }

        /// <summary>Gets a value indicating whether this <see cref="Trilean"/> is set to <see langword="false"/> or <c>-1</c>.</summary>
        /// <returns><see cref="Trilean"/> is false.</returns>
        public readonly bool IsFalse
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._value is -1;
        }

        /// <summary>Gets a value indicating whether this <see cref="Trilean"/> is set to <see langword="null"/> or <c>0</c>.</summary>
        /// <returns><see cref="Trilean"/> is null.</returns>
        public readonly bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._value is 0;
        }

        /// <summary>Gets a value indicating whether this <see cref="Trilean"/> is set to an invalid value.</summary>
        /// <returns><see cref="Trilean"/> invalidity.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly bool IsInvalid
        {
            // This is not something that should ever happen under normal circumstances
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !this.IsTrue && !this.IsFalse && !this.IsNull;
        }

        /// <summary>Converts this <see cref="Trilean"/> into a <see cref="bool"/> value.</summary>
        /// <returns>A <see cref="bool"/> representing this <see cref="Trilean"/>'s value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ToBoolean()
        {
            if (this._value is 0) ThrowHelper.Throw<NullReferenceException>();
            return this._value is 1;
        }

        /// <summary>Converts this <see cref="Trilean"/> into a <see cref="bool"/>? value.</summary>
        /// <returns>A nullable <see cref="bool"/> representing this <see cref="Trilean"/>'s value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool? ToNullableBoolean()
        {
            if (this._value is 0) return null;
            return this._value is 1;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => this._value is 0 ? "null" : this._value is 1 ? "true" : this._value is -1 ? "false" : "invalid";

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Trilean trilean ? this.Equals(trilean) : obj is bool boolean && this.Equals(boolean);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Trilean other) => this._value == other._value;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(bool other) => this._value == (other ? 1 : -1);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(bool? other) => other is null ? this._value is 0 : other.Value ? this._value is 1 : this._value is -1;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this._value.GetHashCode();

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/> without validating.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        /// <remarks>Behavior of such <see cref="Trilean"/> values are undefined.</remarks>
        /// <returns>A <see cref="Trilean"/> with the specified <paramref name="value"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trilean FromRawValueUnsafe(sbyte value) => Unsafe.As<sbyte, Trilean>(ref value); // I don't know why would someone use this but here it is, enjoy!

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        /// <returns>A <see cref="Trilean"/> with the specified <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trilean FromBoolean(bool value) => new(value);

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        /// <returns>A <see cref="Trilean"/> with the specified <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trilean FromBoolean(bool? value) => new(value);

        /// <summary>Initializes a new instance of the <see cref="Trilean"/> struct with <paramref name="value"/>.</summary>
        /// <param name="value">Value to initialize this trilean with.</param>
        /// <returns>A <see cref="Trilean"/> with the specified <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trilean FromValue(sbyte value) => new(value);

        /// <summary>Converts this <see cref="Trilean"/> into a nullable <see cref="bool"/>.</summary>
        /// <param name="trilean"><see cref="Trilean"/> to convert.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Usage", "CA2225", Justification = "Provided.")]
        public static implicit operator bool?(Trilean trilean) => trilean.ToNullableBoolean();

        /// <summary>Converts this <see cref="Trilean"/> into a <see cref="bool"/>.</summary>
        /// <param name="trilean"><see cref="Trilean"/> to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(Trilean trilean) => trilean.ToBoolean();

        /// <summary>Converts this <see cref="Trilean"/> into a <see cref="sbyte"/>.</summary>
        /// <param name="trilean"><see cref="Trilean"/> to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Usage", "CA2225", Justification = "Provided.")]
        public static implicit operator sbyte(Trilean trilean) => trilean.Value;

        /// <summary>Converts this <see cref="bool"/> into a <see cref="Trilean"/>.</summary>
        /// <param name="value"><see cref="bool"/> to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Trilean(bool value) => new(value);

        /// <summary>Converts this <see cref="bool"/> into a <see cref="Trilean"/>.</summary>
        /// <param name="value"><see cref="bool"/> to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Usage", "CA2225", Justification = "Provided.")]
        public static implicit operator Trilean(bool? value) => new(value);

        /// <summary>Converts this <see cref="bool"/> into a <see cref="Trilean"/>.</summary>
        /// <param name="value"><see cref="bool"/> to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Usage", "CA2225", Justification = "Provided.")]
        public static implicit operator Trilean(sbyte value) => new(value);

        /// <summary>Checks two <see cref="Trilean"/> for equality.</summary>
        /// <param name="left">First <see cref="Trilean"/> to check.</param>
        /// <param name="right">Second <see cref="Trilean"/> to check.</param>
        /// <returns>Equality result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Trilean left, Trilean right) => left.Equals(right);

        /// <summary>Checks two <see cref="Trilean"/> for inequality.</summary>
        /// <param name="left">First <see cref="Trilean"/> to check.</param>
        /// <param name="right">Second <see cref="Trilean"/> to check.</param>
        /// <returns>Inequality result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Trilean left, Trilean right) => !left.Equals(right);
    }
}
