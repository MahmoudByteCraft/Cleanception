using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.Notifications;

public class CancelNotificationRequest : ICommand
{
    public DefaultIdType Id { get; set; }

    public CancelNotificationRequest(DefaultIdType id) => Id = id;
}

public class CancelNotificationRequestHandler : ICommandHandler<CancelNotificationRequest>
{
    private readonly IRepositoryWithEvents<Domain.Notification.Notification> _notificationRepo;
    private readonly IStringLocalizer<CancelNotificationRequestHandler> _localizer;
    private readonly IJobService _jobService;

    public CancelNotificationRequestHandler(IJobService jobService, IRepositoryWithEvents<Domain.Notification.Notification> notificationRepo, IStringLocalizer<CancelNotificationRequestHandler> localizer) =>
        (_jobService, _notificationRepo, _localizer) = (jobService, notificationRepo, localizer);

    public async Task<Result<DefaultIdType>> Handle(CancelNotificationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _notificationRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification Not Found."]);

        if (!entity.NotifyDate.HasValue)
        {
            throw new BadRequestException(_localizer["Notification cannot be canceled becuase it is not a schedule notification."]);
        }

        if (entity.CanceledNotification)
        {
            throw new BadRequestException(_localizer["Notification is already canceled"]);
        }

        _jobService.Delete(entity.JobId!);

        entity.CanceledNotification = true;

        await _notificationRepo.UpdateAsync(entity, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(request.Id);
    }
}