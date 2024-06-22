using System.Runtime.CompilerServices;

namespace AG.Collections.Concurrent.Buckets
{
    internal sealed partial class TrieBucket<T>
    {
        [InlineArray(16)]
        internal struct Buckets
        {
            public IBucket<T> _element0;
        }
    }
}
