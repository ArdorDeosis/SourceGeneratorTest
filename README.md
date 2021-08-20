# Benchmark Output
```
// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1679 (1909/November2019Update/19H2)
Intel Core i7-10610U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.301
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT


|               Method |         Mean |      Error |     StdDev |  Gen 0 | Allocated |
|--------------------- |-------------:|-----------:|-----------:|-------:|----------:|
|       WithReflection | 1,229.783 ns | 21.2446 ns | 19.8722 ns | 0.0687 |     288 B |
| WithSourceGeneration |     4.343 ns |  0.1492 ns |  0.1887 ns | 0.0057 |      24 B |

// * Hints *
Outliers
  ToEmojiBenchmark.WithReflection: Default -> 1 outlier  was  detected (1.18 us)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Gen 0     : GC Generation 0 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)

```
