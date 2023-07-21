namespace Cleanception.Application.Identity.Users;

public class UserRolesRequest
{
    public List<RoleDto> UserRoles { get; set; } = new();
}