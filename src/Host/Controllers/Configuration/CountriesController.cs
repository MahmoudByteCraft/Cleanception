using Cleanception.Application.Common.Models;
using Cleanception.Application.Configuration.Country;

namespace Cleanception.Host.Controllers.Configuration;

public class CountriesController : VersionedApiController
{
    [HttpPost("search")]
    [AllowAnonymous]
    [OpenApiOperation("Search for countries using available filters.", "")]
    public async Task<PaginatedResult<CountryDto>> SearchAsync(SearchCountriesRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get country details.", "")]
    public async Task<Result<CountryDto>> GetAsync(Guid id)
    {
        return await Mediator.Send(new GetCountryRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create a new country.", "")]
    public async Task<Result<Guid>> CreateAsync(CreateCountryRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut]
    [OpenApiOperation("Update a country.", "")]
    public async Task<Result<Guid>> UpdateAsync(UpdateCountryRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a country.", "")]
    public async Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteCountryRequest(id));
    }

    [HttpPost("toggelstatus")]
    [OpenApiOperation("Toggle Country Status.", "")]
    public async Task<Result<Guid>> CreateAsync(ToggleCountryStatusRequest request)
    {
        return await Mediator.Send(request);
    }
}