using Cleanception.Application.Common.Models;
using Cleanception.Application.Configuration.State;

namespace Cleanception.Host.Controllers.Configuration;

public class StatesController : VersionedApiController
{
    [HttpPost("search")]
    [AllowAnonymous]
    [OpenApiOperation("Search for states using available filters.", "")]
    public async Task<PaginatedResult<StateDto>> SearchAsync(SearchStatesRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get state details.", "")]
    public async Task<Result<StateDto>> GetAsync(Guid id)
    {
        return await Mediator.Send(new GetStateRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create a new state.", "")]
    public async Task<Result<Guid>> CreateAsync(CreateStateRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut]
    [OpenApiOperation("Update a state.", "")]
    public async Task<Result<Guid>> UpdateAsync(UpdateStateRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a state.", "")]
    public async Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteStateRequest(id));
    }

    [HttpPost("toggelstatus")]
    [OpenApiOperation("Toggle State Status.", "")]
    public async Task<Result<Guid>> CreateAsync(ToggleStateStatusRequest request)
    {
        return await Mediator.Send(request);
    }
}