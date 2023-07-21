using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.State;

public class ToggleStateStatusRequest : ICommand, IActiveRequest<Guid>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class ToggleStateStatusRequestHandler : ICommandHandler<ToggleStateStatusRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.State> _repository;
    private readonly IStringLocalizer<ToggleStateStatusRequestHandler> _localizer;

    public ToggleStateStatusRequestHandler(IRepositoryWithEvents<Domain.Configuration.State> repository, IStringLocalizer<ToggleStateStatusRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(ToggleStateStatusRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["State Not Found.", request.Id]);

        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}