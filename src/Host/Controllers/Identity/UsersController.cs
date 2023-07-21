using Cleanception.Application.Common.Models;
using Cleanception.Application.Identity.Tokens;
using Cleanception.Application.Identity.Users;
using Cleanception.Application.Identity.Users.Password;

namespace Cleanception.Host.Controllers.Identity;

public class UsersController : VersionNeutralApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [OpenApiOperation("Get list of all users.", "")]
    public Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return _userService.GetListAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    [OpenApiOperation("Get a user's details.", "")]
    public Task<UserProfileDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetAsync(id, cancellationToken);
    }

    [HttpGet("{id}/roles")]
    [OpenApiOperation("Get a user's roles.", "")]
    public Task<List<RoleDto>> GetRolesAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetRolesAsync(id, cancellationToken);
    }

    [HttpPost("{id}/roles")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    [OpenApiOperation("Update a user's assigned roles.", "")]
    public Task<string> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
    {
        return _userService.AssignRolesAsync(id, request, cancellationToken);
    }

    [HttpPost]
    [OpenApiOperation("Creates a new user.", "")]
    public Task<string> CreateAsync(CreateUserRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("{id}/toggle-status")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return BadRequest();
        }

        await _userService.ToggleStatusAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for a user.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Search))]
    public Task<string> ConfirmEmailAsync([FromQuery] string tenant, [FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
    {
        return _userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken);
    }

    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm phone number for a user.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Search))]
    public Task<string> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return _userService.ConfirmPhoneNumberAsync(userId, code);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [OpenApiOperation("Request a password reset email for a user.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    public async Task<Result<ConfirmationCodeDto>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        request.Origin = GetOriginFromRequest();
        return await Mediator.Send(request);
    }

    [HttpPost("confirm-forgot-password")]
    [AllowAnonymous]
    [OpenApiOperation("Request a password reset email for a user.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    public async Task<Result<bool>> ConfirmForgotPasswordAsync(ConfirmForgetPasswordRequest request)
    {
        return await Mediator.Send(request);
    }


    [HttpPost("reset-password")]
    [OpenApiOperation("Reset a user's password.", "")]
    [ApiConventionMethod(typeof(UPCApiConventions), nameof(UPCApiConventions.Register))]
    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return await Mediator.Send(request);
    }

    private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
}