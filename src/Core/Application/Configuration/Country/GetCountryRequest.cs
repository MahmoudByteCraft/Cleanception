using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;

namespace Cleanception.Application.Configuration.Country;

public class GetCountryRequest : IQuery<Result<CountryDto>>
{
    public Guid Id { get; set; }

    public GetCountryRequest(Guid id) => Id = id;
}

public class CountryByIdSpec : Specification<Domain.Configuration.Country, CountryDto>
{
    public CountryByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(CountryByIdSpec), id);
}

public class GetCountryRequestHandler : IQueryHandler<GetCountryRequest, Result<CountryDto>>
{
    private readonly IRepository<Domain.Configuration.Country> _repository;
    private readonly IStringLocalizer<GetCountryRequestHandler> _localizer;

    public GetCountryRequestHandler(IRepository<Domain.Configuration.Country> repository, IStringLocalizer<GetCountryRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<CountryDto>> Handle(GetCountryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CountryByIdSpec(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Country Not Found.", request.Id]);
        return await Result<CountryDto>.SuccessAsync(entity);
    }
}