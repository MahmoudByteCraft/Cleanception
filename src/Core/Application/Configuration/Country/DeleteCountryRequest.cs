using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Shared;

namespace Cleanception.Application.Configuration.Country;

public class DeleteCountryRequest : ICommand, IDeleteRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteCountryRequest(Guid id) => Id = id;

    public DeleteCountryRequest() { }
}

public class DeleteCountryRequestHandler : ICommandHandler<DeleteCountryRequest>
{
    private readonly IRepositoryWithEvents<Domain.Configuration.Country> _countryRepo;
    private readonly IStringLocalizer<DeleteCountryRequestHandler> _localizer;

    public DeleteCountryRequestHandler(IRepositoryWithEvents<Domain.Configuration.Country> CountryRepo, IStringLocalizer<DeleteCountryRequestHandler> localizer) =>
        (_countryRepo, _localizer) = (CountryRepo, localizer);

    public async Task<Result<Guid>> Handle(DeleteCountryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _countryRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Country Not Found."]);

        await _countryRepo.DeleteAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}