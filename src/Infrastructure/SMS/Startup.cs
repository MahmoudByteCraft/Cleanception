using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cleanception.Infrastructure.SMS;

internal static class Startup
{
    internal static IServiceCollection AddTwilioSms(this IServiceCollection services, IConfiguration config) =>
        services.Configure<SmsSettings>(config.GetSection(nameof(SmsSettings)));
}