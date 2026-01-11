using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Stock.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStockApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}
