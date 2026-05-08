using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Net.Http.Json;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerformanceSandbox6Benchmarks
{
	private readonly HttpClient _httpClient;

	public PerformanceSandbox6Benchmarks()
	{
		_httpClient = new HttpClient();

		_httpClient.BaseAddress = new Uri("https://localhost:5001/");
	}

	// --------------------------------------------------------
	// WITHOUT REDIS
	// --------------------------------------------------------
	[Benchmark]
	public async Task<string> NoRedis()
	{
		return await _httpClient.GetStringAsync(
			"api/PerformanceSandbox6/dashboard-no-cache");
	}

	// --------------------------------------------------------
	// WITH REDIS
	// --------------------------------------------------------
	[Benchmark]
	public async Task<string> WithRedis()
	{
		return await _httpClient.GetStringAsync(
			"api/PerformanceSandbox6/dashboard-redis");
	}
}