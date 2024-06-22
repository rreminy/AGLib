using System;

namespace AG.Collections.Concurrent.Buckets
{
    [Flags]
    internal enum BucketFindOptions
    {
        None = 0,
        Create = 1,
        Replace = 2,
        Remove = 4,

        // Combination flags
        ModifyingFlags = 7,
    }
}
