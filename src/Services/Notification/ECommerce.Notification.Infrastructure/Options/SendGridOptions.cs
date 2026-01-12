namespace ECommerce.Notification.Infrastructure.Options;

/// <summary>
/// Configuration options for SendGrid email service
/// </summary>
public class SendGridOptions
{
    public const string SectionName = "SendGrid";

    /// <summary>
    /// SendGrid API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = "noreply@ecommerce.com";

    /// <summary>
    /// From name
    /// </summary>
    public string FromName { get; set; } = "E-Commerce";
}
