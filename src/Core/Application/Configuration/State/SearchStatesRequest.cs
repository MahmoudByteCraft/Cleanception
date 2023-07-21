using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.State;

public class SearchStatesRequest : PaginationFilter, IQuery<PaginatedResult<StateDto>>
{
    public string? CountryDialCode { get; set; }

    public bool? IsActive { get; set; }
    public Guid? CountryId { get; set; }

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

public class StatesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.State, StateDto>
{
    public StatesBySearchRequestSpec(SearchStatesRequest request)
        : base(request) =>
                Query.Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
                     .Where(x => x.Country.DialCode == request.CountryDialCode, request.CountryDialCode.HasValue())
                     .Where(x => x.CountryId == request.CountryId, request.CountryId.HasValue)
                     .OrderBy(c => c.Name, !request.HasOrderBy());
}

public class StatesBySearchRequestDomainSpec : EntitiesByPaginationFilterSpec<Domain.Configuration.State>
{
    public StatesBySearchRequestDomainSpec(SearchStatesRequest request)
        : base(request) =>
                Query.Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
                     .Where(x => x.Country.DialCode == request.CountryDialCode, request.CountryDialCode.HasValue())
                     .Where(x => x.CountryId == request.CountryId, request.CountryId.HasValue)
                     .OrderBy(c => c.Name, !request.HasOrderBy());
}


public class SearchStatesRequestHandler : IQueryHandler<SearchStatesRequest, PaginatedResult<StateDto>>
{
    private readonly IReadRepository<Domain.Configuration.State> _repository;

    public SearchStatesRequestHandler(IReadRepository<Domain.Configuration.State> repository) => _repository = repository;

    public async Task<PaginatedResult<StateDto>> Handle(SearchStatesRequest request, CancellationToken cancellationToken)
    {
        var spec = new StatesBySearchRequestSpec(request);
        var result = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return result;
    }
}