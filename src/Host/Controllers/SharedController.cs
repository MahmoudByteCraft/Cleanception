using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Host.Controllers;

public class SharedController : VersionedApiController
{
    [HttpPost("setall")]
    [OpenApiOperation("Active/InActive/Delete Multiple Records.", "")]
    public async Task<Result<int>> SearchAsync(ChangeRecordStatusRequest request)
    {
        return await Mediator.Send(request);
    }
}