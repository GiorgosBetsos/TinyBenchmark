﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    // TODO: rename into BenchmarksCollection ?
    public class BenchmarksContainerAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
