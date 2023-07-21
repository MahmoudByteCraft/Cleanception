using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.State;

public class DeleteStateRequest : ICommand, IDeleteRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteStateRequest(Guid id) => Id = id;
    public DeleteStateRequest() { }
}

public class DeleteStateRequestHandler : ICommandHandler<DeleteStateRequest>
{
    private readonly IRepositoryWithEvents<Domain.Configuration.State> _stateRepo;
    private readonly IStringLocalizer<DeleteStateRequestHandler> _localizer;

    public DeleteStateRequestHandler(IRepositoryWithEvents<Domain.Configuration.State> StateRepo, IStringLocalizer<DeleteStateRequestHandler> localizer) =>
        (_stateRepo, _localizer) = (StateRepo, localizer);

    public async Task<Result<Guid>> Handle(DeleteStateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _stateRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["State Not Found."]);

        await _stateRepo.DeleteAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}