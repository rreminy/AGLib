using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.Collections.Concurrent.Benchmarks.Experimental;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace AG.Collections.Concurrent.Benchmarks
{
    [SimpleJob]
    [MemoryDiagnoser]
    [EventPipeProfiler(EventPipeProfile.CpuSampling)]
    public class ConcurrentSetIntBenchmarks
    {
        private bool _concurrent;
        private int[] _items = default!;
        private int[] _randomItems = default!;
        private ISet<int> _set = default!;

        //[Params(1, 4, 16, 256, 4096, 65536, 1048576, 16777216)]
        [Params(1048576)]
        //[Params(65536)]
        public int ItemCount { get; set; }

        [Params(ConcurrentSetType.Trie)]//, ConcurrentSetType.Expm, ConcurrentSetType.Hash)]
        public ConcurrentSetType Type { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Console.WriteLine($"Items array created: {Timed(() => this._items = CreateItemsArray(42)):F3}ms");
            Console.WriteLine($"Random array created: {Timed(() => this._randomItems = CreateItemsArray(1337)):F3}ms");

            this._set = this.Create();
            this._concurrent = this.Type.GetType()!
                .GetField(Enum.GetName(this.Type)!)!
                .GetCustomAttributes(true)!
                .Any(attribute => attribute.GetType()! == typeof(ConcurrentAttribute));

            Console.WriteLine($"Creted set of type {this.Type} (Concurrent: {this._concurrent})");

            Console.WriteLine($"Set populated: {Timed(() => this.Populate(this._set, false)):F3}ms");
            Console.WriteLine($"Set Validy: {this.Validate(this._set)}");
        }

        private bool Validate(ISet<int> set)
        {
            var items = this._items;
            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];
                if (!set.Contains(item))
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    return false;
                }
            }

            var randomItems = this._randomItems;
            var reference = items.ToHashSet();
            for (var index = 0; index < randomItems.Length; index++)
            {
                var item = randomItems[index];
                if (set.Contains(item) != reference.Contains(item))
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    return false;
                }
            }

            if (this._items.Length != set.Count)
            {
                if (Debugger.IsAttached) Debugger.Break();
                return false;
            }

            var linqCount = set.Select(x => x).Count();
            if (this._items.Length != linqCount)
            {
                if (Debugger.IsAttached) Debugger.Break();
                return false;
            }
            return true;
        }

        private static double Timed(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        private int[] CreateItemsArray(int seed)
        {
            var random = new Random(seed);
            var buffer = new byte[4];
            return [..Enumerable.Range(0, this.ItemCount).Select(item => { random.NextBytes(buffer); return BitConverter.ToInt32(buffer); }).Distinct().OrderBy(item => random.Next())];
        }

        private ISet<int> Create()
        {
            return this.Type switch
            {
                ConcurrentSetType.Trie => new ConcurrentTrieSet<int>(),
                ConcurrentSetType.Expm => new ConcurrentHashSetSlim<int>(),
                ConcurrentSetType.Hash => new HashSet<int>(),
                ConcurrentSetType.Immu => ImmutableHashSet.CreateBuilder<int>(),
                _ => throw new ArgumentException(nameof(this.Type))
            };
        }

        private void Populate(ISet<int> set, bool parallel)
        {
            if (parallel)
            {
                if (this._concurrent)
                {
                    Parallel.ForEach(this._items, item =>
                    {
                        set.Add(item);
                    });
                }
                else
                {
                    Parallel.ForEach(this._items, item =>
                    {
                        lock (set) set.Add(item);
                    });
                }
            }
            else
            {
                foreach (var item in this._items) set.Add(item);
            }
        }

        [Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public ISet<int> CreateAndPopulate(bool parallel)
        {
            var set = this.Create();
            this.Populate(set, parallel);
            return set;
        }

        //[Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public ISet<int> CreateAndPopulateAndClear(bool parallel)
        {
            var set = this.Create();
            this.Populate(set, parallel);
            set.Clear();
            return set;
        }

        //[Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public ISet<int> ClearAndRepopulate(bool parallel)
        {
            var set = this._set;
            set.Clear();
            this.Populate(set, parallel);
            return set;
        }

        [Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public ISet<int> Contains(bool parallel)
        {
            var set = this._set;
            if (parallel)
            {
                Parallel.ForEach(this._items, item =>
                {
                    _ = set.Contains(item);
                });
            }
            else
            {
                foreach (var item in this._items)
                {
                    _ = set.Contains(item);
                }
            }
            return set;
        }

        //[Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public ISet<int> ContainsRandom(bool parallel)
        {
            var set = this._set;
            if (parallel)
            {
                Parallel.ForEach(this._randomItems, item =>
                {
                    _ = set.Contains(item);
                });
            }
            else
            {
                foreach (var item in this._randomItems)
                {
                    _ = set.Contains(item);
                }
            }
            return set;
        }

        [Benchmark]
        public ISet<int> Clear()
        {
            // The purpose of this benchmark is compute how long it takes for a clear to take place
            // when the set has been populated then cleared. The second condition is achieved indirectly
            // because BenchmarkDotNet run this multiple times anyway.

            var set = this._set;
            this._set.Clear();
            Debug.Assert(this._set.Count == 0);
            return set;

            // ClearAndTrim on the other hand is O(1) and therefore a similar test has not been made.
        }

        [Benchmark]
        public ISet<int> Enumerate()
        {
            var set = this._set;
            foreach (var item in set) _ = item;
            return set;
        }
    }
}
