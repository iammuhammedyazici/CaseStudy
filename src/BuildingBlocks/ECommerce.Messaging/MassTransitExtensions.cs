using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Messaging;

public static class MassTransitExtensions
{
    public static void AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(x =>
        {
            configure?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSection = configuration.GetSection("RabbitMq");
                var rabbitMqSectionAlt = configuration.GetSection("RabbitMQ");
                var host = rabbitMqSection["Host"]
                    ?? rabbitMqSection["HostName"]
                    ?? rabbitMqSectionAlt["Host"]
                    ?? rabbitMqSectionAlt["HostName"]
                    ?? "localhost";
                var username = rabbitMqSection["Username"]
                    ?? rabbitMqSection["UserName"]
                    ?? rabbitMqSectionAlt["Username"]
                    ?? rabbitMqSectionAlt["UserName"]
                    ?? "guest";
                var password = rabbitMqSection["Password"]
                    ?? rabbitMqSectionAlt["Password"]
                    ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
