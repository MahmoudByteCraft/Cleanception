using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Roles;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.NotificationTemplate;

public class NotificationTemplateSimplifyDto : IDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public NotificationMethod NotificationMethod { get; set; }

    public NotificationTrigger NotificationTrigger { get; set; }

    public string? RoleId { get; set; }

    public string? FileUrl { get; set; }

    public bool IsActive { get; set; }
    public bool ScheduleTemplate { get; set; }
    public bool IsSelfTrigger { get; set; }

    public DateTime CreatedOn { get; set; }
}

public class NotificationTemplateRawDto : NotificationTemplateSimplifyDto
{

}

public class NotificationTemplateDto : NotificationTemplateRawDto
{
    public RoleSimplifyDto? Role { get; set; }
}

public class NotificationTemplateDetailsDto : IDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public NotificationMethod NotificationMethod { get; set; }

    public NotificationTrigger NotificationTrigger { get; set; }

    // public bool IsSelfTrigger { get; set; }

    public string? RoleId { get; set; }

    public RoleSimplifyDto? Role { get; set; }

    public string? FileUrl { get; set; }

    public bool IsActive { get; set; }
    public bool ScheduleTemplate { get; set; }
    public bool IsSelfTrigger { get; set; }

    public DefaultIdType CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }

}