```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 155H 3.80GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.202
  [Host]     : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3


```
| Method                 | Mean                | Error             | StdDev            | Gen0        | Gen1        | Gen2        | Allocated    |
|----------------------- |--------------------:|------------------:|------------------:|------------:|------------:|------------:|-------------:|
| StringConcatenation    | 186,502,683.3333 ns | 3,684,240.6034 ns | 4,524,578.7227 ns | 612333.3333 | 607333.3333 | 606333.3333 | 2079529056 B |
| StringBuilderBenchmark |     226,123.7535 ns |     4,484.3603 ns |     4,798.2153 ns |    124.5117 |    124.5117 |    124.5117 |     852834 B |
| ListContains           |       5,299.9140 ns |       105.4003 ns |       190.0585 ns |           - |           - |           - |            - |
| HashSetContains        |           1.5385 ns |         0.0580 ns |         0.0953 ns |           - |           - |           - |            - |
| AnyBenchmark           |           0.2129 ns |         0.0313 ns |         0.0693 ns |           - |           - |           - |            - |
| CountGreaterThanZero   |           0.2642 ns |         0.0306 ns |         0.0528 ns |           - |           - |           - |            - |
