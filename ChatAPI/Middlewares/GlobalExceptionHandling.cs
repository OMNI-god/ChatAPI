using System.Net;
using System.Text.Json;

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
                Guid id=Guid.NewGuid();
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.StatusCode=(int)HttpStatusCode.BadRequest;
                context.Response.ContentType="application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    Id = id,
                    Message = $"{ex.Message}, {ex.InnerException.Message}"
                });
            }
        }

    }
}
