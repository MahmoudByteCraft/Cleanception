using Cleanception.Application.Common.Extensions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;

namespace Cleanception.Application.Configuration.State;

public class CreateStateRequest : ICommand
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? DialCode { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CountryId { get; set; }
}

public class CreateStateRequestValidator : AbstractValidator<CreateStateRequest>
{
    public CreateStateRequestValidator(IReadRepository<Domain.Configuration.State> repository, IReadRepository<Domain.Configuration.Country> countryRepository, IStringLocalizer<CreateStateRequestValidator> localizer)
    {

        RuleFor(p => p.Name)
           .NotEmpty()
           .MaximumLength(255);

        RuleFor(p => p.Code)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MaximumLength(20)
           .MustAsync(async (field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.State>(x => x.Code == field), ct))
               .WithMessage((_, field) => localizer["State code already exist.", field]);

        RuleFor(p => p.CountryId)
           .NotEmpty()
           .MustAsync(async (field, ct) => await countryRepository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Id == field), ct))
               .WithMessage((_, field) => localizer["Country not found.", field!]);

        When(x => x.CountryId.HasValue && x.Name.HasValue(), () =>
        {
            RuleFor(p => p)
               .NotEmpty()
               .MustAsync(async (field, request, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.State>(x => x.Name == request.Name && x.CountryId == request.CountryId), ct))
                   .WithMessage((_, field) => localizer["State already exist.", field!]);
        });
    }
}

public class CreateStateRequestHandler : ICommandHandler<CreateStateRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.State> _repository;

    public CreateStateRequestHandler(IRepositoryWithEvents<Domain.Configuration.State> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateStateRequest request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Configuration.State
        {
            Name = request.Name,
            Code = request.Code,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            DialCode = request.DialCode,
            IsActive = request.IsActive,
            CountryId = request.CountryId!.Value
        };

        await _repository.AddAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }
}