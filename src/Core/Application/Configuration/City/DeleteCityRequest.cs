using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.City;

public class DeleteCityRequest : ICommand, IDeleteRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteCityRequest(Guid id) => Id = id;

    public DeleteCityRequest() { }
}

public class DeleteCityRequestHandler : ICommandHandler<DeleteCityRequest>
{
    private readonly IRepositoryWithEvents<Domain.Configuration.City> _cityRepo;
    private readonly IStringLocalizer<DeleteCityRequestHandler> _localizer;

    public DeleteCityRequestHandler(IRepositoryWithEvents<Domain.Configuration.City> CityRepo, IStringLocalizer<DeleteCityRequestHandler> localizer) =>
        (_cityRepo, _localizer) = (CityRepo, localizer);

    public async Task<Result<Guid>> Handle(DeleteCityRequest request, CancellationToken cancellationToken)
    {
        var entity = await _cityRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["City Not Found."]);

        await _cityRepo.DeleteAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}