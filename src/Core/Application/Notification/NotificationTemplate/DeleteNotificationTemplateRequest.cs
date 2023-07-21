using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;

namespace Cleanception.Application.Notification.NotificationTemplate;

public class DeleteNotificationTemplateRequest : ICommand
{
    public Guid Id { get; set; }

    public DeleteNotificationTemplateRequest(Guid id) => Id = id;
}

public class DeleteNotificationTemplateRequestHandler : ICommandHandler<DeleteNotificationTemplateRequest>
{
    private readonly IRepositoryWithEvents<Domain.Notification.NotificationTemplate> _NotificationTemplateRepo;
    private readonly IStringLocalizer<DeleteNotificationTemplateRequestHandler> _localizer;

    public DeleteNotificationTemplateRequestHandler(IRepositoryWithEvents<Domain.Notification.NotificationTemplate> NotificationTemplateRepo, IStringLocalizer<DeleteNotificationTemplateRequestHandler> localizer) =>
        (_NotificationTemplateRepo, _localizer) = (NotificationTemplateRepo, localizer);

    public async Task<Result<Guid>> Handle(DeleteNotificationTemplateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _NotificationTemplateRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Notification template not found."]);

        await _NotificationTemplateRepo.DeleteAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}