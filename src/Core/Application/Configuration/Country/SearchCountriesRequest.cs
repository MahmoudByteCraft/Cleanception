using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.Country;

public class SearchCountriesRequest : PaginationFilter, IQuery<PaginatedResult<CountryDto>>
{
    public bool? IsActive { get; set; }

    public override int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value < 0)
                _pageSize = int.MaxValue;
            else
                _pageSize = value;
        }
    }
}

public class CountriesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.Country, CountryDto>
{
    public CountriesBySearchRequestSpec(SearchCountriesRequest request)
        : base(request) =>
        Query
        .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
        .OrderBy(c => c.Name, !request.HasOrderBy());
}

public class CountriesBySearchRequestDomainSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.Country>
{
    public CountriesBySearchRequestDomainSpec(SearchCountriesRequest request)
        : base(request) =>
        Query
        .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
        .OrderBy(c => c.Name, !request.HasOrderBy());
}

public class SearchCountriesRequestHandler : IQueryHandler<SearchCountriesRequest, PaginatedResult<CountryDto>>
{
    private readonly IReadRepository<Domain.Configuration.Country> _repository;

    public SearchCountriesRequestHandler(IReadRepository<Domain.Configuration.Country> repository) => _repository = repository;

    public async Task<PaginatedResult<CountryDto>> Handle(SearchCountriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new CountriesBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}