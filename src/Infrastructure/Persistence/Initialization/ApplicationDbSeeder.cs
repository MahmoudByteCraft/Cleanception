using Cleanception.Infrastructure.Identity;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Shared.Authorization;
using Cleanception.Shared.Multitenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cleanception.Infrastructure.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder( RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, CustomSeederRunner seederRunner, ILogger<ApplicationDbSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await _seederRunner.RunSeedersAsync(cancellationToken);
        await AssignDefaultRolesPermissions(dbContext);
        await SeedAdminUserAsync();
    }

    private async Task AssignDefaultRolesPermissions(ApplicationDbContext dbContext)
    {
        foreach (string roleName in UPCRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogError("{role} Role Not Found, Please make sure roles.json file contain the default roles", roleName);
                continue;
            }

            // Assign permissions
            if (roleName == UPCRoles.Customer)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Customer, role);
            }
            else if (roleName == UPCRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Admin, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext, IReadOnlyList<FSHPermission> permissions, ApplicationRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == UPCClaims.Permission && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}'.", role.Name, permission.Name);
                dbContext.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = UPCClaims.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbSeeder"
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == RootConstants.EmailAddress)
            is not ApplicationUser adminUser)
        {
            string adminUserName = $"{UPCRoles.Admin}".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FullName = UPCRoles.Admin,
                Email = RootConstants.EmailAddress,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = RootConstants.EmailAddress?.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsSeed = true,
                IsEditAllowed = false,
                IsDeleteAllowed = false,
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User.");
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, RootConstants.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, UPCRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User.");
            await _userManager.AddToRoleAsync(adminUser, UPCRoles.Admin);
        }
    }
}