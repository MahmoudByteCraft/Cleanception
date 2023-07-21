using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.State;

public class UpdateStateRequest : ICommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public Guid? CountryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateStateRequestValidator : AbstractValidator<UpdateStateRequest>
{
    public UpdateStateRequestValidator(IRepository<Domain.Configuration.State> repository, IRepository<Domain.Configuration.Country> countryRepository, IStringLocalizer<UpdateStateRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
           .NotEmpty()
           .MaximumLength(255);

        RuleFor(p => p.Code)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MaximumLength(20)
           .MustAsync(async (request, field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.State>(x => x.Code == field && x.Id != request.Id), ct))
               .WithMessage((_, field) => localizer["State code already exist.", field]);

        RuleFor(p => p.CountryId)
           .NotEmpty()
           .MustAsync(async (field, ct) => await countryRepository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Id == field), ct))
               .WithMessage((_, field) => localizer["Country not found.", field!]);

        When(x => x.CountryId.HasValue && x.Name.HasValue(), () =>
        {
            RuleFor(p => p)
               .NotEmpty()
               .MustAsync(async (field, request, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.State>(x => x.Name == request.Name && x.CountryId == request.CountryId && x.Id != request.Id), ct))
                   .WithMessage((_, field) => localizer["State already exist.", field!]);
        });
    }
}

public class UpdateStateRequestHandler : ICommandHandler<UpdateStateRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.State> _repository;
    private readonly IStringLocalizer<UpdateStateRequestHandler> _localizer;

    public UpdateStateRequestHandler(IRepositoryWithEvents<Domain.Configuration.State> repository, IStringLocalizer<UpdateStateRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateStateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["State Not Found.", request.Id]);

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Longitude = request.Longitude;
        entity.Latitude = request.Latitude;
        entity.IsActive = request.IsActive;
        entity.CountryId = request.CountryId!.Value;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}