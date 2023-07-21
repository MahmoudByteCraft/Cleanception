using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.NotificationLogs;

public class GetNotificationLogRequest : IQuery<Result<NotificationLogDto>>
{
    public Guid Id { get; set; }

    public GetNotificationLogRequest(Guid id) => Id = id;
}

public class NotificationLogByIdSpec : Specification<NotificationLog, NotificationLogDto>, ISingleResultSpecification
{
    public NotificationLogByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(NotificationLogByIdSpec), id);
}

public class GetNotificationLogRequestHandler : IQueryHandler<GetNotificationLogRequest, Result<NotificationLogDto>>
{
    private readonly IRepository<NotificationLog> _repository;
    private readonly IStringLocalizer<GetNotificationLogRequestHandler> _localizer;

    public GetNotificationLogRequestHandler(IRepository<NotificationLog> repository, IStringLocalizer<GetNotificationLogRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<NotificationLogDto>> Handle(GetNotificationLogRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new NotificationLogByIdSpec(request.Id), cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification Not Found.", request.Id]);

        return await Result<NotificationLogDto>.SuccessAsync(entity);
    }
}