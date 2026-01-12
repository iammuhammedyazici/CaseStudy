namespace ECommerce.Notification.Infrastructure.Options;

/// <summary>
/// General notification settings
/// </summary>
public class NotificationSettings
{
    public const string SectionName = "NotificationSettings";

    /// <summary>
    /// Use mock providers instead of real ones (for development/testing)
    /// </summary>
    public bool UseMockProviders { get; set; } = true;
}
