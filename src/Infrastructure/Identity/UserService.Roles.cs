using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Identity;
using Cleanception.Shared.Authorization;
using Cleanception.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<RoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<RoleDto>();

        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            userRoles.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Enabled = await _userManager.IsInRoleAsync(user, role.Name)
            });
        }

        return userRoles;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await _userManager.IsInRoleAsync(user, UPCRoles.Admin)
            && request.UserRoles.Any(a => !a.Enabled && a.Name == UPCRoles.Admin))
        {
            // Get count of users in Admin Role
            int adminCount = (await _userManager.GetUsersInRoleAsync(UPCRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == RootConstants.EmailAddress)
            {
                throw new ConflictException(_localizer["Cannot Remove Admin Role From Main Admin User."]);
            }
            else if (adminCount <= 2)
            {
                throw new ConflictException(_localizer["Tenant should have at least 2 Admins."]);
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.Name) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.Name))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.Name);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.Name);
                }
            }
        }

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _localizer["User Roles Updated Successfully."];
    }
}