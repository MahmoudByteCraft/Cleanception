using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.City;

public class UpdateCityRequest : ICommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? StateId { get; set; }
}

public class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityRequestValidator(IRepository<Domain.Configuration.City> repository, IRepository<Domain.Configuration.State> stateRepository, IStringLocalizer<UpdateCityRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
           .NotEmpty()
           .MaximumLength(255);

        When(x => x.Code.HasValue(), () =>
        {
            RuleFor(p => p.Code)
                .MaximumLength(20)
                .MustAsync(async (request, field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.City>(x => x.Code == field && x.Id != request.Id), ct))
                   .WithMessage((_, field) => localizer["City code already exist.", field]);
        });

        RuleFor(p => p.StateId)
           .NotEmpty()
           .MustAsync(async (field, ct) => await stateRepository.AnyAsync(new ExpressionSpecification<Domain.Configuration.State>(x => x.Id == field), ct))
               .WithMessage((_, field) => localizer["State not found.", field!]);

        When(x => x.StateId.HasValue && x.Name.HasValue(), () =>
        {
            RuleFor(p => p)
               .NotEmpty()
               .MustAsync(async (field, request, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.City>(x => x.Name == request.Name && x.StateId == request.StateId && x.Id != request.Id), ct))
                   .WithMessage((_, field) => localizer["City already exist.", field!]);
        });
    }
}

public class UpdateCityRequestHandler : ICommandHandler<UpdateCityRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.City> _repository;
    private readonly IStringLocalizer<UpdateCityRequestHandler> _localizer;

    public UpdateCityRequestHandler(IRepositoryWithEvents<Domain.Configuration.City> repository, IStringLocalizer<UpdateCityRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateCityRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["City Not Found.", request.Id]);

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Longitude = request.Longitude;
        entity.Latitude = request.Latitude;
        entity.StateId = request.StateId!.Value;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}