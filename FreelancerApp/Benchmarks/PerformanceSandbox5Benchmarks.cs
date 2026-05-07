using System.Text.Json;
using API.Data;
using API.Entities;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerformanceSandbox5Benchmarks
{
    private DataContext _context = null!;

    ////////////////////////////////////////////////////////////
    // SETUP
    ////////////////////////////////////////////////////////////

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
			.UseSqlServer(
		        "Server=localhost;Database=FreelancerMarketplaceDb;Trusted_Connection=True;TrustServerCertificate=True;")
	        .Options;

		_context = new DataContext(options);
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 1
    // TRACKING VS NO TRACKING
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<List<Project>> TrackingQuery()
    {
        return await _context.Projects
            .Take(500)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<List<Project>> NoTrackingQuery()
    {
        return await _context.Projects
            .AsNoTracking()
            .Take(500)
            .ToListAsync();
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 2
    // INCLUDE VS PROJECTION
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<List<Project>> IncludeQuery()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Client)
            .Include(p => p.Freelancer)
            .Include(p => p.Skills)
            .Include(p => p.Proposals)
            .Take(200)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<List<ProjectDto>> ProjectionQuery()
    {
        return await _context.Projects
            .AsNoTracking()
            .Take(200)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ClientName = p.Client.UserName!,
                FreelancerName = p.Freelancer != null
                    ? p.Freelancer.UserName!
                    : ""
            })
            .ToListAsync();
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 3
    // LINQ PIPELINE VS MANUAL LOOP
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<List<string>> LinqPipeline()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Take(5000)
            .ToListAsync();

        return projects
			.Where(p => p.Description.Length > 100)
			.OrderBy(p => p.Title)
            .Select(p => p.Title)
            .Take(1000)
            .ToList();
    }

    [Benchmark]
    public async Task<List<string>> ManualLoop()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Take(5000)
            .ToListAsync();

        var result = new List<string>();

        foreach (var project in projects)
        {
			if (project.Description.Length <= 100)
				continue;

			result.Add(project.Title);

            if (result.Count >= 1000)
                break;
        }

        result.Sort();

        return result;
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 4
    // SERIALIZATION COST
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<string> SerializeFullEntities()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Include(p => p.Client)
            .Include(p => p.Skills)
            .Include(p => p.Proposals)
            .Take(100)
            .ToListAsync();

        return JsonSerializer.Serialize(projects);
    }

    [Benchmark]
    public async Task<string> SerializeProjectedDtos()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Take(100)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ClientName = p.Client.UserName!
            })
            .ToListAsync();

        return JsonSerializer.Serialize(projects);
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 5
    // ANY VS COUNT
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<bool> AnyProjects()
    {
        return await _context.Projects
            .AsNoTracking()
            .AnyAsync();
    }

    [Benchmark]
    public async Task<bool> CountProjects()
    {
        return await _context.Projects
            .AsNoTracking()
            .CountAsync() > 0;
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 6
    // LIST.CONTAINS VS HASHSET.CONTAINS
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<bool> ListContains()
    {
        var ids = await _context.Projects
            .AsNoTracking()
            .Select(p => p.Id)
            .ToListAsync();

        return ids.Contains(99999);
    }

    [Benchmark]
    public async Task<bool> HashSetContains()
    {
        var ids = await _context.Projects
            .AsNoTracking()
            .Select(p => p.Id)
            .ToListAsync();

        var hashSet = ids.ToHashSet();

        return hashSet.Contains(99999);
    }

    ////////////////////////////////////////////////////////////
    // EXERCISE 7
    // PARALLELIZATION TRAP
    ////////////////////////////////////////////////////////////

    [Benchmark]
    public async Task<int> SequentialProcessing()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Take(5000)
            .ToListAsync();

        int total = 0;

        foreach (var project in projects)
        {
            total += SimulateWork(project.Title);
        }

        return total;
    }

    [Benchmark]
    public async Task<int> ParallelProcessing()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Take(5000)
            .ToListAsync();

        int total = 0;

        Parallel.ForEach(projects, project =>
        {
            Interlocked.Add(
                ref total,
                SimulateWork(project.Title));
        });

        return total;
    }

    ////////////////////////////////////////////////////////////
    // HELPER
    ////////////////////////////////////////////////////////////

    private int SimulateWork(string text)
    {
        int total = 0;

        for (int i = 0; i < text.Length; i++)
        {
            total += text[i];
        }

        return total;
    }

    ////////////////////////////////////////////////////////////
    // DTO
    ////////////////////////////////////////////////////////////

    public class ProjectDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string ClientName { get; set; } = "";

        public string FreelancerName { get; set; } = "";
    }
}

////////////////////////////////////////////////////////////
// BENCHMARK TAKEAWAYS (EF CORE PERFORMANCE RULES)
// Quick reference for interpreting results correctly
////////////////////////////////////////////////////////////

/*
1. Tracking vs AsNoTracking
- Tracking stores entity state in memory (change tracking overhead)
- AsNoTracking avoids change tracker → less memory + faster writes
- Difference is small on tiny datasets, grows with large loads
- Default: use AsNoTracking for read-only queries

------------------------------------------------------------

2. Include vs Projection
- Include loads full object graphs (all related entities)
- Projection selects only needed fields (lean SQL + lean objects)
- Include causes high memory allocation + GC pressure
- Projection is the default for scalable read APIs

------------------------------------------------------------

3. LINQ Pipeline vs Manual Loop
- Both operate AFTER DB materialization (ToListAsync already done)
- LINQ is expressive but adds minimal CPU overhead
- Manual loop can be slightly faster in tight control paths
- Real performance is dominated by database fetch, not loop style

------------------------------------------------------------

4. Full Entity Serialization vs DTO Serialization
- Entities often contain navigation graphs → heavy + risky
- DTOs are flat and purpose-built → minimal serialization cost
- Entity serialization can cause cycles or excessive memory use
- Always expose DTOs in APIs, never EF entities directly

------------------------------------------------------------

5. Any() vs Count() > 0
- Any() translates to EXISTS (optimized for early exit)
- Count() may scan more data unless SQL optimizes it
- Both often perform similarly in SQL Server in simple cases
- Prefer Any() for semantic clarity and intent

------------------------------------------------------------

6. List.Contains vs HashSet.Contains
- List lookup is O(n) linear search
- HashSet lookup is O(1) constant time
- BUT DB/materialization often dominates benchmark results
- HashSet only matters when in-memory lookup is the bottleneck

------------------------------------------------------------

7. Sequential vs Parallel Processing
- Parallel adds thread scheduling + synchronization overhead
- Interlocked operations can cause contention
- Small workloads: sequential is faster and simpler
- Parallel only wins with heavy CPU-bound per-item work

------------------------------------------------------------
*/