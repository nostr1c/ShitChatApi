using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using System.Net;

namespace ShitChat.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
