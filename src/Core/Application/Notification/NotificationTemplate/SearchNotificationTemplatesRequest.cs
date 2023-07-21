using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Roles;
using Cleanception.Shared.Notifications;
using Mapster;

namespace Cleanception.Application.Notification.NotificationTemplate;

public class SearchNotificationTemplatesRequest : PaginationFilter, IQuery<PaginatedResult<NotificationTemplateDto>>
{
    public string? RoleId { get; set; }
    public NotificationMethod? NotificationMethod { get; set; }
    public NotificationTrigger? NotificationTrigger { get; set; }
    public bool? IsActive { get; set; }
}

public class NotificationTemplatesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Notification.NotificationTemplate, NotificationTemplateDto>
{
    public NotificationTemplatesBySearchRequestSpec(SearchNotificationTemplatesRequest request)
        : base(request) =>
        Query
        .Where(x => x.NotificationMethod == request.NotificationMethod, request.NotificationMethod.HasValue)
        .Where(x => x.NotificationTrigger == request.NotificationTrigger, request.NotificationTrigger.HasValue)
        .Where(x => x.RoleId == request.RoleId, request.RoleId.HasValue())
        .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
        .OrderBy(c => c.CreatedOn, !request.HasOrderBy());
}

public class SearchNotificationTemplatesRequestHandler : IQueryHandler<SearchNotificationTemplatesRequest, PaginatedResult<NotificationTemplateDto>>
{
    private readonly IReadRepository<Domain.Notification.NotificationTemplate> _repository;
    private readonly IRoleService _roleService;

    public SearchNotificationTemplatesRequestHandler(IRoleService roleService, IReadRepository<Domain.Notification.NotificationTemplate> repository)
        => (_repository, _roleService) = (repository, roleService);

    public async Task<PaginatedResult<NotificationTemplateDto>> Handle(SearchNotificationTemplatesRequest request, CancellationToken cancellationToken)
    {
        var spec = new NotificationTemplatesBySearchRequestSpec(request);

        var result = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);

        foreach (var item in result.Data)
        {
            if (item.RoleId.HasValue())
            {
                var currentRole = await _roleService.GetByIdAsync(item.RoleId!);
                if (result != null)
                {
                    item.Role = currentRole.Adapt<RoleSimplifyDto>();
                }
            }
        }

        return result;
    }
}