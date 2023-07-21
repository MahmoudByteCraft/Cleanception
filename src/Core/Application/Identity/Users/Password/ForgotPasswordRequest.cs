using System.Text.Json.Serialization;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Identity.Tokens;

namespace Cleanception.Application.Identity.Users.Password;

public class ForgotPasswordRequest : ICommand<Result<ConfirmationCodeDto>>
{
    public string Email { get; set; } = default!;

    [JsonIgnore]
    public string? Origin { get; set; }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator(IUserService _userService, IStringLocalizer<ForgotPasswordRequestValidator> localizer)
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");

        RuleFor(p => p.Email)
           .NotEmpty()
           .EmailAddress()
                .WithMessage("Invalid Email Address.")
           .MustAsync(async (field, ct) => await _userService.ExistsWithEmailAsync(field))
               .WithMessage((_, field) => localizer["User not found.", field!]);
    }
}

public class ForgotPasswordRequestHandler : ICommandHandler<ForgotPasswordRequest, Result<ConfirmationCodeDto>>
{
    private readonly IUserService _userService;

    public ForgotPasswordRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<ConfirmationCodeDto>> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        return await _userService.ForgotPasswordAsync(request, request.Origin);
    }
}