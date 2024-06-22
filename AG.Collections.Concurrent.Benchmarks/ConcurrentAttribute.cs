using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Concurrent.Benchmarks
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConcurrentAttribute : Attribute
    {
    }
}
