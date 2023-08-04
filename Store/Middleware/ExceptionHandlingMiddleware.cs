using Store.Exceptions;
using System.Net;
using System.Text.Json;

namespace Store.Middleware;

/// <summary>
/// Middleware to handle specific custom exceptions and return appropriate HTTP status codes and messages.
/// </summary>
public class ExceptionHandlingMiddleware
{
    // Next middleware in the request processing pipeline.
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware logic.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Process the request and move to the next middleware in the pipeline.
            await _next(context);
        }
        // Catch and handle specific custom exceptions.
        catch (NotFoundException ex)
        {
            // NotFoundException corresponds to HTTP 404 (Not Found).
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
        }
        catch (BadRequestException ex)
        {
            // BadRequestException corresponds to HTTP 400 (Bad Request).
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        // Extend with additional specific exception handlers as needed...
        // Generic exception handler.
        catch (Exception ex)
        {
            // For unexpected exceptions, return HTTP 500 (Internal Server Error).
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Handles exceptions by returning a JSON-formatted error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The caught exception.</param>
    /// <param name="statusCode">The HTTP status code to be returned.</param>
    /// <returns>A task representing the asynchronous write operation to the response.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        // Set the response content type to JSON.
        context.Response.ContentType = "application/json";

        // Set the HTTP status code from the provided HttpStatusCode value.
        context.Response.StatusCode = (int)statusCode;

        // Create a formatted error response with status code and exception message.
        var response = new
        {
            status = context.Response.StatusCode,
            message = exception.Message
        };

        // Write the JSON-formatted error response to the HTTP response.
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
