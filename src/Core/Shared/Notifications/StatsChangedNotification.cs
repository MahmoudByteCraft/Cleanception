namespace Cleanception.Shared.Notifications;

public class StatsChangedNotification : INotificationMessage
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? EntityId { get; set; }
    public string? ParentId { get; set; }
    public string? OtherData { get; set; }
    public string? FileUrl { get; set; }
    public NotificationType NotificaitionType { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }
}