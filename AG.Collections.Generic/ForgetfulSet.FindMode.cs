using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Collections.Generic
{
    /// <content>Contains the <see cref="FindMode"/> enum.</content>
    public sealed partial class ForgetfulSet<T>
    {
        private enum FindMode
        {
            /// <summary>Result will be whether item was found.</summary>
            Get,

            /// <summary>Result will be whether item was found.</summary>
            Add,

            /// <summary>Result will be whether item was removed.</summary>
            Remove,
        }
    }
}
