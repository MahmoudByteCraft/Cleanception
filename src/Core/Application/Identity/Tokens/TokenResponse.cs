namespace Cleanception.Application.Identity.Tokens;

// public record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime, AppCustomerDetailsDto? AppCustomer);

public class TokenResponse
{
    public TokenResponse(string token, string refreshToken, DateTime refreshTokenExpiryTime)
    {
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
        Roles = new List<string>();
    }

    public string? UserId { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public List<string>? Roles { get; set; }

}