using System.Net;
using System.Text.Json;

namespace portfolio_api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                Message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
                Detail = _env.IsDevelopment() ? ex.StackTrace : null,
                ErrorType = ex.GetType().Name
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}