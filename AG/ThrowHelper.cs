using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AG
{
    /// <summary>Helper methods for throwing.</summary>
    public static class ThrowHelper
    {
        /// <summary>Throws an exception of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T"><see cref="Exception"/>type to throw.</typeparam>
        /// <exception cref="Exception"><see cref="Exception"/> thrown.</exception>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Throw<T>() where T : Exception, new() => throw new T();

        /// <summary>Throws a specified <paramref name="exception"/>.</summary>
        /// <param name="exception"><see cref="Exception"/> to throw.</param>
        /// <exception cref="Exception"><see cref="Exception"/> thrown.</exception>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Throw(Exception exception) => throw exception;

        /// <summary>Throws an exception of type <typeparamref name="T"/> if <paramref name="condition"/> is <see langword="true"/>.</summary>
        /// <typeparam name="T"><see cref="Exception"/>type to throw.</typeparam>
        /// <param name="condition">Condition for throwing.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf<T>([DoesNotReturnIf(true)] bool condition) where T : Exception, new()
        {
            if (condition) Throw<T>();
        }

        /// <summary>Throws a specified <paramref name="exception"/> if <paramref name="condition"/> is <see langword="true"/>.</summary>
        /// <param name="condition">Condition for throwing.</param>
        /// <param name="exception"><see cref="Exception"/> to throw.</param>
        /// <exception cref="Exception"><see cref="Exception"/> thrown whenever <paramref name="condition"/> is <see langword="true"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, Exception exception)
        {
            if (condition) Throw(exception);
        }

        /// <summary>Throws a specified an <see cref="Exception"/> generated from <paramref name="exceptionFactory"/> if <paramref name="condition"/> is <see langword="true"/>.</summary>
        /// <param name="condition">Condition for throwing.</param>
        /// <param name="exceptionFactory"><see cref="Exception"/> to throw.</param>
        /// <exception cref="Exception"><see cref="Exception"/> thrown whenever <paramref name="condition"/> is <see langword="true"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, Func<Exception> exceptionFactory)
        {
            if (condition) Throw(exceptionFactory());
        }

        /// <summary>Throws a specified an <see cref="Exception"/> generated from <paramref name="exceptionFactory"/> if <paramref name="condition"/> is <see langword="true"/>.</summary>
        /// <typeparam name="TState">State type for exception lambda.</typeparam>
        /// <param name="condition">Condition for throwing.</param>
        /// <param name="exceptionFactory"><see cref="Exception"/> to throw.</param>
        /// <param name="state">State to pass to <paramref name="exceptionFactory"/>.</param>
        /// <exception cref="Exception"><see cref="Exception"/> thrown whenever <paramref name="condition"/> is <see langword="true"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf<TState>([DoesNotReturnIf(true)] bool condition, Func<TState, Exception> exceptionFactory, TState state)
        {
            if (condition) Throw(exceptionFactory(state));
        }
    }
}
