using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Interfaces;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Shared.Notifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace Cleanception.Infrastructure.Notifications.Firebase;

public partial class FirebaseNotificationService : IFirebaseNotificationService
{
    private const int MaxUsersPerChunk = 100;

    private readonly FirebaseSettings _firebaseMainSettings;
    private readonly FirebaseAuthConfiguration _firebaseAuthConfiguration;
    private readonly ApplicationDbContext _dbContext;
    private readonly FirebaseSettings _firebaseSettings;

    public FirebaseNotificationService(FirebaseSettings firebaseSettings, ApplicationDbContext dbContext, FirebaseSettings firebaseMainSettings, FirebaseAuthConfiguration firebaseAuthConfiguration)
    {
        _firebaseMainSettings = firebaseMainSettings;
        _firebaseAuthConfiguration = firebaseAuthConfiguration;
        _dbContext = dbContext;
        _firebaseSettings = firebaseSettings;

        if (FirebaseApp.DefaultInstance == null)
        {
            try
            {
                FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(_firebaseAuthConfiguration)) });
            }
            catch { }
        }
    }

    public async Task<bool> Send(INotificationMessage notification, string fcmToken, CancellationToken cancellationToken = default)
    {
        if (notification == null || fcmToken == null)
            return false;

        if (notification.FileUrl.HasValue())
            notification.FileUrl = _firebaseSettings.DomainUrl + notification.FileUrl;

        var message = GenerateUnicastMessage(notification);

        message.Token = fcmToken;

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
        }
        catch { }

        return true;
    }

    public async Task<bool> SendMulticast(INotificationMessage notification, List<string?> fcmToken, CancellationToken cancellationToken = default)
    {
        if (fcmToken == null || fcmToken.Count == 0)
            return true;

        if(notification.FileUrl.HasValue())
            notification.FileUrl = _firebaseSettings.DomainUrl + notification.FileUrl;

        var message = GenerateMulticastMessage(notification);

        foreach (var chunk in SplitListAsChunks(fcmToken))
        {
            try
            {
                message.Tokens = chunk.ToList();
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, cancellationToken);
            }
            catch { }
        }

        return true;
    }

    public async Task<bool> SendMulticast(INotificationMessage notification, List<IFirebaseIdentityUser?> users, CancellationToken cancellationToken = default)
    {
        if (users == null || users.Count == 0)
            return true;

        if (notification.FileUrl.HasValue())
            notification.FileUrl = _firebaseSettings.DomainUrl + notification.FileUrl;

        var message = GenerateMulticastMessage(notification);

        foreach (var chunk in SplitListAsChunks(users.Where(x => x?.FcmToken.HasValue() == true).Select(x => x!.FcmToken).ToList()))
        {
            try
            {
                message.Tokens = chunk.ToList();
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, cancellationToken);
            }
            catch { }
        }

        return true;
    }

    public async Task<bool> SendBroadCast(INotificationMessage notification, CancellationToken cancellationToken = default)
    {
        if (notification.FileUrl.HasValue())
            notification.FileUrl = _firebaseSettings.DomainUrl + notification.FileUrl;

        var message = GenerateUnicastMessage(notification);

        message.Topic = "all";

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
        }
        catch { }

        return true;
    }

}

public partial class FirebaseNotificationService
{
    private Message GenerateUnicastMessage(INotificationMessage notification)
    {
        Message message = new Message()
        {
            Notification = new Notification
            {
                Title = notification.Title,
                Body = notification.Message,
                ImageUrl = notification.FileUrl.HasValue() ? notification.FileUrl : null,
            },
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    Title = notification.Title,
                    Body = notification.Message,
                    ImageUrl = notification.FileUrl.HasValue() ? notification.FileUrl : null,
                }
            },
            Data = new Dictionary<string, string>()
            {
                 { nameof(INotificationMessage.Title), notification.Title.IsEmpty() ? string.Empty : notification.Title! },
                 { nameof(INotificationMessage.Message), notification.Message.IsEmpty() ? string.Empty : notification.Message },
                 { nameof(INotificationMessage.EntityId), !notification.EntityId.HasValue() ? null : notification.EntityId },
                 { nameof(INotificationMessage.FileUrl), notification.FileUrl.IsEmpty() ? string.Empty : notification.FileUrl },
                 { nameof(INotificationMessage.NotificaitionType), notification.NotificaitionType.ToString() },
            }
        };

        return message;
    }

    private MulticastMessage GenerateMulticastMessage(INotificationMessage notification)
    {
        MulticastMessage message = new MulticastMessage()
        {
            Notification = new Notification
            {
                Title = notification.Title,
                Body = notification.Message,
                ImageUrl = notification.FileUrl.HasValue() ? notification.FileUrl : null,
            },
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    Title = notification.Title,
                    Body = notification.Message,
                    ImageUrl = notification.FileUrl.HasValue() ? notification.FileUrl : null,
                }
            },
            Data = new Dictionary<string, string>()
            {
                 { nameof(INotificationMessage.Title), notification.Title.IsEmpty() ? string.Empty : notification.Title! },
                 { nameof(INotificationMessage.Message), notification.Message.IsEmpty() ? string.Empty : notification.Message },
                 { nameof(INotificationMessage.EntityId), !notification.EntityId.HasValue() ? null : notification.EntityId },
                 { nameof(INotificationMessage.FileUrl), notification.FileUrl.IsEmpty() ? string.Empty : notification.FileUrl },
                 { nameof(INotificationMessage.NotificaitionType), notification.NotificaitionType.ToString() },
            }
        };

        return message;
    }

    private IEnumerable<IEnumerable<string?>> SplitListAsChunks(List<string?> fcmTokens)
    {
        int startIndex = 0;

        while (true)
        {
            var result = fcmTokens.Skip(startIndex).Take(MaxUsersPerChunk);
            if (result.Count() != 0)
            {
                yield return result;
                startIndex += result.Count();
            }
            else
            {
                break;
            }
        }
    }
}
