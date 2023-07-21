using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Identity.Tokens;
using Cleanception.Application.Identity.Users.Password;

namespace Cleanception.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<Result<ConfirmationCodeDto>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await _userManager.IsPhoneNumberConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new InternalServerException(_localizer["An Error has occurred!"]);
        }

        ConfirmationCodeDto model = new ConfirmationCodeDto();
        string? code = code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

        return Result<ConfirmationCodeDto>.Success(model);
    }

    public async Task<Result<bool>> ConfirmForgotPasswordAsync(ConfirmForgetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.Email.Trim().Normalize());
            if (user == null)
            {
                throw new UnauthorizedException(_localizer["Authentication Failed."]);
            }
        }

        if (await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Code))
        {
            await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), request.NewPassword);
        }
        else
        {
            throw new BadRequestException(_localizer["Code is invalid"]);
        }

        return await Result<bool>.SuccessAsync("Password Reset Successful!");
    }

    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id);

        _ = user ?? throw new InternalServerException(_localizer["An Error has occurred!"]);

        await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), request.Password);

        return await Result<string>.SuccessAsync("Password Reset Successful!");
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Change password failed"], result.GetErrors(_localizer));
        }
    }
}