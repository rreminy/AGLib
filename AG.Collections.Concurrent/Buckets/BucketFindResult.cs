using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Concurrent.Buckets
{
    /// <summary>Represent a find results.</summary>
    internal enum BucketFindResult
    {
        /// <summary>Item was not found.</summary>
        NotFound,

        /// <summary>Item was found.</summary>
        Found,

        /// <summary>Item was created.</summary>
        Created,

        /// <summary>Item was replaced.</summary>
        Replaced,

        /// <summary>Item was removed.</summary>
        Removed,
    }
}
