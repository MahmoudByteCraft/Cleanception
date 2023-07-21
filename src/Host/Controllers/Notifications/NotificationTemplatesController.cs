using Cleanception.Application.Common.Models;
using Cleanception.Application.Notification.NotificationTemplate;

namespace Cleanception.Host.Controllers.Notifications;

public class NotificationTemplatesController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Search for notification templates using available filters.", "")]
    public async Task<PaginatedResult<NotificationTemplateDto>> SearchAsync(SearchNotificationTemplatesRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get notification templates details.", "")]
    public async Task<Result<NotificationTemplateDto>> GetAsync(Guid id)
    {
        return await Mediator.Send(new GetNotificationTemplateRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create a new notification templates.", "")]
    public async Task<Result<Guid>> CreateAsync(CreateNotificationTemplateRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut]
    [OpenApiOperation("Update a notification template.", "")]
    public async Task<Result<Guid>> UpdateAsync(UpdateNotificationTemplateRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a notification template.", "")]
    public async Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteNotificationTemplateRequest(id));
    }
}