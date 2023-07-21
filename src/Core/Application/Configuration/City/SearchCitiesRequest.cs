using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.City;

public class SearchCitiesRequest : PaginationFilter, IQuery<PaginatedResult<CityDto>>
{
    public bool? IsActive { get; set; }
    public Guid? StateId { get; set; }

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

public class CitiesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.City, CityDto>
{
    public CitiesBySearchRequestSpec(SearchCitiesRequest request)
        : base(request) =>
        Query.Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
             .Where(x => x.StateId == request.StateId, request.StateId.HasValue)
             .OrderBy(c => c.Name, !request.HasOrderBy());
}

public class CitiesBySearchRequestDomainSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.City>
{
    public CitiesBySearchRequestDomainSpec(SearchCitiesRequest request)
        : base(request) =>
        Query.Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
             .Where(x => x.StateId == request.StateId, request.StateId.HasValue)
             .OrderBy(c => c.Name, !request.HasOrderBy());
}

public class SearchCitiesRequestHandler : IQueryHandler<SearchCitiesRequest, PaginatedResult<CityDto>>
{
    private readonly IReadRepository<Domain.Configuration.City> _repository;

    public SearchCitiesRequestHandler(IReadRepository<Domain.Configuration.City> repository) => _repository = repository;

    public async Task<PaginatedResult<CityDto>> Handle(SearchCitiesRequest request, CancellationToken cancellationToken)
    {
        var spec = new CitiesBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}