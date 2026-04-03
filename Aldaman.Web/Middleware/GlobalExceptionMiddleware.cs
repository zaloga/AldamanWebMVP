using System.Net;
using System.Text.Json;

namespace Aldaman.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private RequestDelegate Next { get; }
    private ILogger<GlobalExceptionMiddleware> Logger { get; }

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        Next = next;
        Logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error from the custom middleware.",
            Detailed = exception.Message // In production, you might want to hide this
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
