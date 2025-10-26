using System.Net;
using System.Text.Json;
using PokedexTL.Application.Exceptions;

namespace PokedexTL.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        HttpStatusCode statusCode;
        if (exception is ArgumentException 
            or ArgumentNullException 
            or PokemonNotFoundException 
            or SpeciesNotFoundException)
        {
            _logger.LogError("Error while attempting retrieving data, user may have inserted wrong input: {Message}"
                , exception.Message);
            statusCode = HttpStatusCode.BadRequest;
        }
        else
        {
            _logger.LogError("Error while attempting retrieving data, unhandled exception: {Message}"
                , exception.Message);
            statusCode = HttpStatusCode.InternalServerError;
        }

        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            error = exception.Message
        };

        var serializedError = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(serializedError);
    }
}