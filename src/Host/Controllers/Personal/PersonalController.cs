using Cleanception.Application.Auditing;
using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Identity.Users;
using Cleanception.Application.Identity.Users.Password;
using Cleanception.Application.Notification.NotificationLogs;
using Cleanception.Shared.Authorization;
using Microsoft.Extensions.Localization;

namespace Cleanception.Host.Controllers.Personal;

public class PersonalController : VersionNeutralApiController
{
    private readonly IUserService _userService;
    private readonly IStringLocalizer<PersonalController> _localizer;
    private readonly ICurrentUser _currentUser;

    public PersonalController(ICurrentUser currentUser, IUserService userService, IStringLocalizer<PersonalController> localizer)
    {
        _localizer = localizer;
        _userService = userService;
        _currentUser = currentUser;
    }

    [HttpGet("profile")]
    [OpenApiOperation("Get profile details of currently logged in user.", "")]
    public async Task<ActionResult<UserProfileDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await _userService.GetAsync(userId, cancellationToken));
    }

    [HttpPut("profile")]
    [OpenApiOperation("Update profile details of currently logged in user.", "")]
    public async Task<string> UpdateProfileAsync(UpdateUserRequest request)
    {
        request.Id = User.GetUserId();

        return await Mediator.Send(request);
    }

    [HttpPost("change-password")]
    [OpenApiOperation("Change password of currently logged in user.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest model)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        model.UserId = User.GetUserId();

        await Mediator.Send(model);

        return Ok();
    }

    [HttpGet("permissions")]
    [OpenApiOperation("Get permissions of currently logged in user.", "")]
    public async Task<ActionResult<List<string>>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await _userService.GetPermissionsAsync(userId, cancellationToken));
    }

    [HttpGet("logs")]
    [OpenApiOperation("Get audit logs of currently logged in user.", "")]
    public Task<List<AuditDto>> GetLogsAsync()
    {
        return Mediator.Send(new GetMyAuditLogsRequest());
    }

    [HttpPost("notifications/search")]
    [OpenApiOperation("Search for notifications using available filters.", "")]
    public async Task<PaginatedResult<NotificationLogDto>> SearchAsync(SearchNotificationLogRequest request)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedException("Unauthorized");
        }

        request.UserId = _currentUser.GetUserId();

        return await Mediator.Send(request);
    }

    [HttpPut("notifications/read/{id}")]
    [OpenApiOperation("Mark notification as read", "")]
    public async Task<Result<bool>> MarkAsRead(Guid id)
    {
        return await Mediator.Send(new MarkNotificationLogNotificationRequest(id));
    }

    [HttpPut("notifications/readall")]
    [OpenApiOperation("Mark all notification as read.", "")]
    public async Task<Result<bool>> MarkAllAsRead()
    {
        return await Mediator.Send(new MarkAllNotificationLogAsViewedRequest());
    }

    [HttpGet("notifications/count")]
    [OpenApiOperation("Search for notifications using available filters.", "")]
    public async Task<Result<int>> SearchAsync(bool? viewed)
    {
        return await Mediator.Send(new GetNotificationLogCountRequest(_currentUser.GetUserId().ToString(), viewed));
    }
}