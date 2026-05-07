```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 155H 3.80GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.202
  [Host]     : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3


```
| Method                 | Mean      | Error     | StdDev    | Median    | Gen0    | Gen1   | Allocated |
|----------------------- |----------:|----------:|----------:|----------:|--------:|-------:|----------:|
| TrackingQuery          | 141.44 μs |  2.588 μs |  2.658 μs | 140.95 μs |  1.9531 |      - |  27.47 KB |
| NoTrackingQuery        | 139.71 μs |  0.685 μs |  0.607 μs | 139.61 μs |  2.4414 |      - |  31.12 KB |
| IncludeQuery           | 824.42 μs | 15.990 μs | 22.415 μs | 826.40 μs | 33.2031 | 7.8125 | 428.57 KB |
| ProjectionQuery        | 275.94 μs |  2.405 μs |  2.008 μs | 275.70 μs |  2.9297 |      - |  42.04 KB |
| LinqPipeline           | 202.46 μs |  3.458 μs |  2.888 μs | 202.43 μs |  2.4414 |      - |  31.49 KB |
| ManualLoop             | 197.66 μs |  3.045 μs |  2.699 μs | 198.31 μs |  2.4414 |      - |  31.15 KB |
| SerializeFullEntities  |        NA |        NA |        NA |        NA |      NA |     NA |        NA |
| SerializeProjectedDtos | 183.73 μs |  1.881 μs |  3.344 μs | 183.72 μs |  2.9297 |      - |  41.47 KB |
| AnyProjects            | 100.14 μs |  1.976 μs |  4.696 μs |  97.72 μs |  0.4883 |      - |    8.2 KB |
| CountProjects          |  97.07 μs |  1.348 μs |  1.053 μs |  96.64 μs |  0.4883 |      - |    8.2 KB |
| ListContains           | 106.57 μs |  0.384 μs |  0.341 μs | 106.57 μs |  0.9766 |      - |  13.48 KB |
| HashSetContains        | 108.73 μs |  1.240 μs |  1.522 μs | 108.21 μs |  0.9766 |      - |  13.99 KB |
| SequentialProcessing   | 138.94 μs |  1.610 μs |  1.427 μs | 138.69 μs |  2.4414 |      - |  31.12 KB |
| ParallelProcessing     | 142.98 μs |  2.006 μs |  1.675 μs | 142.24 μs |  2.4414 |      - |  33.96 KB |

Benchmarks with issues:
  PerformanceSandbox5Benchmarks.SerializeFullEntities: DefaultJob
