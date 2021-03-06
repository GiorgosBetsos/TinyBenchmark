# TinyBenchmark

| Package                     | Version                                                                                                                                   | Downloads                                                                                                                                  | Samples                                                                                                                                                                     |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **TinyBenchmark**           | [![Nuget](https://img.shields.io/nuget/v/TinyBenchmark.svg?logo=nuget)](https://www.nuget.org/packages/TinyBenchmark)                     | [![Nuget](https://img.shields.io/nuget/dt/TinyBenchmark.svg?logo=nuget)](https://www.nuget.org/packages/TinyBenchmark)                     | [![Samples](https://img.shields.io/badge/samples-12-brightgreen.svg)](https://github.com/tommasobertoni/TinyBenchmark/tree/master/samples/TinyBenchmark.Samples/Benchmarks) |
| **TinyBenchmark.Exporters** | [![Nuget](https://img.shields.io/nuget/v/TinyBenchmark.Exporters.svg?logo=nuget)](https://www.nuget.org/packages/TinyBenchmark.Exporters) | [![Nuget](https://img.shields.io/nuget/dt/TinyBenchmark.Exporters.svg?logo=nuget)](https://www.nuget.org/packages/TinyBenchmark.Exporters) | [![Samples](https://img.shields.io/badge/samples-0-brightgreen.svg)](https://github.com/tommasobertoni/TinyBenchmark/tree/master/samples/TinyBenchmark.Samples/)            |

[![GitHub last commit](https://img.shields.io/github/last-commit/tommasobertoni/TinyBenchmark.svg)](https://github.com/tommasobertoni/TinyBenchmark/commits/master)
[![GitHub open issues](https://img.shields.io/github/issues-raw/tommasobertoni/TinyBenchmark.svg?color=5566AA)](https://github.com/tommasobertoni/TinyBenchmark/issues)
[![GitHub closed issues](https://img.shields.io/github/issues-closed-raw/tommasobertoni/TinyBenchmark.svg?color=B8B8B8)](https://github.com/tommasobertoni/TinyBenchmark/issues?q=is%3Aclosed)

[![License](https://img.shields.io/github/license/tommasobertoni/TinyBenchmark.svg)](https://github.com/tommasobertoni/TinyBenchmark/blob/master/LICENSE)
<br />

_Define benchmarks with ease!_

TinyBenchmark provides a simple set of APIs that enable you to inspect the execution time of
different methods and compare them between each other.

_**Usage**_
  - [Run your first benchmark](#run-your-first-benchmark)
  - [Define benchmark arguments](#define-benchmark-arguments)
  - [Average the results over multiple iterations](#average-the-results-over-multiple-iterations)
  - [Named benchmarks](#named-benchmarks)
  - [Benchmarks comparison](#benchmarks-comparison)
  - [Benchmarks output](#benchmarks-output)
  
_**Attributes**_
  - **[Benchmark]**: identifies a benchmark method
  - **[Arguments]**: defines the benchmark arguments
  - **[WarmupWith]**: identifies a warmup method to be executed before the benchmark
  - **[Param]**: defines all the possible values of a property that will be used by all the benchmarks
  - **[Init]**: identifies an initialization method to be executed before _each_ benchmark
  - **[InitWith]**: identifies an initialization method to be executed before the benchmark
  - **[BenchmarksContainer]**: defines additional information about the class that contains the benchmarks
  - **[InitContainer]**: identifies an initialization method to be executed _once_ before every benchmark
  
### Run your first benchmark

```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner();
        var report = runner.Run<BenchmarksContainer>();

        // Explore the data in the report!
        Console.WriteLine($"Total duration: {report.Duration}");
    }
}

class BenchmarksContainer
{
    [Benchmark]
    public void TestMe()
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < 10_000; i++)
            msg += token;
    }
}
```

### Define benchmark arguments
```csharp
class BenchmarksContainer
{
    /// <summary>
    /// This benchmark will be executed once for each [Arguments],
    /// with the "times" variable assigned to the value specified the attribute.
    /// </summary>
    [Benchmark]
    [Arguments(10_000)]
    [Arguments(100_000)]
    public void TestMe(int times)
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < times; i++)
            msg += token;
    }
}
```

### Average the results over multiple iterations
```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner();
        var report = runner.Run<BenchmarksContainer>();

        var benchmarkReport = report.Reports.First();
        Console.WriteLine($"Benchmark: {benchmarkReport.Name}");
        Console.WriteLine($"  average iteration duration: {benchmarkReport.AvgIterationDuration}");
    }
}

class BenchmarksContainer
{
    /// <summary>
    /// This benchmark will be executed once for each [Arguments]
    /// and once for each iteration, for a total of 6 runs.
    /// </summary>
    [Benchmark(Iterations = 3)]
    [Arguments(10_000)]
    [Arguments(100_000)]
    public void TestMe(int times)
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < times; i++)
            msg += token;
    }
}
```

### Named benchmarks
```csharp
class BenchmarksContainer
{
    [Benchmark(Name = "String concatenation")]
    public void StringConcatenation()
    {
        string msg = string.Empty;
        for (int i = 0; i < 50_000; i++)
            msg += "test";
    }
}
```

### Benchmarks comparison
```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner();
        var report = runner.Run<BenchmarksContainer>();

        var stringConcatenationBenchmarkReport = report.Reports[0];
        Console.WriteLine($"Benchmark: {stringConcatenationBenchmarkReport.Name}");
        Console.WriteLine($"  average iteration duration: {stringConcatenationBenchmarkReport.AvgIterationDuration}");

        var stringBuilderBenchmarkReport = report.Reports[1];
        Console.WriteLine($"Benchmark: {stringBuilderBenchmarkReport.Name}");
        Console.WriteLine($"  average iteration duration: {stringBuilderBenchmarkReport.AvgIterationDuration}");

        var efficiency = Math.Round(stringConcatenationBenchmarkReport.AvgIterationDuration / stringBuilderBenchmarkReport.AvgIterationDuration, 1);
        Console.WriteLine($"{stringBuilderBenchmarkReport.Name} is {efficiency} times faster than {stringConcatenationBenchmarkReport.Name}!");
    }
}

public class BenchmarksContainer
{
    private readonly string _token = "test";
    private readonly int _tokensCount = 50_000;

    [Benchmark(Name = "String concatenation")]
    public void StringConcatenation()
    {
        string msg = string.Empty;
        for (int i = 0; i < _tokensCount; i++)
            msg += _token;
    }

    [Benchmark(Name = "Concatenation using StringBuilder")]
    public void StringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _tokensCount; i++)
            sb.Append(_token);

        var msg = sb.ToString();
    }
}
```

### Benchmarks output
Each benchmark can write to the console using an instance of `IBenchmarkOutput`.
<br />
Simply declare a constructor with this parameter and the library will inject an instance of it.

Compared to simply using `Console.WriteLine`, this type has the advantage that is aware of the `OutputLevel`
associated with each message, and therefore the user can easily configure how many logs should be displayed on the console.

These are the current values of `OutputLevel`:
- `Verbose`: print everything, **including the benchmarks output**
- `Normal`: print less information compared to _Verbose_, but still give enough details about the execution.
- `Minimal`: print only the main steps of the execution
- `ErrorsOnly`: print only if some unexpected errors occurr
- `Silent`: don't print _"anything"_

Unless specified otherwise, the default output level of a `BenchmarkRunner` is `Normal`
(therefore, by default, the benchmarks' logs won't be shown).

```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner(maxOutputLevel: OutputLevel.Verbose);
        var report = runner.Run<BenchmarksContainer>();

        var benchmarkReport = report.Reports.First();
        Console.WriteLine($"Benchmark: {benchmarkReport.Name}");
        Console.WriteLine($"  average iteration duration: {benchmarkReport.AvgIterationDuration}");
    }
}

class BenchmarksContainer
{
    private readonly IBenchmarkOutput _output;

    public BenchmarksContainer(IBenchmarkOutput output)
    {
        _output = output;
    }

    [Benchmark(Name = "String concatenation")]
    public void StringConcatenation()
    {
        int times = 50_000;
        string token = "test";

        _output.WriteLine($"Concatenating {times} times the string \"{token}\"");

        string msg = string.Empty;
        for (int i = 0; i < times; i++)
            msg += token;

        _output.WriteLine("Terminated");
    }
}
```
