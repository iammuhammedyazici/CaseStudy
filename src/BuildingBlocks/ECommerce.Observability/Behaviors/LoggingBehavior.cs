using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ECommerce.Observability.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var uniqueId = Guid.NewGuid().ToString();

        _logger.LogInformation("[{UniqueId}] Handling {RequestName}", uniqueId, requestName);

        var timer = Stopwatch.StartNew();
        try
        {
            var response = await next();
            timer.Stop();

            _logger.LogInformation("[{UniqueId}] Handled {RequestName} successfully in {ElapsedMilliseconds}ms",
                uniqueId, requestName, timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            _logger.LogError(ex, "[{UniqueId}] Error handling {RequestName} after {ElapsedMilliseconds}ms",
                uniqueId, requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
