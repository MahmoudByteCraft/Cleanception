using System.Linq.Expressions;
using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Mailing;
using Cleanception.Application.Common.SMS;
using Cleanception.Domain.Notification;
using Cleanception.Infrastructure.Common.Extensions;
using Cleanception.Infrastructure.Persistence.Context;
using Cleanception.Shared.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Notifications.Service;

public partial class NotificationService : INotificationService
{
    private ApplicationDbContext _context;

    private IFirebaseNotificationService _firebaseNotificationService;
    private ISmsService _smsService;
    private IEmailTemplateService _emailTemplateService;
    private IMailService _mailService;

    public NotificationService(ISmsService smsService, IFirebaseNotificationService firebaseNotificationService, IMailService mailService, IEmailTemplateService emailTemplateService, ApplicationDbContext context)
    {
        _firebaseNotificationService = firebaseNotificationService;
        _context = context;
        _smsService = smsService;
        _emailTemplateService = emailTemplateService;
        _mailService = mailService;
    }

    public async Task SendGeneralNotification(Notification Notification)
    {
        var message = new BasicNotification
        {
            Title = Notification.Title,
            Message = Notification.Message,
            NotificaitionType = Notification.NotificationType,
            EntityId = Notification.EntityId.ToString(),
            FileUrl = Notification.FileUrl
        };

        if (Notification.NotificationTarget == NotificationTarget.Global)
        {
            await _firebaseNotificationService.SendBroadCast(message);
            return;
        }

        var query = BuildQueryable(Notification);

        await foreach (var item in PickUsersUsingQueryable(query))
        {
            foreach (var method in Notification.NotificationMethods)
            {
                if (method == NotificationMethod.Notification)
                {
                    await _firebaseNotificationService.SendMulticast(message, item.Where(x => x.FcmToken != null).Select(x => x.FcmToken).ToList());
                }

                if (method == NotificationMethod.Email)
                {
                    string templete = _emailTemplateService.GenerateEmailTemplate("general_mail", message);
                    var mailRequest = new MailRequest(item.Where(x => x.Email.HasValue()).Select(x => x.Email).ToList(), message.Title, templete);
                    await _mailService.SendAsync(mailRequest, CancellationToken.None);
                }

                if (method == NotificationMethod.SMS)
                {
                    await _smsService.SendMulticast(message.Message!, item.Where(x => x.PhoneNumber.HasValue()).Select(x => x.PhoneNumber!).ToList());
                }
            }

            await StoreNotification(item, message);
        }
    }

