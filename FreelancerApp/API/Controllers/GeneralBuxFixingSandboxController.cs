using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings(GroupName = "GeneralBugFixingSandbox")]
public class GeneralBugFixingSandboxController : ControllerBase
{
	// =========================================================
	// SIMPLE GET
	// Demonstrates:
	// - XML comments
	// - Simple response DTO
	// - 200 status code
	// =========================================================

	/// <summary>
	/// Gets a sample freelancer profile.
	/// </summary>
	[HttpGet("sample-user")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<AppUserResponseDto> GetSampleUser()
	{
		return Ok(new AppUserResponseDto
		{
			Id = 1,
			FirstName = "Jose",
			LastName = "Developer",
			KnownAs = "Senior Dev",
			Country = "Argentina",
			IsAvailable = true
		});
	}

	// =========================================================
	// GET BY ID
	// Demonstrates:
	// - Route parameters
	// - 404 handling
	// - Explicit response types
	// =========================================================

	/// <summary>
	/// Gets a project by id.
	/// </summary>
	[HttpGet("projects/{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult<ProjectResponseDto> GetProjectById(int id)
	{
		if (id <= 0)
			return NotFound();

		return Ok(new ProjectResponseDto
		{
			Id = id,
			Title = "Enterprise Migration",
			Description = "Migrate legacy .NET Framework app to .NET 9",
			Status = ProjectStatus.InProgress
		});
	}

	// =========================================================
	// POST WITH VALIDATION
	// Demonstrates:
	// - Validation attributes
	// - Request DTOs
	// - Automatic Swagger validation docs
	// - CreatedAtAction
	// - uses example to demonstrate SwaggerRequestExample attribute and Swashbuckle.AspNetCore.Filters package
	// when you add the SwaggerRequestExample attribute to an endpoint, it will show the example in the Swagger UI request body section
	// =========================================================

	/// <summary>
	/// Creates a new project.
	/// </summary>
	[HttpPost("projects")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[SwaggerRequestExample(
	typeof(CreateProjectRequestDto),
	typeof(CreateProjectExample))]

	public ActionResult<ProjectResponseDto> CreateProject(
		[FromBody] CreateProjectRequestDto request)
	{
		var response = new ProjectResponseDto
		{
			Id = 99,
			Title = request.Title,
			Description = request.Description,
			Status = ProjectStatus.Pending
		};

		return CreatedAtAction(
			nameof(GetProjectById),
			new { id = response.Id },
			response);
	}

	// =========================================================
	// PUT UPDATE
	// Demonstrates:
	// - PUT semantics
	// - Validation
	// - NoContent
	// =========================================================

	/// <summary>
	/// Updates an existing proposal.
	/// </summary>
	[HttpPut("proposals/{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult UpdateProposal(
		int id,
		[FromBody] UpdateProposalRequestDto request)
	{
		if (id <= 0)
			return BadRequest();

		return NoContent();
	}

	// =========================================================
	// DELETE
	// Demonstrates:
	// - DELETE endpoint
	// - 404 handling
	// =========================================================

	/// <summary>
	/// Deletes a portfolio item.
	/// </summary>
	[HttpDelete("portfolio-items/{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeletePortfolioItem(int id)
	{
		if (id <= 0)
			return NotFound();

		return NoContent();
	}

	// =========================================================
	// QUERY PARAMETERS
	// Demonstrates:
	// - Query string binding
	// - Filtering
	// - Pagination concepts
	// =========================================================

	/// <summary>
	/// Searches projects using query parameters.
	/// </summary>
	[HttpGet("search-projects")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult SearchProjects(
		[FromQuery] string? keyword,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 10,
		[FromQuery] bool onlyAvailable = false)
	{
		return Ok(new
		{
			Keyword = keyword,
			Page = page,
			PageSize = pageSize,
			OnlyAvailable = onlyAvailable
		});
	}

	// =========================================================
	// AUTHORIZED ENDPOINT
	// Demonstrates:
	// - JWT authorization
	// - Swagger Authorize button
	// =========================================================

	/// <summary>
	/// Gets secure freelancer analytics.
	/// </summary>
	[Authorize]
	[HttpGet("secure-analytics")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public IActionResult GetSecureAnalytics()
	{
		return Ok(new
		{
			ActiveProjects = 12,
			MonthlyRevenue = 8500
		});
	}

	// =========================================================
	// FILE UPLOAD
	// Demonstrates:
	// - Multipart/form-data
	// - File picker in Swagger
	// =========================================================

	/// <summary>
	/// Uploads a portfolio image.
	/// </summary>
	[HttpPost("upload-photo")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UploadPhoto(IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("Invalid file");

		return Ok(new
		{
			FileName = file.FileName,
			Size = file.Length
		});
	}

	// =========================================================
	// MODEL BINDING DEBUGGING
	// Demonstrates:
	// - Serialization problems
	// - Validation issues
	// =========================================================

	/// <summary>
	/// Demonstrates model binding and validation.
	/// </summary>
	[HttpPost("debug-model-binding")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult DebugModelBinding(
		[FromBody] CreateProposalRequestDto request)
	{
		return Ok(request);
	}

	// =========================================================
	// HEADERS
	// Demonstrates:
	// - Custom header binding
	// - Correlation IDs
	// =========================================================

	/// <summary>
	/// Demonstrates custom request headers.
	/// </summary>
	[HttpGet("headers")]
	public IActionResult ReadHeaders(
		[FromHeader(Name = "X-Correlation-Id")] string? correlationId)
	{
		return Ok(new
		{
			CorrelationId = correlationId
		});
	}

	// =========================================================
	// HIDDEN ENDPOINT
	// Demonstrates:
	// - IgnoreApi
	// =========================================================

	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpPost("internal-rebuild-cache")]
	public IActionResult InternalRebuildCache()
	{
		return Ok("Cache rebuilt");
	}

	/// <summary>
	/// Demonstrates advanced serialization scenarios.
	/// Useful for debugging circular references and payload shape issues.
	/// </summary>
	[HttpGet("serialization-lab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult SerializationLab()
	{
		var response = new SerializationLabResponseDto
		{
			ProjectId = 100,
			Title = "Enterprise ERP Migration",
			CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),

			Status = ProjectStatus.InProgress,

			Budget = 25000,

			Client = new NestedUserDto
			{
				Id = 1,
				KnownAs = "Jose"
			},

			Freelancer = new NestedUserDto
			{
				Id = 2,
				KnownAs = "Senior Angular Dev"
			},

			Skills = new List<string>
		{
			"C#",
			".NET",
			"Angular",
			"SQL Server"
		},

			Messages = Enumerable.Range(1, 25)
				.Select(x => new MessagePreviewDto
				{
					Id = x,
					Content = $"Message {x}",
					Sent = DateTime.UtcNow.AddMinutes(-x)
				})
				.ToList()
		};

		return Ok(response);
	}

	/// <summary>
	/// Demonstrates ProblemDetails and advanced validation responses.
	/// </summary>
	[HttpPost("proposal-validation-lab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult ProposalValidationLab(
		[FromBody] AdvancedProposalRequestDto request)
	{
		if (request.Bid > request.ProjectBudget)
		{
			return Problem(
				detail: "Proposal bid exceeds project budget.",
				title: "Invalid Proposal",
				statusCode: 400);
		}

		if (request.EstimatedDays <= 0)
		{
			ModelState.AddModelError(
				nameof(request.EstimatedDays),
				"EstimatedDays must be greater than zero.");

			return ValidationProblem(ModelState);
		}

		return Ok(new
		{
			Message = "Proposal accepted for review"
		});
	}

	/// <summary>
	/// Demonstrates pagination headers and large response handling.
	/// </summary>
	[HttpGet("projects-performance-lab")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult ProjectsPerformanceLab(
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 25)
	{
		Response.Headers.Append("X-Total-Count", "1500");

		Response.Headers.Append("X-Page", page.ToString());

		Response.Headers.Append("X-Page-Size", pageSize.ToString());

		var projects = Enumerable.Range(1, pageSize)
			.Select(x => new ProjectListItemDto
			{
				Id = x,
				Title = $"Project {x}",
				Budget = 1000 + x,
				Status = ProjectStatus.InProgress
			});

		return Ok(projects);
	}

}

// =========================================================
// ENUM
// Demonstrates:
// - JsonStringEnumConverter support
// =========================================================

public enum ProjectStatus
{
	Pending,
	InProgress,
	Completed,
	Cancelled
}

// =========================================================
// REQUEST DTOs
// =========================================================

public class CreateProjectRequestDto
{
	[Required]
	[MaxLength(100)]
	public string Title { get; set; } = string.Empty;

	[Required]
	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[Range(100, 100000)]
	public decimal Budget { get; set; }

}

public class UpdateProposalRequestDto
{
	[Required]
	[MaxLength(100)]
	public string Title { get; set; } = string.Empty;

	[Range(1, 100000)]
	public decimal Bid { get; set; }

}

public class CreateProposalRequestDto
{
	[Required]
	[EmailAddress]
	public string FreelancerEmail { get; set; } = string.Empty;

	[Required]
	[MinLength(20)]
	public string Description { get; set; } = string.Empty;

	[Range(50, 50000)]
	public decimal Bid { get; set; }

}

// =========================================================
// RESPONSE DTOs
// =========================================================

public class AppUserResponseDto
{
	public int Id { get; set; }

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string KnownAs { get; set; } = string.Empty;

	public string? Country { get; set; }

	public bool IsAvailable { get; set; }

}

public class ProjectResponseDto
{
	public int Id { get; set; }

	public string Title { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public ProjectStatus Status { get; set; }

}

public class SerializationLabResponseDto
{
	public int ProjectId { get; set; }

	public string Title { get; set; } = string.Empty;

	public DateOnly CreatedDate { get; set; }

	public decimal Budget { get; set; }

	public ProjectStatus Status { get; set; }

	public NestedUserDto? Client { get; set; }

	public NestedUserDto? Freelancer { get; set; }

	public List<string> Skills { get; set; } = [];

	public List<MessagePreviewDto> Messages { get; set; } = [];
}

public class NestedUserDto
{
	public int Id { get; set; }

	public string KnownAs { get; set; } = string.Empty;
}

public class MessagePreviewDto
{
	public int Id { get; set; }

	public string Content { get; set; } = string.Empty;

	public DateTime Sent { get; set; }
}

public class AdvancedProposalRequestDto
{
	[Required]
	[MaxLength(100)]
	public string Title { get; set; } = string.Empty;

	[Range(100, 100000)]
	public decimal Bid { get; set; }

	[Range(100, 100000)]
	public decimal ProjectBudget { get; set; }

	[Range(1, 365)]
	public int EstimatedDays { get; set; }
}

public class ProjectListItemDto
{
	public int Id { get; set; }

	public string Title { get; set; } = string.Empty;

	public decimal Budget { get; set; }

	public ProjectStatus Status { get; set; }
}

public class CreateProjectExample
	: IExamplesProvider<CreateProjectRequestDto>
{
	public CreateProjectRequestDto GetExamples()
	{
		return new CreateProjectRequestDto
		{
			Title = "Modernize Legacy Banking Platform",

			Description =
				"Migrate .NET Framework monolith to microservices.",

			Budget = 45000
		};
	}
}