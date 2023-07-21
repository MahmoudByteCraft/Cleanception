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

public class CreateNotificationTemplateRequest : ICommand
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public NotificationMethod NotificationMethod { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }
    public bool IsSelfTrigger { get; set; }
    public string? RoleId { get; set; }
    public FileUploadRequest? File { get; set; }
    public bool IsActive { get; set; }
    public bool ScheduleTemplate { get; set; }
}

public class CreateNotificationTemplateRequestValidator : AbstractValidator<CreateNotificationTemplateRequest>
{
    public CreateNotificationTemplateRequestValidator(IRoleService roleService, IStringLocalizer<CreateNotificationTemplateRequestValidator> localizer)
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

public class CreateNotificationTemplateRequestHandler : ICommandHandler<CreateNotificationTemplateRequest>
{

    private readonly IRepository<Domain.Notification.NotificationTemplate> _repository;
    private readonly IStringLocalizer<CreateNotificationTemplateRequestHandler> _localizer;
    private readonly IFileStorageService _fileStorageService;

    public CreateNotificationTemplateRequestHandler(IFileStorageService fileStorageService, IRepository<Domain.Notification.NotificationTemplate> repository, IStringLocalizer<CreateNotificationTemplateRequestHandler> localizer)
        => (_fileStorageService, _repository, _localizer) = (fileStorageService, repository, localizer);

    public async Task<Result<Guid>> Handle(CreateNotificationTemplateRequest request, CancellationToken cancellationToken)
    {
        var sameNotification = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<Domain.Notification.NotificationTemplate>(x => x.RoleId == request.RoleId && x.NotificationMethod == request.NotificationMethod && x.NotificationTrigger == request.NotificationTrigger && x.ScheduleTemplate == request.ScheduleTemplate && x.IsSelfTrigger == request.IsSelfTrigger));
        if (sameNotification != null)
            throw new BadRequestException(_localizer["There is already template with the same (role / method / target) "]);

        var entity = new Domain.Notification.NotificationTemplate
        {
            Title = request.Title,
            Message = request.Message,
            NotificationMethod = request.NotificationMethod,
            NotificationTrigger = request.NotificationTrigger,
            RoleId = request.RoleId,
            IsActive = request.IsActive,
            ScheduleTemplate = request.ScheduleTemplate,
            IsSelfTrigger = request.IsSelfTrigger,
            FileUrl = request.File != null ? await _fileStorageService.UploadAsync<Domain.Notification.NotificationTemplate>(request.File, FileType.Image, cancellationToken) : null,
        };

        await _repository.AddAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }
}