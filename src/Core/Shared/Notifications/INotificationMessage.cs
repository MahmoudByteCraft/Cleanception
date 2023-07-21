namespace Cleanception.Shared.Notifications;

public interface INotificationMessage
{
    // default title support english
    public string? Title { get; set; }

    // default value support english
    public string? Message { get; set; }

    public string? EntityId { get; set; }
    public string? ParentId { get; set; }
    public string? OtherData { get; set; }
    public string? FileUrl { get; set; }
    public NotificationType NotificaitionType { get; set; }
    public NotificationTrigger NotificationTrigger { get; set; }

    //// the key will be the language code of the title like [AR], [EN], and the value will be the text, it will be used from the frontend if the language is not english
    // public Dictionary<string?, string?> TitleDictionary { get; set; }

    //// the key will be the language code of the value like [AR], [EN], and the value will be the text, it will be used from the frontend if the language is not english
    // public Dictionary<string?, string?> ValueDictionary { get; set; }
}