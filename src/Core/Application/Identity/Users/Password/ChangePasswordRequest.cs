using System.Text.Json.Serialization;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Identity.Users.Password;

public class ChangePasswordRequest : ICommand<Result<string>>
{
    public string Password { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;

    [JsonIgnore]
    public string? UserId { get; set; }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator(IUserService _userService, IStringLocalizer<ChangePasswordRequestValidator> localizer)
    {
        RuleFor(p => p.Password)
            .NotEmpty();

        RuleFor(p => p.NewPassword)
            .NotEmpty();

        RuleFor(p => p.ConfirmNewPassword)
            .Equal(p => p.NewPassword)
                .WithMessage("Passwords do not match.");

        RuleFor(p => p.UserId)
           .NotEmpty()
           .MustAsync(async (field, ct) => await _userService.ExistsWithIdAsync(field))
               .WithMessage((_, field) => localizer["User not found.", field!]);
    }
}

public class ChangePasswordRequestHandler : ICommandHandler<ChangePasswordRequest, Result<string>>
{
    private readonly IUserService _userService;

    public ChangePasswordRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<string>> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _userService.ChangePasswordAsync(request, request.UserId);

        return await Result<string>.SuccessAsync();
    }
}