using ECommerce.Notification.Application.Abstractions.Services;
using ECommerce.Notification.Infrastructure.Options;
using ECommerce.Notification.Infrastructure.Services;
using ECommerce.Notification.Infrastructure.Services.Email;
using ECommerce.Notification.Infrastructure.Services.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NotificationSettings>(
            configuration.GetSection(NotificationSettings.SectionName));
        services.Configure<SendGridOptions>(
            configuration.GetSection(SendGridOptions.SectionName));
        services.Configure<TwilioOptions>(
            configuration.GetSection(TwilioOptions.SectionName));

        var useMockProviders = configuration
            .GetValue<bool>($"{NotificationSettings.SectionName}:UseMockProviders", true);

        if (useMockProviders)
        {
            services.AddScoped<IEmailService, MockEmailService>();
            services.AddScoped<ISmsService, MockSmsService>();
        }
        else
        {
            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<ISmsService, TwilioSmsService>();
        }

        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
