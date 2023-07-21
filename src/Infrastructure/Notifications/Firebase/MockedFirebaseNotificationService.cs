using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Identity.Interfaces;
using Cleanception.Shared.Notifications;

namespace Cleanception.Infrastructure.Notifications.Firebase;

public class MockedFirebaseNotificationService : IFirebaseNotificationService
{
    public async Task<bool> Send(INotificationMessage notification, string fcmToken, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        return true;
    }

    public async Task<bool> SendBroadCast(INotificationMessage notification, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);
        return true;
    }

    public async Task<bool> SendMulticast(INotificationMessage notification, List<string?> fcmToken, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        return true;
    }

    public async Task<bool> SendMulticast(INotificationMessage notification, List<IFirebaseIdentityUser?> users, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        return true;
    }
}