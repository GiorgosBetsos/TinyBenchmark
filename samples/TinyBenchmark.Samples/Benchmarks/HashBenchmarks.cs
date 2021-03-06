﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    [BenchmarksContainer(Name = "Hash vs List")]
    public class HashBenchmarks
    {
        [Param(10_000, 100_000, 10_000_000)]
        public int ItemsCount { get; set; }

        [Param("abcdefghijklmnopqrstuvwxyz")]
        public string TargetItem { get; set; }

        private readonly IBenchmarkOutput _output;

        private readonly HashSet<string> _hash = new HashSet<string>();
        private readonly List<string> _list = new List<string>();

        public HashBenchmarks(IBenchmarkOutput output)
        {
            _output = output;
        }

        public void FindInHashInit()
        {
            var targetPosition = this.ItemsCount * 2 / 3;
            for (int i = 0; i < this.ItemsCount; i++)
                if (i == targetPosition)
                    _hash.Add(this.TargetItem);
                else
                    _hash.Add(Guid.NewGuid().ToString());
        }

        public void FindInHashWarmup() => FindInHash(3);

        [Benchmark(Iterations = 3, Name = "HashSet.Contains")]
        [Arguments(10)]
        [InitWith(nameof(FindInHashInit))]
        [WarmupWith(nameof(FindInHashWarmup))]
        public void FindInHash(int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                var found = _hash.Contains(TargetItem);
                if (!found)
                    _output.WriteLine($"Item {TargetItem} was not found in the hash set.");
            }
        }

        public void FindInListInit()
        {
            var targetPosition = this.ItemsCount * 2 / 3;
            for (int i = 0; i < this.ItemsCount; i++)
                if (i == targetPosition)
                    _list.Add(this.TargetItem);
                else
                    _list.Add(Guid.NewGuid().ToString());
        }

        public void FindInListWarmup() => FindInList(1);

        [Benchmark(Iterations = 2, Name = "List.Contains", Baseline = true)]
        [Arguments(10)]
        [InitWith(nameof(FindInListInit))]
        [WarmupWith(nameof(FindInListWarmup))]
        public void FindInList(int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                var found = _list.Contains(TargetItem);
                if (!found)
                    _output.WriteLine($"Item {TargetItem} was not found in the list.");
            }
        }
    }
}
