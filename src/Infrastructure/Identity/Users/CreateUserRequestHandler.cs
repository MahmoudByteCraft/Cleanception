using System.Text;
using Cleanception.Application.Common.Events;
using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Mailing;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Identity;
using Cleanception.Infrastructure.Auth;
using Cleanception.Infrastructure.Common;
using Cleanception.Shared.Authorization;
using Cleanception.Shared.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Cleanception.Infrastructure.Identity.Users;

public class CreateUserRequestHandler : ICommandHandler<CreateUserRequest, string>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<CreateUserRequestHandler> _localizer;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly SecuritySettings _securitySettings;
    private readonly IEmailTemplateService _templateService;
    private readonly IEventPublisher _events;
    private readonly INotificationService _notificationService;

    public CreateUserRequestHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<CreateUserRequestHandler> localizer,
        IJobService jobService,
        IMailService mailService,
        IOptions<SecuritySettings> securitySettings,
        IEventPublisher events,
        IEmailTemplateService templateService,
        INotificationService notificationService)
    {
        _userManager = userManager;
        _localizer = localizer;
        _jobService = jobService;
        _mailService = mailService;
        _securitySettings = securitySettings.Value;
        _templateService = templateService;
        _events = events;
        _notificationService = notificationService;
    }

    public async Task<string> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            FullName = $"{request.FirstName} {request.MiddleName} {request.LastName}",
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InternalServerException(_localizer["Validation Errors Occurred."], result.GetErrors(_localizer));
        }

        await _userManager.AddToRoleAsync(user, UPCRoles.Customer);

        var messages = new List<string> { string.Format(_localizer["User {0} Registered."], user.UserName) };

        _jobService.Enqueue(() => _notificationService.SendByTrigger(
               NotificationTrigger.NewUser,
               new SendingByTriggerOptions
               {
                   EntityId = Guid.Parse(user.Id),
                   ScheduleTemplate = false,
                   TemplatesToUse = SendingByTriggerTemplates.SelfTemplates,
                   SelfUserId = user.Id
               }));

        await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

        return string.Join(Environment.NewLine, messages);
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "api/users/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryStringKeys.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryStringKeys.Code, code);
        return verificationUri;
    }

}
