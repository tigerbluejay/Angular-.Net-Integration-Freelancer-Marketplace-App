// =========================================================
// OPERATION FILTER
// Demonstrates:
// - automatic custom headers
// =========================================================

using Microsoft.OpenApi.Models;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace API.Filters;

public class CorrelationIdOperationFilter : IOperationFilter
{
	public void Apply(
		OpenApiOperation operation,
		OperationFilterContext context)
	{
		operation.Parameters ??=
			new List<OpenApiParameter>();

		operation.Parameters.Add(new OpenApiParameter
		{
			Name = "X-Correlation-Id",
			In = ParameterLocation.Header,
			Required = false,
			Description = "Optional request correlation id"
		});
	}
}