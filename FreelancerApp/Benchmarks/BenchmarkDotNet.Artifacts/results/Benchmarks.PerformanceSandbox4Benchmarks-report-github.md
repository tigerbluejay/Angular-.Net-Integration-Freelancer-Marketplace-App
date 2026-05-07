```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 155H 3.80GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.202
  [Host]     : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3


```
| Method                  | Mean          | Error       | StdDev      | Gen0     | Gen1     | Gen2     | Allocated |
|------------------------ |--------------:|------------:|------------:|---------:|---------:|---------:|----------:|
| LinqHeavyPipeline       |  9,904.878 μs | 195.3152 μs | 371.6074 μs | 140.6250 | 140.6250 | 140.6250 | 6197638 B |
| ManualForeachLoop       |    496.166 μs |   6.8834 μs |   6.4387 μs |  19.5313 |  19.0430 |   9.7656 |  524667 B |
| SequentialSmallWorkload |      9.330 μs |   0.1798 μs |   0.3101 μs |        - |        - |        - |         - |
| ParallelSmallWorkload   |     16.956 μs |   0.2069 μs |   0.1936 μs |   0.2747 |        - |        - |    3506 B |
| SequentialHugeWorkload  | 88,569.777 μs | 123.2689 μs | 109.2746 μs |        - |        - |        - |         - |
| ParallelHugeWorkload    | 41,470.324 μs | 705.7778 μs | 660.1850 μs |        - |        - |        - |    6517 B |
| PredictableCondition    |    244.588 μs |   3.5143 μs |   3.2872 μs |        - |        - |        - |         - |
| RandomCondition         |  3,684.931 μs |  48.0713 μs |  44.9659 μs |        - |        - |        - |         - |
| ArrayTraversal          |    364.407 μs |   3.8798 μs |   3.4393 μs |        - |        - |        - |         - |
| LinkedListTraversal     |  2,411.000 μs |  33.0434 μs |  27.5927 μs |        - |        - |        - |         - |
