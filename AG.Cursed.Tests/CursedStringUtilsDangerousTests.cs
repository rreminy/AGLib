using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AG.Cursed.Tests
{
    public static class CursedStringUtilsDangerousTests
    {
        [Fact]
        [SuppressMessage("Major Code Smell", "S3998")]
        [SuppressMessage("Major Code Smell", "S2925")]
        [SuppressMessage("Major Bug", "S2445")]
        [SuppressMessage("Critical Bug", "S2551")]
        public static unsafe void DangerouslyManuallyCreatedString()
        {
            if (!Environment.Is64BitProcess || sizeof(nint) != 8) return;
            // This test is actually outside of original specifications for the purpose of
            // CursedStringUtils and was rather a challenge to create a string manually
            // without using any of the string constructors or any of the objects' .ToString()
            // methods then make the CLR perform work by invoking the Garbage Collector then
            // checking back to see if the string object is still valid.

            // Please don't do what this method is about to do!
            // If you do note that I'll not provide support should your application crashes

            // bytesPtr needs to be word-aligned
            var bytesPtr = NativeMemory.AlignedAlloc(48, 8);
            var bytes = new Span<byte>(bytesPtr, 48);
            bytes.Clear();

            // Creating this with new btye[48] array will result in AccessViolationException
            // below as a result of GC involvement, moving the array in the process while
            // the CLR is unaware of manualString referencing a string object there.

            var str = "Hello, World!"; // 13 chars, 26 bytes + 2 null bytes = 28 bytes
            BitConverter.GetBytes(CursedStringUtils.StringExpectedMethodTable).AsSpan().CopyTo(bytes[8..]);
            BitConverter.GetBytes(str.Length).AsSpan().CopyTo(bytes[16..]);
            MemoryMarshal.AsBytes(str.AsSpan()).CopyTo(bytes[20..]);

            // Prepare threads for locking contentions
            var exit = false; // For DangerouslyManuallyCreatedString_LockingThread
            var thread1 = new Thread(obj => DangerouslyManuallyCreatedString_LockingThread(obj!, ref exit));
            var thread2 = new Thread(obj => DangerouslyManuallyCreatedString_LockingThread(obj!, ref exit));

            var span = new Span<char>((byte*)bytesPtr + 20, 13);
            Assert.True(CursedStringUtils.TryGetStringFromSpan(span, out var manualString));
            Assert.Equal(str, manualString);

            // The purpose of these threads are an attempt to upgrade the thin lock into a fat lock as described here:
            // https://devblogs.microsoft.com/premier-developer/managed-object-internals-part-2-object-header-layout-and-the-cost-of-locking/
            // The .GetHashCode() method doesn't work because its overriden by the string class.
            thread1.Start(manualString);
            thread2.Start(manualString);

            // Multiple attempts at testing and invoking the GC
            for (var attempt = 0; attempt < 3; attempt++)
            {
                // Will this survive the CLR?
                var hash = manualString.GetHashCode();
                Assert.Equal(str.GetHashCode(), hash);
                lock (manualString!)
                {
                    Assert.Equal(hash, manualString.GetHashCode()); // NOTE: This is calling string.GetHashCode() and not object.GetHashCode() due to virtual override
                    Assert.IsType<string>(manualString); // MethodTable doubles as a type identifier!
                    Assert.Equal(str, manualString); // NOTE: manualString is a copy
                    Thread.Sleep(100);
                }

                // Lets involve the GC
                AggressiveGC();
                Thread.Sleep(10);

                // And of course, why not?
                hash = manualString.GetHashCode();
                Assert.Equal(str.GetHashCode(), hash);
                lock (manualString!)
                {
                    Thread.Sleep(100);
                    Assert.Equal(hash, manualString.GetHashCode()); // NOTE: This is calling string.GetHashCode() and not object.GetHashCode() due to virtual override
                    Assert.IsType<string>(manualString); // MethodTable doubles as a type identifier!
                    Assert.Equal(str, manualString); // NOTE: manualString is a copy
                }
            }
            Volatile.Write(ref exit, true);
            thread1.Join();
            thread2.Join();
            NativeMemory.AlignedFree(bytesPtr);
            AggressiveGC();
            GC.KeepAlive(manualString);
            AggressiveGC();

            // So apparently the CLR and GC doesn't seem care where an object actually is and therefore
            // this can be done with relative safety, but as mentioned before this is not something you
            // should be doing! There are a lot of variables not tested here and the behavior may change
            // from one release to another. The object header for example is not tested at all here.

            // No support will be provided for using this method's way of creating strings.
        }

        private static void DangerouslyManuallyCreatedString_LockingThread(object obj, ref bool exit)
        {
            Assert.IsType<string>(obj);

            try
            {
                while (true)
                {
                    if (Volatile.Read(ref exit)) return;
                    lock (obj)
                    {
                        AggressiveGC();
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(10);
                    AggressiveGC();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [SuppressMessage("Critical Code Smell", "S1215")]
        private static void AggressiveGC()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
