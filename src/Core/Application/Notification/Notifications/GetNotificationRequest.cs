using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.Notifications;

public class GetNotificationRequest : IQuery<Result<NotificationDetailsDto>>
{
    public DefaultIdType Id { get; set; }

    public GetNotificationRequest(DefaultIdType id) => Id = id;
}

public class NotificationByIdSpec : Specification<Domain.Notification.Notification, NotificationDetailsDto>, ISingleResultSpecification
{
    public NotificationByIdSpec(DefaultIdType id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(NotificationByIdSpec), id);
}

public class GetNotificationRequestHandler : IQueryHandler<GetNotificationRequest, Result<NotificationDetailsDto>>
{
    private readonly IRepository<Domain.Notification.Notification> _repository;
    private readonly IStringLocalizer<GetNotificationRequestHandler> _localizer;

    public GetNotificationRequestHandler(IRepository<Domain.Notification.Notification> repository, IStringLocalizer<GetNotificationRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<NotificationDetailsDto>> Handle(GetNotificationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new NotificationByIdSpec(request.Id), cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification not Found.", request.Id]);

        return await Result<NotificationDetailsDto>.SuccessAsync(entity);
    }
}