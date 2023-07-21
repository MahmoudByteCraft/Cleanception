using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.FileStorage;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Identity.Roles;
using Cleanception.Domain.Common;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.NotificationTemplate;

public class UpdateNotificationTemplateRequest : ICommand
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public NotificationMethod NotificationMethod { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }

    public bool IsSelfTrigger { get; set; }
    public string? RoleId { get; set; }
    public string? FileUrl { get; set; }
    public FileUploadRequest? File { get; set; }
    public bool IsActive { get; set; }
    public bool ScheduleTemplate { get; set; }
}

public class UpdateNotificationTemplateRequestValidator : AbstractValidator<UpdateNotificationTemplateRequest>
{
    public UpdateNotificationTemplateRequestValidator(
        IRoleService roleService,
        IStringLocalizer<UpdateNotificationTemplateRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
           .NotEmpty();

        RuleFor(p => p.Message)
           .NotEmpty();

        When(x => x.IsSelfTrigger, () =>
        {
            RuleFor(p => p.RoleId)
                .Cascade(CascadeMode.Stop)
                .Null();
        })
        .Otherwise(() =>
        {
            RuleFor(p => p.RoleId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (field, ct) => await roleService.GetByIdAsync(field!) != null)
                       .WithMessage((_, field) => localizer["Role not found.", field!]);
        });
    }
}

public class UpdateNotificationTemplateRequestHandler : ICommandHandler<UpdateNotificationTemplateRequest>
{

    private readonly IRepositoryWithEvents<Domain.Notification.NotificationTemplate> _repository;
    private readonly IStringLocalizer<UpdateNotificationTemplateRequestHandler> _localizer;
    private readonly IFileStorageService _fileStorageService;

    public UpdateNotificationTemplateRequestHandler(IFileStorageService fileStorageService, IRepositoryWithEvents<Domain.Notification.NotificationTemplate> repository, IStringLocalizer<UpdateNotificationTemplateRequestHandler> localizer) =>
        (_fileStorageService, _repository, _localizer) = (fileStorageService, repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateNotificationTemplateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["NotificationTemplate Not Found.", request.Id]);

        var sameNotification = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<Domain.Notification.NotificationTemplate>(x => x.RoleId == request.RoleId && x.NotificationMethod == request.NotificationMethod && x.NotificationTrigger == request.NotificationTrigger && x.ScheduleTemplate == request.ScheduleTemplate && x.IsSelfTrigger == request.IsSelfTrigger && x.Id != request.Id));
        if (sameNotification != null)
            throw new BadRequestException(_localizer["There is already template with the same (role / method / target) "]);

        entity.Title = request.Title;
        entity.Message = request.Message;
        entity.NotificationMethod = request.NotificationMethod;
        entity.NotificationTrigger = request.NotificationTrigger;
        entity.RoleId = request.RoleId;
        entity.IsActive = request.IsActive;
        entity.ScheduleTemplate = request.ScheduleTemplate;
        entity.IsSelfTrigger = request.IsSelfTrigger;
        entity.FileUrl = request.File != null
                                ? await _fileStorageService.UploadAsync<Domain.Notification.NotificationTemplate>(request.File, FileType.Image, cancellationToken)
                                : ((request.FileUrl != entity.FileUrl) ? null : entity.FileUrl);

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}