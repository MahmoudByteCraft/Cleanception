using Cleanception.Application.Common.Interfaces;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.Notifications;

public class NotificationSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Message { get; set; }
    public string? Title { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTarget NotificationTarget { get; set; }

    public string? RolesNames { get; set; }
    public string? CustomerTypeNames { get; set; }
    public string? PriceCodesNames { get; set; }
    public string? BranchNames { get; set; }
    public string? SalePersonNames { get; set; }
    public string? NotificationParametersJson { get; set; }
    public string? NotificationMethodsJson { get; set; }

    public DateTime? NotifyDate { get; set; }
    public bool CanceledNotification { get; set; }
    public string? JobId { get; set; }
    public string? FileUrl { get; set; }
    public DefaultIdType? EntityId { get; set; }

    public DateTime CreatedOn { get; set; }
}

public class NotificationRawDto : NotificationSimplifyDto
{

}

public class NotificationDto : NotificationRawDto
{

}

public class NotificationDetailsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Message { get; set; }
    public string? Title { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTarget NotificationTarget { get; set; }

    public string? RolesNames { get; set; }
    public string? CustomerTypeNames { get; set; }
    public string? PriceCodesNames { get; set; }
    public string? BranchNames { get; set; }
    public string? SalePersonNames { get; set; }
    public string? NotificationParametersJson { get; set; }
    public string? NotificationMethodsJson { get; set; }

    public DateTime? NotifyDate { get; set; }
    public bool CanceledNotification { get; set; }
    public string? JobId { get; set; }
    public string? FileUrl { get; set; }
    public DefaultIdType? EntityId { get; set; }

    public DefaultIdType CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}