using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Identity.Roles;
using Mapster;

namespace Cleanception.Application.Notification.NotificationTemplate;

public class GetNotificationTemplateRequest : IQuery<Result<NotificationTemplateDto>>
{
    public Guid Id { get; set; }

    public GetNotificationTemplateRequest(Guid id) => Id = id;
}

public class NotificationTemplateByIdSpec : Specification<Domain.Notification.NotificationTemplate, NotificationTemplateDto>, ISingleResultSpecification
{
    public NotificationTemplateByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(NotificationTemplateByIdSpec), id);
}

public class GetNotificationTemplateRequestHandler : IQueryHandler<GetNotificationTemplateRequest, Result<NotificationTemplateDto>>
{
    private readonly IRepository<Domain.Notification.NotificationTemplate> _repository;
    private readonly IRoleService _roleService;
    private readonly IStringLocalizer<GetNotificationTemplateRequestHandler> _localizer;

    public GetNotificationTemplateRequestHandler(IRoleService roleService, IRepository<Domain.Notification.NotificationTemplate> repository, IStringLocalizer<GetNotificationTemplateRequestHandler> localizer)
        => (_roleService, _repository, _localizer) = (roleService, repository, localizer);

    public async Task<Result<NotificationTemplateDto>> Handle(GetNotificationTemplateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new NotificationTemplateByIdSpec(request.Id), cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification template not found.", request.Id]);

        if(entity.RoleId.HasValue())
        {
            var result = await _roleService.GetByIdAsync(entity.RoleId!);
            if(result != null)
            {
                entity.Role = result.Adapt<RoleSimplifyDto>();
            }
        }

        return await Result<NotificationTemplateDto>.SuccessAsync(entity);
    }
}