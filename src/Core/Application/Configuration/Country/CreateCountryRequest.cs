using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Domain.Enums;

namespace Cleanception.Application.Configuration.Country;

public class CreateCountryRequest : ICommand
{
    public string? Name { get; set; }
    public string? DialCode { get; set; }
    public string? Code { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? Flag { get; set; }
    public bool IsActive { get; set; } = true;
    public ContinentType ContinentType { get; set; }
}

public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    public CreateCountryRequestValidator(IReadRepository<Domain.Configuration.Country> repository, IStringLocalizer<CreateCountryRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MaximumLength(255)
           .MustAsync(async (field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Name == field), ct))
               .WithMessage((_, field) => localizer["Country name already exist.", field]);

        RuleFor(p => p.Code)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MaximumLength(20)
          .MustAsync(async (field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Code == field), ct))
              .WithMessage((_, field) => localizer["Country code already exist.", field]);

        RuleFor(p => p.DialCode)
           .NotEmpty()
           .MaximumLength(20)
           .MustAsync(async (field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.DialCode == field), ct))
               .WithMessage((_, field) => localizer["Country dial code already exist.", field]);
    }
}

public class CreateCountryRequestHandler : ICommandHandler<CreateCountryRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.Country> _repository;

    public CreateCountryRequestHandler(IRepositoryWithEvents<Domain.Configuration.Country> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateCountryRequest request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Configuration.Country
        {
            Name = request.Name,
            DialCode = request.DialCode,
            Code = request.Code,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            Flag = request.Flag,
            ContinentType = request.ContinentType,
            IsActive = request.IsActive,
        };

        await _repository.AddAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }
}