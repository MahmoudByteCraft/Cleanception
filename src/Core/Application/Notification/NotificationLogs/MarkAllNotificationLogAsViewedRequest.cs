using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.NotificationLogs;

public class MarkAllNotificationLogAsViewedRequest : ICommand<Result<bool>>
{
}

public class MarkAllNotificationAsViewedRequestValidator : AbstractValidator<MarkAllNotificationLogAsViewedRequest>
{
    public MarkAllNotificationAsViewedRequestValidator()
    {

    }
}

public class MarkAllNotificationAsViewedRequestHandler : ICommandHandler<MarkAllNotificationLogAsViewedRequest, Result<bool>>
{

    private readonly IJobService _jobService;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<NotificationLog> _notificationLogRepo;
    private readonly IStringLocalizer<GetNotificationLogRequestHandler> _localizer;

    public MarkAllNotificationAsViewedRequestHandler(ICurrentUser currentUser, IRepository<NotificationLog> notificationLogRepo, IJobService jobService, IStringLocalizer<GetNotificationLogRequestHandler> localizer)
    {
        _jobService = jobService;
        _localizer = localizer;
        _notificationLogRepo = notificationLogRepo;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(MarkAllNotificationLogAsViewedRequest request, CancellationToken cancellationToken)
    {
        var entites = await _notificationLogRepo.ListAsync(new ExpressionSpecification<NotificationLog>(x => x.UserId == _currentUser.GetUserId().ToString() && !x.Viewed), cancellationToken);

        entites.ForEach(x =>
        {
            x.Viewed = true;
        });

        await _notificationLogRepo.UpdateRangeAsync(entites, cancellationToken);

        return await Result<bool>.SuccessAsync(true);
    }
}