using Cleanception.Application.Common.SMS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Cleanception.Infrastructure.SMS;

public class SmsService : ISmsService
{
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IOptions<SmsSettings> settings, ILogger<SmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        if (_settings == null || !_settings.Enabled)
            return;

        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_settings.FromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber));
        }
        catch
        {

        }
    }

    public async Task<bool> SendMulticast(string message, List<string?>? phoneNumbers, CancellationToken cancellationToken = default)
    {

        if (phoneNumbers == null || phoneNumbers.Count == 0 || !_settings.Enabled)
            return true;

        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            foreach (string? phoneNumber in phoneNumbers)
            {
                await MessageResource.CreateAsync(
                        body: message,
                        from: new Twilio.Types.PhoneNumber(_settings.FromPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(phoneNumber));
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

}