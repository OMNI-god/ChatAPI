using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Middlewares
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandling> _logger;

        public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger)
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
                var traceID = context.TraceIdentifier;
                _logger.LogError(ex, "Unhandled exception occurred. TraceID: {TraceId}, Path: {Path}, Method: {Method}, Query: {QueryString}",
                    traceID,
                    context.Request.Path,
                    context.Request.Method,
                    context.Request.QueryString.Value);

                var problem = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "An unexpected error occured",
                    Detail = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceID"] = traceID;

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problem.Status!.Value;

                var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
            }
        }

    }
}
