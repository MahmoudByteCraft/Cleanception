using Cleanception.Application.Identity.Interfaces;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Common.Interfaces;

public interface IFirebaseNotificationService
{
    Task<bool> Send(INotificationMessage notification, string fcmToken, CancellationToken cancellationToken = default);

    Task<bool> SendMulticast(INotificationMessage notification, List<string?> fcmToken, CancellationToken cancellationToken = default);

    Task<bool> SendBroadCast(INotificationMessage notification, CancellationToken cancellationToken = default);

    Task<bool> SendMulticast(INotificationMessage notification, List<IFirebaseIdentityUser?> users, CancellationToken cancellationToken = default);
}