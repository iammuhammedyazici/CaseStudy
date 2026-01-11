using Serilog.Context;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Observability;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headerValue = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        var correlationId = Guid.TryParse(headerValue, out var parsed)
            ? parsed
            : Guid.NewGuid();

        context.Items["CorrelationId"] = correlationId;
        context.Request.Headers["X-Correlation-Id"] = correlationId.ToString();
        context.Response.Headers["X-Correlation-Id"] = correlationId.ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
