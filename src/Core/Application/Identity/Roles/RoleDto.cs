using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Identity.Roles;

public class RoleSimplifyDto : IDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class RoleDto : RoleSimplifyDto
{
    public string? Description { get; set; }
    public List<string>? Permissions { get; set; }
}