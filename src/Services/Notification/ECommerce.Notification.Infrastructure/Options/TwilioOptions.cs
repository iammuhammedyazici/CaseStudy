namespace ECommerce.Notification.Infrastructure.Options;

/// <summary>
/// Configuration options for Twilio SMS service
/// </summary>
public class TwilioOptions
{
    public const string SectionName = "Twilio";

    /// <summary>
    /// Twilio Account SID
    /// </summary>
    public string AccountSid { get; set; } = string.Empty;

    /// <summary>
    /// Twilio Auth Token
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// From phone number (E.164 format, e.g., +15551234567)
    /// </summary>
    public string FromPhoneNumber { get; set; } = string.Empty;
}
