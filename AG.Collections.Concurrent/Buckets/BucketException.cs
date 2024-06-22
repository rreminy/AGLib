using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AG.Collections.Concurrent.Buckets
{
    /// <summary>Represents an error that occurs inside buckets.</summary>
    public sealed class BucketException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="BucketException"/> class.</summary>
        public BucketException() { }

        /// <summary>Initializes a new instance of the <see cref="BucketException"/> class with a <paramref name="message"/>.</summary>
        /// <param name="message">Exception message.</param>
        public BucketException(string? message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="BucketException"/> class with a <paramref name="message"/> and an <paramref name="innerException"/>.</summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/>.</param>
        public BucketException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>Throws a <see cref="BucketException"/>.</summary>
        /// <exception cref="BucketException">Thrown upon invocation.</exception>
        [DoesNotReturn] public static void Throw() => throw new BucketException();

        /// <summary>Throws a <see cref="BucketException"/> with a <paramref name="message"/>.</summary>
        /// <param name="message">Exception message.</param>
        /// <exception cref="BucketException">Thrown upon invocation.</exception>
        [DoesNotReturn] public static void Throw(string? message) => throw new BucketException(message);

        /// <summary>Throws a <see cref="BucketException"/> with a <paramref name="message"/> and <paramref name="innerException"/>.</summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/>.</param>
        /// <exception cref="BucketException">Thrown upon invocation.</exception>
        [DoesNotReturn] public static void Throw(string? message, Exception? innerException) => throw new BucketException(message, innerException);

        /// <summary>Throws a <see cref="BucketException"/> if <paramref name="condition"/> is true.</summary>
        /// <param name="condition">Condition to use before throwing.</param>
        /// <exception cref="BucketException">Thrown upon invocation if <paramref name="condition"/> is true.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition)
        {
            if (condition) Throw();
        }

        /// <summary>Throws a <see cref="BucketException"/> if <paramref name="condition"/> is true.</summary>
        /// <param name="condition">Condition to use before throwing.</param>
        /// <param name="message">Exception message.</param>
        /// <exception cref="BucketException">Thrown upon invocation if <paramref name="condition"/> is true.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string? message)
        {
            if (condition) Throw(message);
        }

        /// <summary>Throws a <see cref="BucketException"/> if <paramref name="condition"/> is true.</summary>
        /// <param name="condition">Condition to use before throwing.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/>.</param>
        /// <exception cref="BucketException">Thrown upon invocation if <paramref name="condition"/> is true.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string? message, Exception? innerException)
        {
            if (condition) Throw(message, innerException);
        }
    }
}
