namespace Cleanception.Application.Identity.Tokens;

public record ConfirmRequest(string Email, string Code);

public class ConfirmationCodeDto
{
    public string? ConfirmationCode { get; set; }
}