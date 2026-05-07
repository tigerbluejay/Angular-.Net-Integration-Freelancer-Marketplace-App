using BenchmarkDotNet.Attributes;
using CloudinaryDotNet;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerformanceSandbox4Benchmarks
{
	////////////////////////////////////////////////////////////
	// DATA
	////////////////////////////////////////////////////////////

	private List<int> _numbers = [];
	private LinkedList<int> _linkedNumbers = [];

	private int[] _predictableData = [];
	private int[] _randomData = [];

	private List<int> _smallWorkload = [];
	private List<int> _hugeWorkload = [];

	////////////////////////////////////////////////////////////
	// SETUP
	////////////////////////////////////////////////////////////

	[GlobalSetup]
	public void Setup()
	{
		_numbers = Enumerable.Range(1, 1_000_000).ToList();

		_linkedNumbers = new LinkedList<int>(_numbers);

		_predictableData = Enumerable
			.Repeat(1, 1_000_000)
			.ToArray();

		var random = new Random(42);

		_randomData = Enumerable
			.Range(1, 1_000_000)
			.Select(_ => random.Next(0, 2))
			.ToArray();

		_smallWorkload = Enumerable.Range(1, 100).ToList();

		_hugeWorkload = Enumerable.Range(1, 1_000_000).ToList();
	}

	////////////////////////////////////////////////////////////
	// EXERCISE 1
	// LINQ VS FOREACH
	////////////////////////////////////////////////////////////

	[Benchmark]
	public List<int> LinqHeavyPipeline()
	{
		return _numbers
			.Where(x => x % 2 == 0)
			.Select(x => x * 2)
			.Where(x => x > 1000)
			.OrderBy(x => x)
			.Take(50000)
			.ToList();
	}

	[Benchmark]
	public List<int> ManualForeachLoop()
	{
		var result = new List<int>();

		foreach (var number in _numbers)
		{
			if (number % 2 != 0)
				continue;

			var doubled = number * 2;

			if (doubled <= 1000)
				continue;

			result.Add(doubled);

			if (result.Count >= 50000)
				break;
		}

		result.Sort();

		return result;
	}

	////////////////////////////////////////////////////////////
	// EXERCISE 2
	// PARALLELIZATION TRAP
	////////////////////////////////////////////////////////////

	[Benchmark]
	public int SequentialSmallWorkload()
	{
		int total = 0;

		foreach (var item in _smallWorkload)
		{
			total += ExpensiveOperation(item);
		}

		return total;
	}

	[Benchmark]
	public int ParallelSmallWorkload()
	{
		int total = 0;

		Parallel.ForEach(_smallWorkload, item =>
		{
			Interlocked.Add(ref total, ExpensiveOperation(item));
		});

		return total;
	}

	[Benchmark]
	public int SequentialHugeWorkload()
	{
		int total = 0;

		foreach (var item in _hugeWorkload)
		{
			total += ExpensiveOperation(item);
		}

		return total;
	}

	[Benchmark]
	public int ParallelHugeWorkload()
	{
		int total = 0;

		Parallel.ForEach(_hugeWorkload, item =>
		{
			Interlocked.Add(ref total, ExpensiveOperation(item));
		});

		return total;
	}

	////////////////////////////////////////////////////////////
	// EXERCISE 3
	// BRANCH PREDICTION
	////////////////////////////////////////////////////////////

	[Benchmark]
	public int PredictableCondition()
	{
		int count = 0;

		foreach (var value in _predictableData)
		{
			if (value == 1)
			{
				count++;
			}
		}

		return count;
	}

	[Benchmark]
	public int RandomCondition()
	{
		int count = 0;

		foreach (var value in _randomData)
		{
			if (value == 1)
			{
				count++;
			}
		}

		return count;
	}

	////////////////////////////////////////////////////////////
	// EXERCISE 4
	// CACHE LOCALITY
	////////////////////////////////////////////////////////////

	[Benchmark]
	public long ArrayTraversal()
	{
		long total = 0;

		for (int i = 0; i < _numbers.Count; i++)
		{
			total += _numbers[i];
		}

		return total;
	}

	[Benchmark]
	public long LinkedListTraversal()
	{
		long total = 0;

		foreach (var number in _linkedNumbers)
		{
			total += number;
		}

		return total;
	}

	////////////////////////////////////////////////////////////
	// HELPER
	////////////////////////////////////////////////////////////

	private int ExpensiveOperation(int value)
	{
		double result = value;

		for (int i = 0; i < 50; i++)
		{
			result = Math.Sqrt(result + i);
		}

		return (int)result;
	}
}

/*
What You Should Expect
Exercise 1 — LINQ vs foreach

Usually:

LINQ allocates more
foreach often faster
difference may or may not justify readability tradeoff

This is a VERY realistic backend optimization scenario.

Exercise 2 — Parallelization Trap

Usually:

small workload → parallel slower
huge workload → parallel eventually wins

This teaches:
parallelism has overhead.

Very important lesson.

Exercise 3 — Branch Prediction

Usually:

predictable condition faster
random condition noisier/slower

This one is more educational than practically critical for backend work.

Exercise 4 — Cache Locality

Usually:

array/list traversal significantly faster
linked list traversal surprisingly slower

This demonstrates:
memory layout matters.

Even when algorithmic complexity looks similar.
*/