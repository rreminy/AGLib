using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Concurrent.Benchmarks
{
    public enum ConcurrentSetType
    {
        [Concurrent]
        Trie,

        [Concurrent]
        HashTrie,

        [Concurrent]
        Expm,

        Hash,

        Immu,
    }
}
