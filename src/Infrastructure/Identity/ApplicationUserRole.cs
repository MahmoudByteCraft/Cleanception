using Microsoft.AspNetCore.Identity;

namespace Cleanception.Infrastructure.Identity;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ApplicationRole? Role { get; set; } = default!;
}