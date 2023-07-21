using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.Notifications;

public class DeleteNotificationRequest : ICommand
{
    public DefaultIdType Id { get; set; }

    public DeleteNotificationRequest(DefaultIdType id) => Id = id;
}

public class DeleteNotificationRequestHandler : ICommandHandler<DeleteNotificationRequest>
{
    private readonly IRepositoryWithEvents<Domain.Notification.Notification> _notificationRepo;
    private readonly IStringLocalizer<DeleteNotificationRequestHandler> _localizer;
    private readonly IJobService _jobService;

    public DeleteNotificationRequestHandler(IJobService jobService, IRepositoryWithEvents<Domain.Notification.Notification> notificationRepo, IStringLocalizer<DeleteNotificationRequestHandler> localizer) =>
        (_jobService, _notificationRepo, _localizer) = (jobService, notificationRepo, localizer);

    public async Task<Result<DefaultIdType>> Handle(DeleteNotificationRequest request, CancellationToken DeletelationToken)
    {
        var entity = await _notificationRepo.GetByIdAsync(request.Id, DeletelationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification not found."]);

        if (entity.NotifyDate.HasValue && !entity.CanceledNotification)
        {
            _jobService.Delete(entity.JobId!);
        }

        await _notificationRepo.DeleteAsync(entity, DeletelationToken);

        return await Result<DefaultIdType>.SuccessAsync(request.Id);
    }
}