using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;

namespace Cleanception.Application.Configuration.State;

public class GetStateRequest : IQuery<Result<StateDto>>
{
    public Guid Id { get; set; }

    public GetStateRequest(Guid id) => Id = id;
}

public class StateByIdSpec : Specification<Domain.Configuration.State, StateDto>
{
    public StateByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id).EnableCache(nameof(StateByIdSpec), id);
}

public class GetStateRequestHandler : IQueryHandler<GetStateRequest, Result<StateDto>>
{
    private readonly IRepository<Domain.Configuration.State> _repository;
    private readonly IStringLocalizer<GetStateRequestHandler> _localizer;

    public GetStateRequestHandler(IRepository<Domain.Configuration.State> repository, IStringLocalizer<GetStateRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<StateDto>> Handle(GetStateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new StateByIdSpec(request.Id), cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["State Not Found.", request.Id]);

        return await Result<StateDto>.SuccessAsync(entity);
    }
}