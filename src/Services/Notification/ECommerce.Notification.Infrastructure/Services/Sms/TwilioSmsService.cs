using ECommerce.Contracts.Common;
using ECommerce.Notification.Application.Abstractions.Services;
using ECommerce.Notification.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ECommerce.Notification.Infrastructure.Services.Sms;

/// <summary>
/// Twilio implementation of SMS service
/// </summary>
public class TwilioSmsService : ISmsService
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(
        IOptions<TwilioOptions> options,
        ILogger<TwilioSmsService> logger)
    {
        _options = options.Value;
        _logger = logger;

        TwilioClient.Init(_options.AccountSid, _options.AuthToken);
    }

    public async Task<Result> SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var messageResource = await MessageResource.CreateAsync(
                to: new PhoneNumber(to),
                from: new PhoneNumber(_options.FromPhoneNumber),
                body: message);

            if (messageResource.ErrorCode == null)
            {
                _logger.LogInformation("SMS sent successfully to {To}. SID: {Sid}", to, messageResource.Sid);
                return Result.Success();
            }

            _logger.LogError("Failed to send SMS to {To}. Error: {ErrorCode} - {ErrorMessage}",
                to, messageResource.ErrorCode, messageResource.ErrorMessage);

            return Result.Failure($"Failed to send SMS: {messageResource.ErrorMessage}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending SMS to {To}", to);
            return Result.Failure($"Exception while sending SMS: {ex.Message}");
        }
    }
}
