using System;
using System.Threading;

namespace AG.Threading
{
    /// <summary>Very similar to <see cref="Lazy{T}"/>, but allows resetting its value.</summary>
    /// <typeparam name="T">Value's type.</typeparam>
    public sealed class ResettableLazy<T>
    {
        private readonly Func<T>? _factory;
        private readonly LazyThreadSafetyMode _mode;
        private Lazy<T> _lazy = default!;

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified <paramref name="factory"/> and <paramref name="mode"/>.</summary>
        /// <param name="factory">Factory to create the value from.</param>
        /// <param name="mode">Thread safety mode.</param>
        public ResettableLazy(Func<T> factory, LazyThreadSafetyMode mode)
        {
            this._factory = factory;
            this._mode = mode;
            this._lazy = this.CreateLazy();
        }

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified <paramref name="factory"/> and <paramref name="threadSafe"/>.</summary>
        /// <param name="factory">Factory to create the value from.</param>
        /// <param name="threadSafe">Thread safe.</param>
        public ResettableLazy(Func<T> factory, bool threadSafe) : this(factory, threadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None) { }

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified <paramref name="factory"/> and <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/> thread safety mode.</summary>
        /// <param name="factory">Factory to create the value from.</param>
        public ResettableLazy(Func<T> factory) : this(factory, LazyThreadSafetyMode.ExecutionAndPublication) { }

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified using the default parameterless constructor as the factory and <paramref name="mode"/> thread safety mode.</summary>
        /// <param name="mode">Thread safety mode.</param>
        public ResettableLazy(LazyThreadSafetyMode mode) : this(null!, mode) { }

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified using the default parameterless constructor as the factory and possibly <paramref name="threadSafe"/>.</summary>
        /// <param name="threadSafe">Thread safe.</param>
        public ResettableLazy(bool threadSafe) : this(null!, threadSafe) { }

        /// <summary>Initializes a new instance of the <see cref="ResettableLazy{T}"/> class with a specified <paramref name="value"/>.</summary>
        /// <param name="value">Value this <see cref="ResettableLazy{T}"/> contains.</param>
        public ResettableLazy(T value) : this(() => value, false) { this._lazy = new(value); }

        /// <summary>Gets a value indicating whether a value has been created.</summary>
        public bool IsValueCreated => this._lazy.IsValueCreated;

        /// <summary>Gets or sets lazily initialized value of the current <see cref="ResettableLazy{T}"/> instance.</summary>
        public T Value
        {
            get => this._lazy.Value;
            set => this._lazy = new(value); // I need it T_T
        }

        /// <summary>Resets this <see cref="ResettableLazy{T}"/>'s value.</summary>
        public void Reset() => Volatile.Write(ref this._lazy, this.CreateLazy());

        /// <summary>Creates and reset this <see cref="ResettableLazy{T}"/>'s value.</summary>
        /// <remarks>Other threads accessing <see cref="Value"/> will still be able to access it while a new one is being made.</remarks>
        public void CreateAndReset()
        {
            var lazy = this.CreateLazy();
            _ = lazy.Value;
            this._lazy = lazy;
        }

        /// <summary>Resets and create this <see cref="ResettableLazy{T}"/>'s value.</summary>
        /// <remarks>Other threads accessing <see cref="Value"/> be blocked until creation is completed.</remarks>
        public void ResetAndCreate()
        {
            var lazy = this.CreateLazy();
            this._lazy = lazy;
            _ = lazy.Value;
        }

        private Lazy<T> CreateLazy()
        {
            if (this._factory is null) return new Lazy<T>(this._mode);
            return new(this._factory, this._mode);
        }
    }
}
