using System.Net;
using System.Text.Json;

namespace Store.Middleware;

/// <summary>
/// Middleware responsible for handling exceptions and providing a formatted error response.
/// </summary>
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance for logging error details.</param>
    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware logic.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Process the request and move to the next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception details
            _logger.LogError(ex, ex.Message);

            // Handle the exception by returning a standardized error response
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception by returning a JSON-formatted error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous write operation to the response.</returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set the response content type to JSON
        context.Response.ContentType = "application/json";

        // Set the response status code to 500 (Internal Server Error)
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Create a formatted error response with status code, generic message, and exception details
        var response = new
        {
            status = context.Response.StatusCode,
            message = "An unexpected error occurred.",
            details = exception.Message
        };

        // Write the JSON-formatted error response to the HTTP response
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
