```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8328/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 155H 3.80GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.203
  [Host]     : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3


```
| Method    | Mean      | Error    | StdDev   | Allocated |
|---------- |----------:|---------:|---------:|----------:|
| NoRedis   | 325.10 ms | 1.855 ms | 1.549 ms |  52.48 KB |
| WithRedis |  18.74 ms | 0.358 ms | 0.335 ms |   5.91 KB |
