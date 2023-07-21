using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.City;

public class CreateCityRequest : ICommand
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? StateId { get; set; }
}

public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator(IReadRepository<Domain.Configuration.City> repository, IReadRepository<Domain.Configuration.State> stateRepository, IStringLocalizer<CreateCityRequestValidator> localizer)
    {

        RuleFor(p => p.Name)
           .NotEmpty()
           .MaximumLength(255);

        When(x => x.Code.HasValue(), () =>
        {
            RuleFor(p => p.Code)
               .MaximumLength(50)
               .MustAsync(async (field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.City>(x => x.Code == field), ct))
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
               .MustAsync(async (field, request, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.City>(x => x.Name == request.Name && x.StateId == request.StateId), ct))
                   .WithMessage((_, field) => localizer["City already exist.", field!]);
        });
    }
}

public class CreateCityRequestHandler : ICommandHandler<CreateCityRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.City> _repository;

    public CreateCityRequestHandler(IRepositoryWithEvents<Domain.Configuration.City> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateCityRequest request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Configuration.City
        {
            Name = request.Name,
            Code = request.Code,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            IsActive = request.IsActive,
            StateId = request.StateId!.Value,
        };

        await _repository.AddAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }
}