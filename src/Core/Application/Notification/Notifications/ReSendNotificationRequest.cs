using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.Notifications;

public class ReSendNotificationRequest : ICommand<Result<bool>>
{
    public DefaultIdType Id { get; set; }
}

public class ReSendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{

}

public class ReSendNotificationRequestHandler : ICommandHandler<ReSendNotificationRequest, Result<bool>>
{
    private readonly IJobService _jobService;
    private readonly IReadRepository<Domain.Notification.Notification> _NotificationRepo;
    private readonly IStringLocalizer<ReSendNotificationRequestHandler> _localizer;

    public ReSendNotificationRequestHandler(
        IReadRepository<Domain.Notification.Notification> NotificationRepo,
        IJobService jobService,
        IStringLocalizer<ReSendNotificationRequestHandler> localizer)
    {
        _jobService = jobService;
        _NotificationRepo = NotificationRepo;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(ReSendNotificationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _NotificationRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification Not Found."]);

        _jobService.Enqueue<INotificationService>(x => x.SendGeneralNotification(entity));

        return await Result<bool>.SuccessAsync(true);
    }
}