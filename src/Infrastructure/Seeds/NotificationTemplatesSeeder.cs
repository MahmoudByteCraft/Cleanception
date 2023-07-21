using System.Reflection;
using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Domain.Notification;
using Cleanception.Infrastructure.Identity;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Infrastructure.Persistence.Initialization;
using Cleanception.Infrastructure.Seeds.Resources;
using Cleanception.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cleanception.Infrastructure.Seeds;

public class NotificationTemplatesSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<NotificationTemplatesSeeder> _logger;

    public NotificationTemplatesSeeder(RoleManager<ApplicationRole> roleManager, ISerializerService serializerService, ILogger<NotificationTemplatesSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
        _roleManager = roleManager;
    }

    public int Order()
    {
        return SeedsOrder.NotificationTemplates;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        _logger.LogInformation("Started To Seed NotificationTemplates.");

        // Here you can use your own logic to populate the database.
        // As an example, I am using a JSON file to populate the database.
        string notificationTemplatesSource = await File.ReadAllTextAsync(path + "/Seeds/Resources/notificationTemplates.json", cancellationToken);
        var fileNotificationTemplates = _serializerService.Deserialize<List<NotificationTemplate>>(notificationTemplatesSource);

        var notificationTemplates = await _db.NotificationTemplates.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

        if(notificationTemplates.Count == 0 && fileNotificationTemplates.Count > 0)
        {
            _logger.LogWarning("Seed Notification Templates role");

            var roles = await _db.Roles.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

            fileNotificationTemplates.ForEach(x =>
            {
                if(x.RoleId != null)
                {
                    x.RoleId = roles.FirstOrDefault(y => y.Name == RoleConstants.GetRoleKeyValue(x.RoleId!))?.Id;
                }
            });

            if(fileNotificationTemplates.Count > 0)
            {
                await _db.NotificationTemplates.AddRangeAsync(fileNotificationTemplates.Where(x => x.RoleId.HasValue() || !x.RoleId.HasValue() && x.IsSelfTrigger));

                await _db.SaveChangesAsync();
            }

        }

        _logger.LogInformation("Seeded Notification Templates.");
    }
}