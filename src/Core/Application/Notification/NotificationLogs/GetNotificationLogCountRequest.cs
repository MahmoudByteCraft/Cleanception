using System.Text.Json.Serialization;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Domain.Notification;

namespace Cleanception.Application.Notification.NotificationLogs;

public class GetNotificationLogCountRequest : IQuery<Result<int>>
{
    [JsonIgnore]
    public string? UserId { get; set; }
    public bool? Viewed { get; set; }

    public GetNotificationLogCountRequest(string? userId, bool? viewed) => (UserId, Viewed) = (userId, viewed);

    public GetNotificationLogCountRequest()
    {

    }
}

public class GetNotificationLogCountRequestHandler : IQueryHandler<GetNotificationLogCountRequest, Result<int>>
{
    private readonly IRepository<NotificationLog> _repository;
    private readonly IStringLocalizer<GetNotificationLogCountRequestHandler> _localizer;

    public GetNotificationLogCountRequestHandler(IRepository<NotificationLog> repository, IStringLocalizer<GetNotificationLogCountRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<int>> Handle(GetNotificationLogCountRequest request, CancellationToken cancellationToken)
    {
        if(request.Viewed.HasValue)
        {
            return await Result<int>.SuccessAsync(await _repository.CountAsync(new ExpressionSpecification<NotificationLog>(x => x.UserId == request.UserId && x.Viewed == request.Viewed), cancellationToken));
        }

        return await Result<int>.SuccessAsync(await _repository.CountAsync(new ExpressionSpecification<NotificationLog>(x => x.UserId == request.UserId), cancellationToken));
    }
}