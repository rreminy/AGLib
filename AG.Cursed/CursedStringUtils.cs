using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AG.Cursed
{
    /// <summary>Cursed string utilities.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CursedStringUtils
    {
        internal static readonly nint StringExpectedMethodTable = GetExpectedMethodTable();

        /// <summary>Try getting the <see cref="string"/> corresponding to a <paramref name="span"/>.</summary>
        /// <remarks>
        /// <para>This is a very unsafe operation using pointer arithmetic.</para>
        /// <para>This method only works on 64-bit processes with 64-bit <see cref="nint"/>.</para>
        /// </remarks>
        /// <param name="span">Span to get the <see cref="string"/> from.</param>
        /// <param name="result"><see cref="string"/> object corresponding to the <paramref name="span"/>.</param>
        /// <returns>Whether a <see cref="string"/> was successfully extracted.</returns>
        [SuppressMessage("Major Code Smell", "S907", Justification = "Remove code duplication.")]
        [SuppressMessage("Major Code Smell", "S125", Justification = "Previous pin-free code.")]
        public static unsafe bool TryGetStringFromSpan(ReadOnlySpan<char> span, [MaybeNullWhen(false)] out string result)
        {
            // Special case
            if (span.Length == 0)
            {
                result = string.Empty;
                return true;
            }

            // This method only works on 64-bit processes.
            if (!Environment.Is64BitProcess || sizeof(nint) != 8) goto Failed;

            // https://devblogs.microsoft.com/premier-developer/managed-object-internals-part-1-layout/
            // An object layout consist of:
            // - Object Header (8 bytes)
            // - Method Table Reference (8 bytes, this is where object references point to)
            // - Fields (variable, for strings the first field is length of type int, followed by the characters inline + null byte)
            // This method attempts to extract the string object from a span if its attached to it.
            // The object header is not checked by this method, only the method table.
            // All we have to do is pray that this layout never changes.

            fixed (void* fixedPtr = span)
            {
                // The span already points to the characters in the string, lets move it backward 12 bytes to point it to the Method Table
                // var ptr = (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)) - 12; // A test failed randomly and I'm assuming its because I avoided pinning.
                var ptr = ((nint)fixedPtr) - 12;

                // 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F | 10 11 12 13 | 14...
                //      Object Header        ^     Method Table          Length      ^ Characters
                //                           |                                       |
                //                          ptr                                    span

                // Observations:
                // - Object Header tends to be almost always 0 unless Monitor has been used.
                // - Method table however is a constant for strings.
                // - This memory structure so far is always aligned to 8 byte multiples.

                // Lets make sure its aligned to an 8 byte boundary (64-bit) and that its within the current page
                // to avoid false positives and potential AccessViolationException, including the Object Header
                if ((ptr & 7) == 0 && ptr % 4096 is < 4084 and >= 8)
                {
                    // Lets take the string's length field
                    var length = *(int*)(ptr + 8);

                    // Lets make sure the length value is the same as the span's, and make sure the method table is the same
                    if (length == span.Length && *(nint*)ptr == StringExpectedMethodTable)
                    {
                        // Lets take the object from the pointer then use the CLR to make sure is a string before returning.
                        var obj = Unsafe.As<nint, object?>(ref ptr);

                        // This is technically reduntant but better be safe, a manual check is already in place via StringExpectedMethodTable
                        // AccessViolationException will be thrown here if this fails. We cannot catch this, so this means if we're
                        // wrong then well... Bye World!, it was nice seeing you all!
                        // We also use SequenceEqual to make sure its the same span. Under the hood its optimized to perform a length and reference check.
                        if (obj is string str && str.AsSpan().SequenceEqual(span))
                        {
                            result = str;
                            return true;
                        }
                    }
                }
            }

        Failed:
            result = default;
            return false;
        }

        private static unsafe nint GetExpectedMethodTable()
        {
            // There's already a string we can take the Method Table from, lets make use of it!
            var str = string.Empty;
            var ptr = (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(str.AsSpan())) - 12;
            return *(nint*)ptr;
        }
    }
}
