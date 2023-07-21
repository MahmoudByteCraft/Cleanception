using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Identity.Users;

public class UserSimplifyDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
    public DefaultIdType? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
}

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public DefaultIdType? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? AppCustomerId { get; set; }
    public Guid? SalesPersonId { get; set; }
    public Guid? AppSalesPersonId { get; set; }
    public Guid? EmployeeId { get; set; }
    public ICollection<UserRoleSimplifyDto>? ApplicationUserRoles { get; set; }

}

public class AppSalesPersonUserDto : IDto
{
    public string Id { get; set; } = default!;
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
    public DefaultIdType? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? SalesPersonId { get; set; }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ImageUrl { get; set; }

    public DefaultIdType? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? AppCustomerId { get; set; }
    public Guid? SalesPersonId { get; set; }
    public Guid? AppSalesPersonId { get; set; }
    public Guid? EmployeeId { get; set; }
    public List<string>? Roles { get; set; }
}

public class UserCacheDto
{
    public string? Id { get; set; }
    public bool IsActive { get; set; }
}