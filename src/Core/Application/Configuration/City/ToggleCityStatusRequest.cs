using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.City;

public class ToggleCityStatusRequest : ICommand, IActiveRequest<Guid>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class ToggleCityStatusRequestHandler : ICommandHandler<ToggleCityStatusRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.City> _repository;
    private readonly IStringLocalizer<ToggleCityStatusRequestHandler> _localizer;

    public ToggleCityStatusRequestHandler(IRepositoryWithEvents<Domain.Configuration.City> repository, IStringLocalizer<ToggleCityStatusRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(ToggleCityStatusRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["City Not Found.", request.Id]);

        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}