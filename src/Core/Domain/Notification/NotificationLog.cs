using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Shared.Notifications;

namespace Cleanception.Domain.Notification;

[SupportDeepSearch]
public class NotificationLog : AuditableEntity, IAggregateRoot
{
    [ColumnSupportDeepSearch]
    public string? Title { get; set; }

    public string? Message { get; set; }

    public string? EntityId { get; set; }

    public string? ParentId { get; set; }

    public string? OtherData { get; set; }

    public string? UserId { get; set; }
    public bool Viewed { get; set; }

    public string? FileUrl { get; set; }
    public NotificationType NotificaitionType { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }
}
