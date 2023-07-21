using System.ComponentModel.DataAnnotations.Schema;
using Cleanception.Domain.Attributes;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Shared.Notifications;
using Newtonsoft.Json;

namespace Cleanception.Domain.Notification;

[SupportDeepSearch]
public class Notification : AuditableEntity, IAggregateRoot
{
    [ColumnSupportDeepSearch]
    public string? Message { get; set; }

    [ColumnSupportDeepSearch]
    public string? Title { get; set; }

    public string? RolesNames { get; set; }
    public string? CustomerTypeNames { get; set; }
    public string? PriceCodesNames { get; set; }
    public string? BranchNames { get; set; }
    public string? SalePersonNames { get; set; }

    public string? NotificationParametersJson { get; set; }
    public string? NotificationMethodsJson { get; set; }

    public NotificationType NotificationType { get; set; }
    public DateTime? NotifyDate { get; set; }
    public bool CanceledNotification { get; set; }
    public string? JobId { get; set; }
    public string? FileUrl { get; set; }
    public DefaultIdType? EntityId { get; set; }

    [NotMapped]
    public NotificationParameters NotificationParameters
    {
        get => string.IsNullOrEmpty(NotificationParametersJson) ? new NotificationParameters() : JsonConvert.DeserializeObject<NotificationParameters>(NotificationParametersJson);
        set => NotificationParametersJson = value != null ? JsonConvert.SerializeObject(value) : null;
    }

    [NotMapped]

    public List<NotificationMethod> NotificationMethods
    {
        get => string.IsNullOrEmpty(NotificationMethodsJson) ? new List<NotificationMethod>() : JsonConvert.DeserializeObject<List<NotificationMethod>>(NotificationMethodsJson);
        set => NotificationMethodsJson = value != null ? JsonConvert.SerializeObject(value) : null;
    }

    public NotificationTarget NotificationTarget { get; set; }
}

public class NotificationParameters
{
    public List<string>? UserIds { get; set; }
    public List<string>? RoleIds { get; set; }
}