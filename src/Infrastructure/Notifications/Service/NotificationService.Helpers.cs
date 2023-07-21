using System.Linq.Expressions;
using Cleanception.Application.Common.Extensions;
using Cleanception.Domain.Notification;
using Cleanception.Infrastructure.Identity;
using Cleanception.Shared.Constants;
using Cleanception.Shared.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Notifications.Service;

public partial class NotificationService
{
    private IQueryable<ApplicationUser> BuildQueryable(Notification notification)
    {
        var userExpression = _context.Users.AsQueryable().AsNoTracking();

        if (notification.NotificationParameters.UserIds?.Count > 0)
        {
            return userExpression.Where(x => notification.NotificationParameters.UserIds!.Contains(x.Id));
        }

        return userExpression;
    }

    private async IAsyncEnumerable<List<ApplicationUser>> PickUsers(Expression<Func<ApplicationUser, bool>> expression)
    {
        int takeCount = 50;
        int skip = 0;

        while (true)
        {
            var chunk = await _context.Users.AsNoTracking().Where(expression).Skip(skip).Take(takeCount).ToListAsync();
            if (chunk.Count == 0)
                break;

            yield return chunk;

            skip += takeCount;

        }
    }

    private async IAsyncEnumerable<List<ApplicationUser>> PickUsersUsingQueryable(IQueryable<ApplicationUser> query)
    {
        int takeCount = 50;
        int skip = 0;

        while (true)
        {
            var chunk = await query.Skip(skip).Take(takeCount).ToListAsync();
            if (chunk.Count == 0)
                break;

            yield return chunk;

            skip += takeCount;

        }
    }

    private async Task<string?> ReplaceMessageVariables(NotificationTrigger trigger, string? message, object? entity, object? parentEntity, string? entityId, string? parentId, Dictionary<string, string>? messageVariablesToReplace)
    {
        if (!message.HasValue())
            return message;

        if (messageVariablesToReplace != null && messageVariablesToReplace.Count > 0)
        {
            foreach (var item in messageVariablesToReplace)
            {
                message = message!.Replace(item.Key, item.Value);
            }
        }

        if (entity == null)
            return message;

        if(trigger == NotificationTrigger.NewUser)
        {
            var applicationUser = (ApplicationUser)entity;
            message = message!.Replace(NotificationVariablesNameConstants.NewUser.FullName, applicationUser?.FullName ?? "-");
        }

        return message;
    }

    private async Task<(string? createdBy, object? obj)> GetObjectOfTheTrigger(NotificationTrigger trigger, string? entityId)
    {
        if (entityId == null)
            return (null, null);

        if (trigger == NotificationTrigger.NewUser)
        {
            var entity = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId);

            return new (entity?.CreatedBy.ToString(), entity);
        }

        return new(null, null);
    }

    private async Task<(string? fieldValue, object? obj)> GetObjectOfTheTriggerByProperty(NotificationTrigger trigger, string? entityId, string? parentId = null, string? otherId = null, string? propertyName = "CreatedBy")
    {
        if (entityId == null)
            return (null, null);

        if (trigger == NotificationTrigger.NewUser)
        {
            var entity = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId);

            return new(entity?.GetType()?.GetProperty(propertyName!)?.GetValue(entity)?.ToString(), entity);
        }

        return new(null, null);
    }

    private async Task StoreNotification(List<ApplicationUser> users, INotificationMessage notification)
    {
        var entitesToStore = users.Select(x => new NotificationLog
        {
            Title = notification.Title,
            Message = notification.Message,
            UserId = x.Id,
            EntityId = notification.EntityId,
            ParentId = notification.ParentId,
            OtherData = notification.OtherData,
            FileUrl = notification.FileUrl,
            NotificaitionType = notification.NotificaitionType,
            NotificationTrigger = notification.NotificationTrigger
        });

        await _context.NotificationLogs.AddRangeAsync(entitesToStore);

        await _context.SaveChangesAsync();
    }

    private async Task StoreNotifications(List<ApplicationUser> users, List<INotificationMessage> notifications)
    {
        var messages = new List<NotificationLog>();

        foreach (var notification in notifications)
        {
            var entitesToStore = users.Select(x => new NotificationLog
            {
                Title = notification.Title,
                Message = notification.Message,
                UserId = x.Id,
                EntityId = notification.EntityId,
                ParentId = notification.ParentId,
                OtherData = notification.OtherData,
                FileUrl = notification.FileUrl,
                NotificaitionType = notification.NotificaitionType,
                NotificationTrigger = notification.NotificationTrigger
            });

            messages.AddRange(entitesToStore);
        }

        await _context.NotificationLogs.AddRangeAsync(messages);

        await _context.SaveChangesAsync();
    }
}
