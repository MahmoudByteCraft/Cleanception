using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;

namespace Cleanception.Application.Configuration.City;

public class GetCityRequest : IQuery<Result<CityDto>>
{
    public Guid Id { get; set; }

    public GetCityRequest(Guid id) => Id = id;
}

public class CityByIdSpec : Specification<Domain.Configuration.City, CityDto>
{
    public CityByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(CityByIdSpec), id);
}

public class GetCityRequestHandler : IQueryHandler<GetCityRequest, Result<CityDto>>
{
    private readonly IRepository<Domain.Configuration.City> _repository;
    private readonly IStringLocalizer<GetCityRequestHandler> _localizer;

    public GetCityRequestHandler(IRepository<Domain.Configuration.City> repository, IStringLocalizer<GetCityRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<CityDto>> Handle(GetCityRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CityByIdSpec(request.Id), cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["City Not Found.", request.Id]);

        return await Result<CityDto>.SuccessAsync(entity);
    }
}