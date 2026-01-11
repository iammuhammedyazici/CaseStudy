using ECommerce.Observability.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Observability;

public static class DependencyInjection
{
    public static IServiceCollection AddObservabilityBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddExceptionHandler<Exceptions.GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
