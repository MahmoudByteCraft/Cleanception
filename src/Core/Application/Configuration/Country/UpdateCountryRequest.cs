using Cleanception.Application.Common.Exceptions;
using Cleanception.Application.Common.Messaging;
using Cleanception.Application.Common.Models;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Domain.Enums;

namespace Cleanception.Application.Configuration.Country;

public class UpdateCountryRequest : ICommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DialCode { get; set; }
    public string? Code { get; set; }
    public string? Flag { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public ContinentType ContinentType { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
{
    public UpdateCountryRequestValidator(IRepository<Domain.Configuration.Country> repository, IStringLocalizer<UpdateCountryRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MaximumLength(255)
           .MustAsync(async (request, field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Name == field && x.Id != request.Id), ct))
               .WithMessage((_, field) => localizer["Country name already exist.", field]);

        RuleFor(p => p.DialCode)
           .NotEmpty()
           .MaximumLength(20)
           .MustAsync(async (request, field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.DialCode == field && x.Id != request.Id), ct))
               .WithMessage((_, field) => localizer["Country dial code already exist.", field]);

        RuleFor(p => p.Code)
          .NotEmpty()
          .MaximumLength(20)
          .MustAsync(async (request, field, ct) => !await repository.AnyAsync(new ExpressionSpecification<Domain.Configuration.Country>(x => x.Code == field && x.Id != request.Id), ct))
              .WithMessage((_, field) => localizer["Country code already exist.", field]);
    }
}

public class UpdateCountryRequestHandler : ICommandHandler<UpdateCountryRequest>
{

    private readonly IRepositoryWithEvents<Domain.Configuration.Country> _repository;
    private readonly IStringLocalizer<UpdateCountryRequestHandler> _localizer;

    public UpdateCountryRequestHandler(IRepositoryWithEvents<Domain.Configuration.Country> repository, IStringLocalizer<UpdateCountryRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateCountryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = entity ?? throw new NotFoundException(_localizer["Country Not Found.", request.Id]);

        entity.Name = request.Name;
        entity.DialCode = request.DialCode;
        entity.Longitude = request.Longitude;
        entity.Latitude = request.Latitude;
        entity.Code = request.Code;
        entity.Flag = request.Flag;
        entity.ContinentType = request.ContinentType;
        entity.IsActive = request.IsActive;

        await _repository.UpdateAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(request.Id);
    }
}