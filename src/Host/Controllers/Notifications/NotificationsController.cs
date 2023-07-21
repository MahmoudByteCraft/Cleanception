using Cleanception.Application.Common.Models;
using Cleanception.Application.Notification.Notifications;

namespace Cleanception.Host.Controllers.Notifications;

public class NotificationsController : VersionedApiController
{
    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get Notification details.", "")]
    public Task<Result<NotificationDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetNotificationRequest(id));
    }

    [HttpPost("search")]
    [OpenApiOperation("Search for Notifications using available filters.", "")]
    public async Task<PaginatedResult<NotificationDto>> SearchAsync(SearchNotificationsRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("cancel")]
    [OpenApiOperation("Cancel notification .", "")]
    public async Task<Result<Guid>> CancelAsync(CancelNotificationRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete notification .", "")]
    public async Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteNotificationRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Send notification.", "")]
    public async Task<Result<bool>> CreateAsync(SendNotificationRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("resend")]
    [OpenApiOperation("Resend notification.", "")]
    public async Task<Result<bool>> ResendAsync(ReSendNotificationRequest request)
    {
        return await Mediator.Send(request);
    }


    [HttpPost("trigger/variables")]
    [OpenApiOperation("Get notification trigger variables.", "")]
    public async Task<Result<List<string>>> TriggerVariables(GetNotificationTriggerVariables request)
    {
        return await Mediator.Send(request);
    }
}