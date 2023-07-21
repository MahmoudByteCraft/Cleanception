using System.Text.Json.Serialization;
using Cleanception.Application.Common.FileStorage;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Validation;

namespace Cleanception.Application.Identity.Users;

public class UpdateUserRequest : ICommand<string>
{
    [JsonIgnore]
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IUserService userService, IStringLocalizer<UpdateUserRequestValidator> localizer)
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .MaximumLength(75);

        //RuleFor(p => p.LastName)
        //    .NotEmpty()
        //    .MaximumLength(75);

        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress()
                .WithMessage(localizer["Invalid Email Address."])
            .MustAsync(async (user, email, _) => !await userService.ExistsWithEmailAsync(email, user.Id))
                .WithMessage((_, email) => string.Format(localizer["Email {0} is already registered."], email));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            .MustAsync(async (user, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!, user.Id))
                .WithMessage((_, phone) => string.Format(localizer["Phone number {0} is already registered."], phone))
                .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));
    }
}