using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace AG.Collections.Concurrent.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            Debug();
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
#else
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
        }

        public static void Debug()
        {
            var tests = new ConcurrentSetIntBenchmarks();
            tests.Type = ConcurrentSetType.Trie;
            tests.ItemCount = 1048576;
            tests.GlobalSetup();
            var count = 0;
            while (count < 65536)
            {
                var sw = Stopwatch.StartNew();
                var set = tests.CreateAndPopulate(true);
                Console.WriteLine($"{++count} => {sw.Elapsed.TotalMilliseconds:F3}ms");
            }
        }
    }
}
