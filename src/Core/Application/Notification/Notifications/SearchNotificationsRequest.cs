using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Notification;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.Notifications;

public class SearchNotificationsRequest : PaginationFilter, IQuery<PaginatedResult<NotificationDto>>
{
    public NotificationType? NotificaitionType { get; set; }
    public NotificationMethod? NotificationMethod { get; set; }
    public NotificationTarget? NotificationTarget { get; set; }
}

public class NotificationsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Notification.Notification, NotificationDto>
{
    public NotificationsBySearchRequestSpec(SearchNotificationsRequest request)
        : base(request) =>
        Query
        .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
        .Where(x => x.NotificationType == request.NotificaitionType, request.NotificaitionType.HasValue)
        .Where(x => !string.IsNullOrEmpty(x.NotificationMethodsJson) && x.NotificationMethodsJson.Contains(nameof(request.NotificationMethod)), request.NotificationMethod.HasValue)
        .Where(x => x.NotificationTarget == request.NotificationTarget, request.NotificationTarget.HasValue);
}

public class SearchNotificationsRequestHandler : IQueryHandler<SearchNotificationsRequest, PaginatedResult<NotificationDto>>
{
    private readonly IReadRepository<Domain.Notification.Notification> _repository;
    private readonly IUserService _userService;

    public SearchNotificationsRequestHandler(IUserService userService, IReadRepository<Domain.Notification.Notification> repository)
        => (_repository, _userService) = (repository, userService);

    public async Task<PaginatedResult<NotificationDto>> Handle(SearchNotificationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new NotificationsBySearchRequestSpec(request);

        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}