
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Any exception that occurs in the whole API will bubble up to this
            // point and be caught by the catch block
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Return a detailed response if we are in development mode
            var response = env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            // In production, we return a simplified error response (no stack trace)
            // so it's safe and easy to handle on the client side.
            // This way the user/client doesn't get confused by verbose errors.
            await context.Response.WriteAsync(json);

        }
    }   

}