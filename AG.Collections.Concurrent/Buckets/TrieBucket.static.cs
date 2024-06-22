using System.Runtime.CompilerServices;

namespace AG.Collections.Concurrent.Buckets
{
    internal sealed partial class TrieBucket<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashIndex(int hash, int level) => unchecked((int)(((uint)hash >> (4 * level)) & 0x0000000f));
    }
}
