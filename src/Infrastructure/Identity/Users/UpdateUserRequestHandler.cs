using Cleanception.Application.Common.Caching;
using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.FileStorage;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Common;
using Cleanception.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Cleanception.Infrastructure.Identity.Users;

public class UpdateUserRequestHandler : ICommandHandler<UpdateUserRequest, string>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UpdateUserRequestHandler> _localizer;
    private readonly IFileStorageService _fileStorage;
    private readonly IUserService _userService;
    private readonly ICacheService _cache;

    public UpdateUserRequestHandler(UserManager<ApplicationUser> userManager, IStringLocalizer<UpdateUserRequestHandler> localizer, IFileStorageService fileStorage, IUserService userService, ICacheService cache)
    {
        _userManager = userManager;
        _localizer = localizer;
        _fileStorage = fileStorage;
        _userService = userService;
        _cache = cache;
    }

    public async Task<string> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        string currentImage = user.ImageUrl ?? string.Empty;
        if (request.Image != null || request.DeleteCurrentImage)
        {
            user.ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image, cancellationToken);
            if (request.DeleteCurrentImage && !string.IsNullOrEmpty(currentImage))
            {
                string root = Directory.GetCurrentDirectory();
                _fileStorage.Remove(Path.Combine(root, currentImage));
            }
        }

        if(await _userService.ExistsWithPhoneNumberAsync(user.PhoneNumber, user.Id))
        {
            throw new BadRequestException("Already exist with the same mobile number");
        }

        user.FullName = request.FirstName;

        if(!string.IsNullOrWhiteSpace(request.MiddleName))
            user.FullName += " " + request.MiddleName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.FullName += " " + request.LastName;

        user.PhoneNumber = request.PhoneNumber;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);

        await _cache.RemoveAsync($"{CacheConstants.User}-{user.Id}", cancellationToken);

        return user.Id;
    }
}
