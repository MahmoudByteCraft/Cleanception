using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Users;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.NotificationLogs;

public class NotificationLogSimplifyDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? EntityId { get; set; }
    public string? ParentId { get; set; }

    public string? OtherData { get; set; }

    public string? UserId { get; set; }
    public string? AppCustomerId { get; set; }
    public string? EmployeeId { get; set; }
    public bool Viewed { get; set; }

    public string? FileUrl { get; set; }
    public NotificationType NotificaitionType { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }

    public DateTime? CreatedOn { get; set; }
}

public class NotificationLogRawDto : NotificationLogSimplifyDto
{

}

public class NotificationLogDto : NotificationLogRawDto
{
    public UserSimplifyDto? User { get; set; }
}

public class NotificationLogDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }

    public string? Message { get; set; }

    public string? EntityId { get; set; }

    public string? ParentId { get; set; }

    public string? OtherData { get; set; }

    public string? UserId { get; set; }
    public string? AppCustomerId { get; set; }
    public string? EmployeeId { get; set; }
    public bool Viewed { get; set; }

    public string? FileUrl { get; set; }
    public NotificationType NotificaitionType { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }
    public DateTime? CreatedOn { get; set; }
}