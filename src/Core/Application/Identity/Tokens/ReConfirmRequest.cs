using Cleanception.Application.Common.Messaging;

namespace Cleanception.Application.Identity.Tokens;

public class ReConfirmRequest : ICommand<ConfirmationCodeDto>
{
    public string Email { get; set; } = default!;
}
