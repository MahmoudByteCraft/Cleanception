using Cleanception.Domain.Notification;
using Cleanception.Shared.Notifications;
using Hangfire;

namespace Cleanception.Application.Common.Interfaces;
public interface INotificationService : ITransientService
{
    [Queue("notification")]
    [AutomaticRetry(Attempts = 0)]
    Task SendGeneralNotification(Domain.Notification.Notification filters);

    [Queue("notification")]
    [AutomaticRetry(Attempts = 0)]
    Task SendByTrigger(NotificationTrigger trigger, SendingByTriggerOptions? options = null);
}

public class SendingByTriggerOptions
{
    public Guid? EntityId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? OtherId { get; set; }
    public List<string>? RoleIds { get; set; }
    public List<Guid>? OtherIds { get; set; }
    public SendingByTriggerTemplates TemplatesToUse { get; set; }
    public List<NotificationMethod> NotificationMethods { get; set; }
    public bool? ScheduleTemplate { get; set; }
    public Dictionary<string, string>? CustomMessageVariablesToReplace { get; set; }
    public string? SelfUserId { get; set; }
    public string? SelfPropertyName { get; set; }
    public bool StoreNotification { get; set; } = true;
}

public enum SendingByTriggerTemplates
{
    BothTemplates,
    SelfTemplates,
    RelatedUsersTemplates
}
