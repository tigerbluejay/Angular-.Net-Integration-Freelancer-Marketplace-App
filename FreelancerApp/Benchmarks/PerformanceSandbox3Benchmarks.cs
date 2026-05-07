using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerformanceSandbox3Benchmarks
{
	private List<string> _projectTitles = [];
	private List<int> _projectIdsList = [];
	private HashSet<int> _projectIdsHashSet = [];

	////////////////////////////////////////////////////////////
	// SETUP
	////////////////////////////////////////////////////////////

	[GlobalSetup]
	public void Setup()
	{
		// Simulate realistic project titles
		_projectTitles = Enumerable.Range(1, 10000)
			.Select(x => $"Project Title {x}")
			.ToList();

		// Simulate project IDs
		_projectIdsList = Enumerable.Range(1, 100000).ToList();

		// Create HashSet once
		_projectIdsHashSet = _projectIdsList.ToHashSet();
	}

	////////////////////////////////////////////////////////////
	// 1. STRING CONCATENATION
	////////////////////////////////////////////////////////////

	[Benchmark]
	public string StringConcatenation()
	{
		string result = "";

		foreach (var title in _projectTitles)
		{
			result += title + " | ";
		}

		return result;
	}

	////////////////////////////////////////////////////////////
	// 2. STRING BUILDER
	////////////////////////////////////////////////////////////

	[Benchmark]
	public string StringBuilderBenchmark()
	{
		var sb = new StringBuilder();

		foreach (var title in _projectTitles)
		{
			sb.Append(title);
			sb.Append(" | ");
		}

		return sb.ToString();
	}

	////////////////////////////////////////////////////////////
	// 3. LIST.CONTAINS
	////////////////////////////////////////////////////////////

	[Benchmark]
	public bool ListContains()
	{
		return _projectIdsList.Contains(99999);
	}

	////////////////////////////////////////////////////////////
	// 4. HASHSET.CONTAINS
	////////////////////////////////////////////////////////////

	[Benchmark]
	public bool HashSetContains()
	{
		return _projectIdsHashSet.Contains(99999);
	}

	////////////////////////////////////////////////////////////
	// 5. ANY()
	////////////////////////////////////////////////////////////

	[Benchmark]
	public bool AnyBenchmark()
	{
		return _projectIdsList.Any();
	}

	////////////////////////////////////////////////////////////
	// 6. COUNT() > 0
	////////////////////////////////////////////////////////////

	[Benchmark]
	public bool CountGreaterThanZero()
	{
		return _projectIdsList.Count() > 0;
	}
}