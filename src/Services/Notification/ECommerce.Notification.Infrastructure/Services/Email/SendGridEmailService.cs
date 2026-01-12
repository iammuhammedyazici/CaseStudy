using ECommerce.Contracts.Common;
using ECommerce.Notification.Application.Abstractions.Services;
using ECommerce.Notification.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ECommerce.Notification.Infrastructure.Services.Email;

/// <summary>
/// SendGrid implementation of email service
/// </summary>
public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailService> logger)
    {
        _options = options.Value;
        _client = new SendGridClient(_options.ApiKey);
        _logger = logger;
    }

    public async Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var from = new EmailAddress(_options.FromEmail, _options.FromName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);

            var response = await _client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To}", to);
                return Result.Success();
            }

            var errorBody = await response.Body.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to send email to {To}. Status: {Status}, Error: {Error}",
                to, response.StatusCode, errorBody);

            return Result.Failure($"Failed to send email: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email to {To}", to);
            return Result.Failure($"Exception while sending email: {ex.Message}");
        }
    }
}
