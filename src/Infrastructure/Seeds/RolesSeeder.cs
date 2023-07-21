using System.Reflection;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Infrastructure.Identity;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Infrastructure.Persistence.Initialization;
using Cleanception.Infrastructure.Seeds.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cleanception.Infrastructure.Seeds;

public class RolesSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<RolesSeeder> _logger;

    public RolesSeeder(RoleManager<ApplicationRole> roleManager, ISerializerService serializerService, ILogger<RolesSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
        _roleManager = roleManager;
    }

    public int Order()
    {
        return SeedsOrder.Roles;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        _logger.LogInformation("Started To Seed Roles.");

        string rolesSource = await File.ReadAllTextAsync(path + "/Seeds/Resources/roles.json", cancellationToken);

        var fileRoles = _serializerService.Deserialize<List<ApplicationRole>>(rolesSource);

        var roles = await _db.Roles.AsNoTracking().Select(x => x.Name).ToListAsync(cancellationToken: cancellationToken);

        foreach (var item in fileRoles.Where(x => !roles.Contains(x.Name)))
        {
            var roleToAdd = new ApplicationRole(item.Name)
            {
                IsSeed = true,
                IsEditAllowed = false,
                IsDeleteAllowed = false
            };

            var result = await _roleManager.CreateAsync(roleToAdd);
            if(!result.Succeeded)
            {
                throw new Exception("Cannot Seed Built In Roles");
            }

            _logger.LogWarning($"Seed \"{item.Name}\" role");
        }

        _logger.LogInformation("Seeded Roles.");
    }
}