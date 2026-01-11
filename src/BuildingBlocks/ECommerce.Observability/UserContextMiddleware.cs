using System.Security.Claims;
using Serilog.Context;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Observability;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? context.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
