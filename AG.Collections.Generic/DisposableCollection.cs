using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Generic
{
    /// <summary>A collection of <see cref="IDisposable"/> items.</summary>
    /// <remarks>Disposing this collection will dispose all items.</remarks>
    public sealed class DisposableCollection : HashSet<IDisposable>, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="DisposableCollection"/> class.</summary>
        public DisposableCollection() : base() { }

        /// <summary>Initializes a new instance of the <see cref="DisposableCollection"/> class with an initial <paramref name="capacity"/>.</summary>
        /// <param name="capacity">Initial collection capacity.</param>
        public DisposableCollection(int capacity) : base(capacity) { }

        /// <summary>Initializes a new instance of the <see cref="DisposableCollection"/> class populated with <paramref name="items"/>.</summary>
        /// <param name="items">Initial collection items.</param>
        public DisposableCollection(IEnumerable<IDisposable> items) : base(items) { }

        /// <inheritdoc/>
        [SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "Intentional")]
        [SuppressMessage("Design", "CA1031", Justification = "Rethrown as AggregatgeException.")]
        public void Dispose()
        {
            List<Exception>? exceptions = null;
            foreach (var disposable in this)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    (exceptions ??= []).Add(ex);
                }
            }
            if (exceptions is not null) ThrowHelper.Throw(new AggregateException(exceptions));
        }
    }
}
