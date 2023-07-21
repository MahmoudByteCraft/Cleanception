using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Shared.Constants;
using Cleanception.Shared.Notifications;

namespace Cleanception.Application.Notification.Notifications;

public class GetNotificationTriggerVariables : ICommand<Result<List<string>>>
{
    public NotificationTrigger Trigger { get; set; }
}

public class GetNotificationTriggerVariablesRequestHandler : ICommandHandler<GetNotificationTriggerVariables, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(GetNotificationTriggerVariables request, CancellationToken cancellationToken)
    {
        return Result<List<string>>.SuccessListAsData(GetTriggerVariables(request.Trigger));
    }

    private List<string> GetTriggerVariables(NotificationTrigger trigger)
    {
        if (trigger == NotificationTrigger.NewUser)
        {
            return new List<string>
            {
                NotificationVariablesNameConstants.NewUser.FullName,
                NotificationVariablesNameConstants.NewUser.ConfirmationCode
            };
        }

        return new List<string>();
    }
}
