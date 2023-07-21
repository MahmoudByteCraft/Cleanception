using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Shared.Notifications;

namespace Cleanception.Domain.Notification;

[SupportDeepSearch]
public class NotificationTemplate : AuditableEntity, IAggregateRoot
{
    [ColumnSupportDeepSearch]
    public string? Title { get; set; }

    [ColumnSupportDeepSearch]
    public string? Message { get; set; }

    public NotificationMethod NotificationMethod { get; set; }

    public NotificationTrigger NotificationTrigger { get; set; }

    public bool IsSelfTrigger { get; set; }

    public string? RoleId { get; set; }

    public string? FileUrl { get; set; }

    public bool IsActive { get; set; }
    public bool ScheduleTemplate { get; set; }
}