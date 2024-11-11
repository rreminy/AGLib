using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Generic
{
    /// <summary>A collection of <see cref="IAsyncDisposable"/> items.</summary>
    /// <remarks>Disposing this collection will dispose all items.</remarks>
    public sealed class AsyncDisposableCollection : HashSet<IAsyncDisposable>, IAsyncDisposable, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="AsyncDisposableCollection"/> class.</summary>
        public AsyncDisposableCollection() : base() { }

        /// <summary>Initializes a new instance of the <see cref="AsyncDisposableCollection"/> class with an initial <paramref name="capacity"/>.</summary>
        /// <param name="capacity">Initial collection capacity.</param>
        public AsyncDisposableCollection(int capacity) : base(capacity) { }

        /// <summary>Initializes a new instance of the <see cref="AsyncDisposableCollection"/> class populated with <paramref name="items"/>.</summary>
        /// <param name="items">Initial collection items.</param>
        public AsyncDisposableCollection(IEnumerable<IAsyncDisposable> items) : base(items) { }

        /// <inheritdoc/>
        [SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "Intentional")]
        [SuppressMessage("Design", "CA1031", Justification = "Rethrown as AggregatgeException.")]
        public async ValueTask DisposeAsync()
        {
            List<Exception>? exceptions = null;
            foreach (var disposable in this)
            {
                try
                {
                    await disposable.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    (exceptions ??= []).Add(ex);
                }
            }
            if (exceptions is not null) ThrowHelper.Throw(new AggregateException(exceptions));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
