using Cleanception.Application.Common.Models;
using Cleanception.Application.Configuration.City;

namespace Cleanception.Host.Controllers.Configuration;

public class CitiesController : VersionedApiController
{
    [HttpPost("search")]
    [AllowAnonymous]
    [OpenApiOperation("Search for cities using available filters.", "")]
    public async Task<PaginatedResult<CityDto>> SearchAsync(SearchCitiesRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get city details.", "")]
    public async Task<Result<CityDto>> GetAsync(Guid id)
    {
        return await Mediator.Send(new GetCityRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create a new City.", "")]
    public async Task<Result<Guid>> CreateAsync(CreateCityRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut]
    [OpenApiOperation("Update a city.", "")]
    public async Task<Result<Guid>> UpdateAsync(UpdateCityRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a city.", "")]
    public async Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteCityRequest(id));
    }

    [HttpPost("toggelstatus")]
    [OpenApiOperation("Toggle City Status.", "")]
    public async Task<Result<Guid>> CreateAsync(ToggleCityStatusRequest request)
    {
        return await Mediator.Send(request);
    }
}