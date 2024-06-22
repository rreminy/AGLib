using System.Runtime.CompilerServices;
using static AG.Collections.Concurrent.Buckets.Bucket;

namespace AG.Collections.Concurrent.Buckets
{
    internal sealed partial class LeafBucket<T>
    {
        [InlineArray(LeafBucketSize)]
        internal struct Entries
        {
            public T _element0;
        }
    }
}
