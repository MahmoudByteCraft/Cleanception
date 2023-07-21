﻿using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Localization;

namespace Cleanception.Infrastructure.Localization;

public static class Startup
{
    public static IServiceCollection AddPOLocalization(this IServiceCollection services, IConfiguration config)
    {
        var localizationSettings = config.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();

        if (localizationSettings?.EnableLocalization is true
             && localizationSettings.ResourcesPath is not null)
        {
            services.AddPortableObjectLocalization(options =>
            {
                options.ResourcesPath = localizationSettings.ResourcesPath;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                if (localizationSettings.SupportedCultures != null)
                {
                    var supportedCultures = localizationSettings.SupportedCultures.Select(x => new CultureInfo(x)).ToList();

                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }

                options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultRequestCulture ?? "en-US");
                options.FallBackToParentCultures = localizationSettings.FallbackToParent ?? true;
                options.FallBackToParentUICultures = localizationSettings.FallbackToParent ?? true;
            });

            services.AddSingleton<ILocalizationFileLocationProvider, UPCPoFileLocationProvider>();
        }

        return services;
    }
}
