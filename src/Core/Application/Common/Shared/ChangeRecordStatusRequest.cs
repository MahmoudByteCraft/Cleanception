using System.Text.Json;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Application.Configuration.City;
using Cleanception.Application.Configuration.Country;
using Cleanception.Application.Configuration.State;
using Cleanception.Application.Identity.Users;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Domain.Configuration;
using Cleanception.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Cleanception.Application.Common.Shared;

public class ChangeRecordStatusRequest : ICommand<Result<int>>
{
    public EntityToChange EntityToChange { get; set; }
    public OperationType OperationType { get; set; }
    public object? Filter { get; set; }
    public List<Guid>? Ids { get; set; }
}

public class ChangeRecordStatusRequestHandler : ICommandHandler<ChangeRecordStatusRequest, Result<int>>
{
    public readonly IServiceProvider _serviceProvider;
    public readonly IMediator _mediator;
    public readonly IUserService _userService;

    public ChangeRecordStatusRequestHandler(IServiceProvider serviceProvider, IMediator mediator, IUserService userService)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _userService = userService;
    }

    public async Task<Result<int>> Handle(ChangeRecordStatusRequest request, CancellationToken cancellationToken)
    {
        switch (request.EntityToChange)
        {
            case EntityToChange.Country:
                return await SetStatus<Country, CountriesBySearchRequestDomainSpec, ToggleCountryStatusRequest, DeleteCountryRequest>(EntityToChange.Country, request.OperationType, request.Filter != null ? new CountriesBySearchRequestDomainSpec(JsonConvert.DeserializeObject<SearchCountriesRequest>(((JsonElement)request!.Filter!).GetRawText())) : null, request.Ids, cancellationToken);
            case EntityToChange.State:
                return await SetStatus<State, StatesBySearchRequestDomainSpec, ToggleStateStatusRequest, DeleteStateRequest>(EntityToChange.State, request.OperationType, request.Filter != null ? new StatesBySearchRequestDomainSpec(JsonConvert.DeserializeObject<SearchStatesRequest>(((JsonElement)request!.Filter!).GetRawText())) : null, request.Ids, cancellationToken);
            case EntityToChange.City:
                return await SetStatus<City, CitiesBySearchRequestDomainSpec, ToggleCityStatusRequest, DeleteCityRequest>(EntityToChange.City, request.OperationType, request.Filter != null ? new CitiesBySearchRequestDomainSpec(JsonConvert.DeserializeObject<SearchCitiesRequest>(((JsonElement)request!.Filter!).GetRawText())) : null, request.Ids, cancellationToken);
        }

        return await Result<int>.SuccessAsync(0);
    }

    private async Task<Result<int>> SetStatus<TYPE, TSpec, TActiveRequestType, TDeleteRequestType>(EntityToChange entityToChange, OperationType statusToChange, TSpec? spec, List<Guid>? ids, CancellationToken cancellationToken)
        where TYPE : BaseEntity<Guid>, IAggregateRoot
        where TSpec : class, ISpecification<TYPE>
        where TActiveRequestType : class, IActiveRequest<Guid>, new()
        where TDeleteRequestType : class, IDeleteRequest<Guid>, new()
    {
        var repo = _serviceProvider.GetService<IRepository<TYPE>>();

        List<TYPE>? result = null;
        if (spec != null)
        {
            result = await repo!.ListAsync(spec, cancellationToken);
        }
        else if (ids != null)
        {
            result = await repo.ListAsync(new ExpressionSpecification<TYPE>(x => ids.Contains(x.Id)), cancellationToken);
        }

        if (statusToChange == OperationType.SetAsActive || statusToChange == OperationType.SetAsInActive)
        {
            foreach (var item in result)
            {
                var temp = new TActiveRequestType
                {
                    Id = item.Id,
                    IsActive = statusToChange == OperationType.SetAsActive ? true : false
                };

                await _mediator.Send(temp);
            }
        }
        else if(statusToChange == OperationType.SetAsDeleted)
        {
            foreach (var item in result)
            {
                var temp = new TDeleteRequestType
                {
                    Id = item.Id
                };

                await _mediator.Send(temp);
            }
        }

        return await Result<int>.SuccessAsync(result!.Count);
    }
}