    public async Task SendByTrigger(NotificationTrigger trigger, SendingByTriggerOptions? options = null)
    {
        if (options == null)
            options = new SendingByTriggerOptions();

        var templates = await GetMessageTemplatesByTrigger(trigger, options.TemplatesToUse, options.ScheduleTemplate);

        if (options.RoleIds != null)
            templates = templates.Where(x => options.RoleIds.Contains(x.RoleId)).ToList();

        var roles = await _context.Roles.Select(x => new { Id = x.Id, Name = x.Name}).ToListAsync();

        (string? FieldValue, object? Entity)? triggerEntity = new(null, null);

        // if (options.EntityId.HasValue)
        //    triggerEntity = await GetObjectOfTheTrigger(trigger, options.EntityId?.ToString());

        if (options.EntityId.HasValue)
            triggerEntity = await GetObjectOfTheTriggerByProperty(trigger, options.EntityId?.ToString(), options.ParentId?.ToString(), options.OtherId?.ToString());

        foreach (var template in templates)
        {
            var currentRoleDetail = roles.Find(x => x.Id == template.RoleId);

            if (currentRoleDetail == null && template.RoleId.HasValue())
                continue;

            var basicNotificationMessage = GetTemplateBasicNotification(template.NotificationTemplate, trigger);

            var basicSmsMessage = GetTemplateBasicNotification(template.SmsTemplate, trigger);

            var basicEmailMessage = GetTemplateBasicNotification(template.EmailTemplate, trigger);

            if (basicNotificationMessage == null && basicSmsMessage == null && basicEmailMessage == null)
                continue;

            if (basicNotificationMessage != null )
            {
                basicNotificationMessage.EntityId = options.EntityId?.ToString();
                basicNotificationMessage.ParentId = options.ParentId?.ToString();
                basicNotificationMessage.Message = await ReplaceMessageVariables(trigger, basicNotificationMessage.Message, triggerEntity?.Entity, null, basicNotificationMessage.EntityId!, basicNotificationMessage.ParentId, options.CustomMessageVariablesToReplace);
            }

            if (basicSmsMessage != null)
            {
                basicSmsMessage.EntityId = options.EntityId?.ToString();
                basicSmsMessage.ParentId = options.ParentId?.ToString();
                basicSmsMessage.Message = await ReplaceMessageVariables(trigger, basicSmsMessage.Message, triggerEntity?.Entity, null, basicSmsMessage.EntityId!, basicSmsMessage.ParentId, options.CustomMessageVariablesToReplace);
            }

            if (basicEmailMessage != null)
            {
                basicEmailMessage.EntityId = options.EntityId?.ToString();
                basicEmailMessage.ParentId = options.ParentId?.ToString();
                basicEmailMessage.Message = await ReplaceMessageVariables(trigger, basicEmailMessage.Message, triggerEntity?.Entity, null, basicEmailMessage.EntityId!, basicEmailMessage.ParentId, options.CustomMessageVariablesToReplace);
            }

            Notification? ubNotificationParameters = null;

            if(template.RoleId.HasValue())
            {
                ubNotificationParameters = new Notification
                {
                    NotificationParameters = new NotificationParameters
                    {
                        RoleIds = template.RoleId.HasValue() ? new List<string> { template.RoleId! } : null
                    },
                    NotificationTarget = GetRoleNotificationTarget(currentRoleDetail.Name)
                };
            }
            else
            {
                string? selfUserId = null;

                if (options.SelfUserId.HasValue())
                {
                    selfUserId = options.SelfUserId;
                }
                else if (triggerEntity.HasValue && triggerEntity.Value.FieldValue.HasValue() && Guid.Parse(triggerEntity.Value.FieldValue!) != Guid.Empty)
                {
                    selfUserId = triggerEntity!.Value.FieldValue;
                }
                else
                {
                    continue;
                }

                ubNotificationParameters = new Notification
                {
                    NotificationParameters = new NotificationParameters
                    {
                        UserIds = new List<string> { selfUserId! }
                    },
                };
            }

            //var expression = BuildExpression(ubNotificationParameters);
            var query = BuildQueryable(ubNotificationParameters);

            await foreach (var item in PickUsersUsingQueryable(query))
            {
                if (basicNotificationMessage != null && (options.NotificationMethods == null || options.NotificationMethods.Contains(NotificationMethod.Notification)))
                {
                    await _firebaseNotificationService.SendMulticast(basicNotificationMessage, item.Where(x => x.FcmToken != null).Select(x => x.FcmToken).ToList());
                }

                if (basicSmsMessage != null && (options.NotificationMethods == null || options.NotificationMethods.Contains(NotificationMethod.SMS)))
                {
                    await _smsService.SendMulticast(basicSmsMessage.Message!, item.Where(x => x.PhoneNumber.HasValue()).Select(x => x.PhoneNumber!).ToList());
                }

                if (basicEmailMessage != null && (options.NotificationMethods == null || options.NotificationMethods.Contains(NotificationMethod.Email)))
                {
                    string templete = _emailTemplateService.GenerateEmailTemplate("general_mail", basicEmailMessage);
                    var mailRequest = new MailRequest(item.Where(x => x.Email.HasValue()).Select(x => x.Email).ToList(), basicEmailMessage.Message, templete);
                    await _mailService.SendAsync(mailRequest, CancellationToken.None);
                }

                if((basicNotificationMessage != null || basicSmsMessage != null
                    || basicEmailMessage != null) && options.StoreNotification)
                    await StoreNotification(item, basicNotificationMessage ?? basicSmsMessage ?? basicEmailMessage);
            }
        }
    }

    private async Task<List<GroupedNotification>> GetMessageTemplatesByTrigger(NotificationTrigger trigger, SendingByTriggerTemplates templatesToUse, bool? scheduleTemplate = null)
    {
        List<NotificationTemplate>? templates;

        Expression<Func<NotificationTemplate, bool>> templatesExpression = x => x.IsActive && x.NotificationTrigger == trigger;

        if(templatesToUse == SendingByTriggerTemplates.SelfTemplates)
        {
            templatesExpression = templatesExpression.And(x => x.IsSelfTrigger);
        }
        else if (templatesToUse == SendingByTriggerTemplates.RelatedUsersTemplates)
        {
            templatesExpression = templatesExpression.And(x => !x.IsSelfTrigger);
        }

        if (scheduleTemplate.HasValue)
        {
            templatesExpression = templatesExpression.And(x => x.ScheduleTemplate == scheduleTemplate);
        }

        templates = await _context.NotificationTemplates.Where(templatesExpression).ToListAsync();

        return templates.GroupBy(x => x.RoleId).Select(x => new GroupedNotification
        {
            RoleId = x.Key,
            NotificationTemplate = x.FirstOrDefault(y => y.NotificationMethod == NotificationMethod.Notification),
            SmsTemplate = x.FirstOrDefault(y => y.NotificationMethod == NotificationMethod.SMS),
            EmailTemplate = x.FirstOrDefault(y => y.NotificationMethod == NotificationMethod.Email),
        }).ToList();
    }

    private NotificationType GetTriggerNotificationType(NotificationTrigger trigger)
    {
        if (trigger == NotificationTrigger.NewUser)
            return NotificationType.Account;

        return NotificationType.Unknown;
    }

    private BasicNotification? GetTemplateBasicNotification(NotificationTemplate? template, NotificationTrigger trigger)
    {
        if (template == null)
            return null;

        return new BasicNotification
        {
            Title = template.Title,
            Message = template.Message,
            NotificaitionType = GetTriggerNotificationType(template.NotificationTrigger),
            NotificationTrigger = trigger
        };
    }

    private NotificationTarget GetRoleNotificationTarget(string roleName)
    {
        return NotificationTarget.User;
    }
}

public class GroupedNotification
{
    public string? RoleId { get; set; }
    public NotificationTemplate? NotificationTemplate { get; set; }
    public NotificationTemplate? SmsTemplate { get; set; }
    public NotificationTemplate? EmailTemplate { get; set; }
}