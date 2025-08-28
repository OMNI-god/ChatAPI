using Serilog.Context;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;
    public CorrelationIdMiddleware(RequestDelegate next) => this.next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        string correlationID = context.Request.Headers.ContainsKey("X-Correlation-ID")
        ? context.Request.Headers["X-Correlation-ID"].ToString()
        : Guid.NewGuid().ToString();
        context.Response.Headers.Add("X-Correlation-ID", correlationID);

        using (LogContext.PushProperty("CorrelationId", correlationID))
        {
            await next(context);
        }
    }
}