using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Users;
using Cleanception.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Http;

namespace Cleanception.Infrastructure.Middleware;

public class UserValidatorMiddleware : IMiddleware
{
    public ICurrentUser _currentUser { get; set; }
    public IUserService _userService { get; set; }
    public ApplicationDbContext _dbContext { get; set; }

    public UserValidatorMiddleware(ICurrentUser currentUser, IUserService userService, ApplicationDbContext dbContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (_currentUser.IsAuthenticated())
        {
            var user = await _userService.GetCachedUserAsync(_currentUser.GetUserId()!.ToString(), default);

            if (user == null)
            {
                throw new UnauthorizedException("User not found");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("Account is not active");
            }
        }

        await next(httpContext);
    }
}
