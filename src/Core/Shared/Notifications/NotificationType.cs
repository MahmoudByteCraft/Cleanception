using System.Text.Json.Serialization;

namespace Cleanception.Shared.Notifications;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationType
{
    General,
    Account,
    Unknown = 1000
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationMethod
{
    Notification,
    SMS,
    Email
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTarget
{
    Global,
    User
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTrigger
{
    NewUser,
    Unknown = 1000
}