using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.Country;

public class ToggleCountryStatusRequest : ICommand, IActiveRequest<Guid>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class ToggleCountryStatusRequestHandler : ICommandHandler<ToggleCountryStatusRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.Country> _repository;
    private readonly IStringLocalizer<ToggleCountryStatusRequestHandler> _localizer;

    public ToggleCountryStatusRequestHandler(IRepositoryWithEvents<Domain.Configuration.Country> repository, IStringLocalizer<ToggleCountryStatusRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(ToggleCountryStatusRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Country Not Found.", request.Id]);

        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}