using Cleanception.Application.Common.Interfaces;
using Cleanception.Infrastructure.Notifications.Firebase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Cleanception.Infrastructure.Notifications;

internal static class Startup
{
    internal static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration config)
    {
        AddSignalrNotifications(services, config);

        AddFirebaseNotifications(services, config);

        return services;
    }

    internal static IServiceCollection AddSignalrNotifications(this IServiceCollection services, IConfiguration config)
    {
        ILogger logger = Log.ForContext(typeof(Startup));

        var signalRSettings = config.GetSection(nameof(SignalRSettings)).Get<SignalRSettings>();

        if (!signalRSettings.UseBackplane)
        {
            services.AddSignalR();
        }
        else
        {
            var backplaneSettings = config.GetSection("SignalRSettings:Backplane").Get<SignalRSettings.Backplane>();
            if (backplaneSettings is null) throw new InvalidOperationException("Backplane enabled, but no backplane settings in config.");
            switch (backplaneSettings.Provider)
            {
                case "redis":
                    if (backplaneSettings.StringConnection is null) throw new InvalidOperationException("Redis backplane provider: No connectionString configured.");
                    services.AddSignalR().AddStackExchangeRedis(backplaneSettings.StringConnection, options =>
                    {
                        options.Configuration.AbortOnConnectFail = false;
                    });
                    break;

                default:
                    throw new InvalidOperationException($"SignalR backplane Provider {backplaneSettings.Provider} is not supported.");
            }

            logger.Information($"SignalR Backplane Current Provider: {backplaneSettings.Provider}.");
        }

        return services;
    }

    internal static IServiceCollection AddFirebaseNotifications(this IServiceCollection services, IConfiguration config)
    {
        ILogger logger = Log.ForContext(typeof(Startup));

        var firebaseSettings = config.GetSection(nameof(FirebaseSettings)).Get<FirebaseSettings>();
        if(firebaseSettings == null)
        {
            logger.Warning($"Firebase configuration file not found");
        }
        else
        {
            if (firebaseSettings.EnableFirebase)
            {
                logger.Information($"Firebase is enabled, loading configuration ...");

                var firebaseAuthSettings = config.GetSection("FirebaseSettings:FirebaseAuthConfiguration").Get<FirebaseAuthConfiguration>();
                if (firebaseAuthSettings is null)
                    throw new InvalidOperationException("Firebase configuration not found.");

                services.AddSingleton(firebaseSettings);
                services.AddSingleton(firebaseAuthSettings);

                services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();

                logger.Information($"Firebase configuration has been loaded successfully !!!");
            }
            else
            {
                logger.Warning($"Firebase is disabled in configuration file");
                services.AddScoped<IFirebaseNotificationService, MockedFirebaseNotificationService>();
            }
        }

        return services;
    }

    internal static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("/notifications", options =>
        {
            options.CloseOnAuthenticationExpiration = true;
        });
        return endpoints;
    }
}