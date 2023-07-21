using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Identity.Users.Password;

public class ConfirmForgetPasswordRequest : ICommand<Result<bool>>
{
    public string Email { get; set; } = default!;
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class ConfirmForgetPasswordRequestValidator : AbstractValidator<ConfirmForgetPasswordRequest>
{
    public ConfirmForgetPasswordRequestValidator(IUserService _userService, IStringLocalizer<ConfirmForgetPasswordRequestValidator> localizer)
    {
        RuleFor(p => p.NewPassword)
           .NotEmpty()
           .MinimumLength(6);

        RuleFor(p => p.ConfirmPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.NewPassword);

        RuleFor(p => p.Email)
           .NotEmpty()
           .MustAsync(async (field, ct) => await _userService.ExistsWithEmailAsync(field))
               .WithMessage((_, field) => localizer["User not found.", field!]);
    }
}

public class ConfirmForgetPasswordRequestHandler : ICommandHandler<ConfirmForgetPasswordRequest, Result<bool>>
{
    private readonly IUserService _userService;

    public ConfirmForgetPasswordRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(ConfirmForgetPasswordRequest request, CancellationToken cancellationToken)
    {
        return await _userService.ConfirmForgotPasswordAsync(request);
    }
}