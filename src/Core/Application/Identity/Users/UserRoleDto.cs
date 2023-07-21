namespace Cleanception.Application.Identity.Users;

public class RoleDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
}

public class UserRoleDto
{
    public UserDetailsDto? User { get; set; }
    public RoleDto? Role { get; set; }
}

public class UserRoleSimplifyDto
{
    public RoleDto? Role { get; set; }
}