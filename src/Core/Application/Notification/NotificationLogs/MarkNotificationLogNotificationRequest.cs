using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.NotificationLogs;

public class MarkNotificationLogNotificationRequest : ICommand<Result<bool>>
{
    public Guid Id { get; set; }
    public MarkNotificationLogNotificationRequest(Guid id)
    {
        Id = id;
    }
}

public class MarkAsViewedNotificationRequestValidator : AbstractValidator<MarkNotificationLogNotificationRequest>
{
    public MarkAsViewedNotificationRequestValidator()
    {

    }
}

public class MarkAsViewedNotificationRequestHandler : ICommandHandler<MarkNotificationLogNotificationRequest, Result<bool>>
{


    private readonly IJobService _jobService;
    private readonly IRepository<NotificationLog> _notificationLogRepo;
    private readonly IStringLocalizer<GetNotificationLogRequestHandler> _localizer;

    public MarkAsViewedNotificationRequestHandler(IRepository<NotificationLog> notificationLogRepo, IJobService jobService, IStringLocalizer<GetNotificationLogRequestHandler> localizer)
    {
        _jobService = jobService;
        _localizer = localizer;
        _notificationLogRepo = notificationLogRepo;
    }

    public async Task<Result<bool>> Handle(MarkNotificationLogNotificationRequest logNotificationRequest, CancellationToken cancellationToken)
    {
        var entity = await _notificationLogRepo.GetByIdAsync(logNotificationRequest.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification Not Found.", logNotificationRequest.Id]);

        entity.Viewed = true;

        await _notificationLogRepo.UpdateAsync(entity, cancellationToken);

        return await Result<bool>.SuccessAsync(true);
    }
}