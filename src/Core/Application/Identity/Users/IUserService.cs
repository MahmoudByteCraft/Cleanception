using System.Security.Claims;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Identity.Tokens;
using Cleanception.Application.Identity.Users.Password;

namespace Cleanception.Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<PaginatedResult<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);

    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);
    Task<bool> ExistsWithIdAsync(string? userId);
    Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken);
    Task<int> GetCountAsync(CancellationToken cancellationToken);
    Task<UserProfileDto?> GetAsync(string userId, CancellationToken cancellationToken);
    Task<List<RoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken);
    Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken);
    Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken);
    Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
    Task<string> ConfirmEmailAsync(string userId, string code, string tenant, CancellationToken cancellationToken);
    Task<string> ConfirmPhoneNumberAsync(string userId, string code);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request);
    Task ChangePasswordAsync(ChangePasswordRequest request, string userId);
    Task<UserSimplifyDto?> GetSimplifyAsync(string userId, CancellationToken cancellationToken);

    Task<UserCacheDto?> GetCachedUserAsync(string userId, CancellationToken cancellationToken);
    Task RemoveCachedUserAsync(string userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string userId);

    Task<Result<ConfirmationCodeDto>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
    Task<Result<bool>> ConfirmForgotPasswordAsync(ConfirmForgetPasswordRequest request);
}