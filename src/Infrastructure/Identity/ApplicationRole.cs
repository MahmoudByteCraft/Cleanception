using System.ComponentModel.DataAnnotations.Schema;
using Cleanception.Domain.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Cleanception.Infrastructure.Identity;

public class ApplicationRole : IdentityRole, IAggregateRoot
{
    public string? Description { get; set; }
    public bool IsSeed { get; set; }
    public bool IsEditAllowed { get; set; } = true;
    public bool IsDeleteAllowed { get; set; } = true;

    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();

    public ApplicationRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
        ApplicationUserRoles = new HashSet<ApplicationUserRole>();
    }
}