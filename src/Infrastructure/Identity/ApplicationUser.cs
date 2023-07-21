using Cleanception.Application.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cleanception.Infrastructure.Identity;

public class ApplicationUser : IdentityUser, IFirebaseIdentityUser
{
    public ApplicationUser()
    {
        ApplicationUserRoles = new HashSet<ApplicationUserRole>();
    }

    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? FcmToken { get; set; }
    public string? ObjectId { get; set; }
    public DefaultIdType? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public bool IsSeed { get; set; }
    public bool IsEditAllowed { get; set; } = true;
    public bool IsDeleteAllowed { get; set; } = true;

    public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }
}