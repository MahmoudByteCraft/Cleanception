using Cleanception.Application.Common.FileStorage;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Identity.Roles;
using Cleanception.Domain.Common;
using Cleanception.Domain.Notification;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.Notifications;

public class SendNotificationRequest : ICommand<Result<bool>>
{
    public NotificationTarget NotificationTarget { get; set; }
    public NotificationType NotificationType { get; set; }
    public List<NotificationMethod> NotificationMethods { get; set; } = default!;
    public DefaultIdType? EntityId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public FileUploadRequest? File { get; set; }
    public DateTime? NotifyDate { get; set; }
    public List<string>? UserIds { get; set; }
    public List<DefaultIdType>? CustomerTypeIds { get; set; }
    public List<string>? PriceCodes { get; set; }
    public List<DefaultIdType>? BranchIds { get; set; }
    public List<DefaultIdType>? SalePersonIds { get; set; }
    public List<string>? RoleIds { get; set; }
}

public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{
    public SendNotificationRequestValidator()
    {
        RuleFor(p => p.Title)
           .NotEmpty()
           .MaximumLength(255);

        RuleFor(p => p.Message)
           .NotEmpty()
           .MaximumLength(255);

        RuleFor(p => p.NotificationMethods)
          .NotNull()
          .NotEmpty()
          .Must(x => x.Count > 0);

        When(x => x.NotifyDate.HasValue, () =>
        {
            RuleFor(p => p.NotifyDate)
               .GreaterThan(DateTime.UtcNow);
        });
    }
}

public class SendNotificationRequestHandler : ICommandHandler<SendNotificationRequest, Result<bool>>
{
    private readonly IJobService _jobService;
    private readonly IRoleService _roleService;
    private readonly IRepository<Domain.Notification.Notification> _NotificationRepo;
    private readonly IFileStorageService _fileStorageService;

    public SendNotificationRequestHandler(IJobService jobService, IRoleService roleService, IRepository<Domain.Notification.Notification> NotificationRepo, IFileStorageService fileStorageService)
    {
        _jobService = jobService;
        _roleService = roleService;
        _NotificationRepo = NotificationRepo;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<bool>> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
    {
        var Notification = await FillNotification(request, cancellationToken);

        string? jobId = string.Empty;

        if (!request.NotifyDate.HasValue)
        {
            jobId = _jobService.Enqueue<INotificationService>(x => x.SendGeneralNotification(Notification));
        }
        else
        {
            var test = new DateTimeOffset(request.NotifyDate.Value);

            jobId = _jobService.Schedule<INotificationService>(x => x.SendGeneralNotification(Notification), new DateTimeOffset(request.NotifyDate.Value));
        }

        Notification.JobId = jobId;

        await _NotificationRepo.AddAsync(Notification, cancellationToken);

        return await Result<bool>.SuccessAsync(true);
    }

    private async Task<Domain.Notification.Notification> FillNotification(SendNotificationRequest request, CancellationToken cancellationToken)
    {
        var notificationParameters = new NotificationParameters();

        notificationParameters.UserIds = request.UserIds;

        notificationParameters.RoleIds = request.RoleIds;

        var temp = new Domain.Notification.Notification
        {
            Message = request.Message,
            Title = request.Title,
            NotifyDate = request.NotifyDate,
            NotificationType = request.NotificationType,
            EntityId = request.EntityId,
            NotificationMethods = request.NotificationMethods,
            NotificationTarget = request.NotificationTarget,
            FileUrl = request.File != null ? await _fileStorageService.UploadAsync<Domain.Notification.Notification>(request.File, FileType.Image, cancellationToken) : null,
            NotificationParameters = notificationParameters
        };

        var roleNames = new List<string>();

        if (temp.NotificationParameters.RoleIds?.Count > 0)
        {
            foreach (string roleId in temp.NotificationParameters.RoleIds)
            {
                var role = await _roleService.GetByIdAsync(roleId);
                if (role != null)
                    roleNames.Add(role.Name ?? "Unknown");
            }

            temp.RolesNames = string.Join(",", roleNames);
        }

        return temp;
    }
}