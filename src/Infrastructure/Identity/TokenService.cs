using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Tokens;
using Cleanception.Infrastructure.Auth.Jwt;
using Cleanception.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cleanception.Infrastructure.Identity;

internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<TokenService> _localizer;
    private readonly JwtSettings _jwtSettings;
    private readonly IJobService _jobService;
    private readonly INotificationService _notificationService;

    public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager, IStringLocalizer<TokenService> localizer, IJobService jobService, INotificationService notificationService)
    {
        _userManager = userManager;
        _localizer = localizer;
        _jwtSettings = jwtSettings.Value;
        _jobService = jobService;
        _notificationService = notificationService;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        user ??= await _userManager.FindByNameAsync(request.Email.Trim().Normalize());

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException(_localizer["Authentication Failed."]);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(_localizer["User Not Active. Please contact the administrator."]);
        }

        var userRoles = (await _userManager.GetRolesAsync(user))?.ToList();

        var resultToReturn = await GenerateTokensAndUpdateUser(user, ipAddress, userRoles);

        resultToReturn.Roles = userRoles;

        resultToReturn.UserId = user.Id;

        user.FcmToken = request.FcmToken;

        return resultToReturn;
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            throw new UnauthorizedException(_localizer["Authentication Failed."]);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException(_localizer["Invalid Refresh Token."]);
        }

        var userRoles = (await _userManager.GetRolesAsync(user))?.ToList();

        var resultToReturn = await GenerateTokensAndUpdateUser(user, ipAddress, userRoles);

        resultToReturn.Roles = userRoles;

        resultToReturn.UserId = user.Id;

        return resultToReturn;
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress, List<string>? roles)
    {
        string token = GenerateJwt(user, ipAddress, roles);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, string ipAddress, List<string>? roles) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress, roles));

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress, List<string>? roles) =>
        new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),

            // new(ClaimTypes.Role, customParams.Roles != null ? string.Join(",", customParams.Roles) : string.Empty),

            new(UPCClaims.IpAddress, ipAddress),
            new(UPCClaims.ImageUrl, user.ImageUrl ?? string.Empty),
            new(UPCClaims.Fullname, user.FullName ?? string.Empty),
        }.Concat(roles?.Select(role => new Claim(UPCClaims.Role, role ?? string.Empty)) ?? new List<Claim>());

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           //expires: DateTime.UtcNow.AddSeconds(1),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(_localizer["Invalid Token."]);
        }

        return principal;
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(_localizer["Invalid Token."]);
        }

        var res = principal.FindFirstValue("exp");

        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        dateTime = dateTime.AddSeconds(double.Parse(res)).ToUniversalTime();

        if (DateTime.UtcNow > dateTime)
        {
            throw new UnauthorizedException(_localizer["Invalid Token."]);
        }

        return true;
    }

    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}