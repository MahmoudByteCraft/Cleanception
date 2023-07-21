using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;

namespace Cleanception.Application.Identity.Users.Password;

public class ResetPasswordRequest : ICommand<Result<string>>
{
    public string? Id { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator(IUserService _userService, IStringLocalizer<ResetPasswordRequestValidator> localizer)
    {
        RuleFor(p => p.Password)
           .NotEmpty()
           .MinimumLength(6);

        RuleFor(p => p.ConfirmPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.Password);

        RuleFor(p => p.Id)
           .NotEmpty()
           .MustAsync(async (field, ct) => await _userService.ExistsWithIdAsync(field))
               .WithMessage((_, field) => localizer["User not found.", field!]);
    }
}

public class ResetPasswordRequestHandler : ICommandHandler<ResetPasswordRequest, Result<string>>
{
    private readonly IUserService _userService;

    public ResetPasswordRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<string>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        return await _userService.ResetPasswordAsync(request);
    }
}