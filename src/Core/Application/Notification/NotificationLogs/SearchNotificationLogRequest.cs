using System.Text.Json.Serialization;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Notification;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.NotificationLogs;

public class SearchNotificationLogRequest : PaginationFilter, IQuery<PaginatedResult<NotificationLogDto>>
{
    public NotificationType? NotificaitionType { get; set; }
    public bool? Viewed { get; set; }

    [JsonIgnore]
    public Guid? UserId { get; set; }

    [JsonIgnore]
    public bool IncludeUser { get; set; }
}

public class NotificationsBySearchRequestSpec : EntitiesByPaginationFilterSpec<NotificationLog, NotificationLogDto>
{
    public NotificationsBySearchRequestSpec(SearchNotificationLogRequest request)
        : base(request) =>
        Query
        .Where(x => x.UserId == request.UserId.ToString(), request.UserId.HasValue)
        .Where(x => x.NotificaitionType == request.NotificaitionType, request.NotificaitionType.HasValue)
        .Where(x => x.Viewed == request.Viewed, request.Viewed.HasValue)
        .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
}

public class SearchNotificationsRequestHandler : IQueryHandler<SearchNotificationLogRequest, PaginatedResult<NotificationLogDto>>
{
    private readonly IReadRepository<NotificationLog> _repository;
    private readonly IUserService _userService;

    public SearchNotificationsRequestHandler(IUserService userService, IReadRepository<NotificationLog> repository)
        => (_repository, _userService) = (repository, userService);

    public async Task<PaginatedResult<NotificationLogDto>> Handle(SearchNotificationLogRequest request, CancellationToken cancellationToken)
    {
        var spec = new NotificationsBySearchRequestSpec(request);

        var result = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);

        if(request.IncludeUser)
        {
            foreach (var item in result.Data)
            {
                item.User = await _userService.GetSimplifyAsync(item.UserId!, cancellationToken);
            }
        }

        return result;
    }
}