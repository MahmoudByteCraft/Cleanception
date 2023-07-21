using Ardalis.Specification.EntityFrameworkCore;
using Cleanception.Application.Common.Caching;
using Cleanception.Application.Common.Events;
using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.FileStorage;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Mailing;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Tokens;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Identity;
using Cleanception.Infrastructure.Auth;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Shared.Authorization;
using Cleanception.Shared.Constants;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Cleanception.Infrastructure.Identity;

internal partial class UserService : IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _db;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly SecuritySettings _securitySettings;
    private readonly IEmailTemplateService _templateService;
    private readonly IFileStorageService _fileStorage;
    private readonly IEventPublisher _events;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUser _currentUser;

    public UserService(IOptions<SecuritySettings> securitySettings, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db, IStringLocalizer<UserService> localizer, IJobService jobService, IMailService mailService, IEmailTemplateService templateService, IFileStorageService fileStorage, IEventPublisher events, ICacheService cache, ICacheKeyService cacheKeys, ITokenService tokenService, ICurrentUser currentUser)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _localizer = localizer;
        _jobService = jobService;
        _mailService = mailService;
        _securitySettings = securitySettings.Value;
        _templateService = templateService;
        _fileStorage = fileStorage;
        _events = events;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _tokenService = tokenService;
        _currentUser = currentUser;
    }

    public async Task<PaginatedResult<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        var users = await _userManager.Users.Include(x => x.ApplicationUserRoles).ThenInclude(x => x.Role)
            .WithSpecification(spec)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync(cancellationToken);

        int count = await _userManager.Users
            .WithSpecification(spec)
            .CountAsync(cancellationToken);

        return PaginatedResult<UserDetailsDto>.Success(users, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        if(!exceptId.HasValue())
        {
            return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser;
        }

        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && (user.Id != exceptId);
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        if (!exceptId.HasValue())
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser;
        }

        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && (user.Id != exceptId);
    }

    public async Task<bool> ExistsWithIdAsync(string? userId)
    {
        return await _userManager.Users.AnyAsync(x => x.Id == userId);
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken) =>
        (await _userManager.Users.Include(x => x.ApplicationUserRoles).ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserDetailsDto>>();

    public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        _userManager.Users.AsNoTracking().CountAsync(cancellationToken);

    public async Task<UserProfileDto?> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Include(x => x.ApplicationUserRoles)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var roles = (await _userManager.GetRolesAsync(user))?.ToList();

        string? profile = string.Empty;

        var dto = user.Adapt<UserProfileDto>();
        if(dto != null)
        {
            dto.Roles = roles;
            dto.ImageUrl = profile.HasValue() ? profile : null;
        }

        return dto;
    }

    public async Task<UserSimplifyDto?> GetSimplifyAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectToType<UserSimplifyDto>()
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        bool isAdmin = await _userManager.IsInRoleAsync(user, UPCRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_localizer["Administrators Profile's Status cannot be toggled"]);
        }

        user.IsActive = request.ActivateUser;

        await _userManager.UpdateAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new NotFoundException(_localizer["user.NotFound"]);

        var result = await _userManager.DeleteAsync(user); // NOTE HARD DELETE
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(_localizer["employee.cannotBeDeletedBecauseOfInternalError"]);
        }

        await _db.SaveChangesAsync();

        await RemoveCachedUserAsync(user.Id, default);

        return true;
    }

    public async Task<UserCacheDto?> GetCachedUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await _cache.GetOrSetAsync($"{CacheConstants.User}-{userId}", slidingExpiration: TimeSpan.FromMinutes(10), cancellationToken: cancellationToken, getItemCallback: async () =>
        {
            return await _db.Users.AsNoTracking().Where(x => x.Id == _currentUser.GetUserId()!.ToString()).ProjectToType<UserCacheDto>().FirstOrDefaultAsync();
        });
    }

    public async Task RemoveCachedUserAsync(string userId, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync($"{CacheConstants.User}-{userId}", cancellationToken);
    }
}