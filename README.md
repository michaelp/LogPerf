# What is the fastest way of (not) logging?

## Context

1) We have many millions of calls in an average workflow
2) Most of the code is heavily instrumented with logging
3) By default the log level configuration is set to `WARN` level so the `DEBUG` / `INFO` level messages are ignored
4) Let's see what is the most performant way way to achieve this

## References     

What is the fastest way of (not) logging? https://logging.apache.org/log4net/release/faq.html

## Results:

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
AMD Ryzen 7 3800X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  DefaultJob : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT


```
|       Method |        Mean |     Error |    StdDev | Ratio |     Gen 0 | Gen 1 | Gen 2 |  Allocated |
|------------- |------------:|----------:|----------:|------:|----------:|------:|------:|-----------:|
|          Nop | 26,229.0 us | 123.33 us | 115.36 us |  1.00 | 1687.5000 |     - |     - | 14320120 B |
| NopOptimized |    725.6 us |  10.52 us |   9.84 us |  0.03 |         - |     - |     - |        1 B |
