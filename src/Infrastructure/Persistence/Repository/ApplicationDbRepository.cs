using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Persistence.Repository;

// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public ApplicationDbRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    // This is only done when no Selector is defined, so regular specifications with a selector also still work.

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();

    protected IQueryable<TResult> ApplyConfigSpecification<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig? config = null) =>
    specification.Selector is not null
        ? base.ApplySpecification(specification)
        : ApplySpecification(specification, false)
            .ProjectToType<TResult>(config);

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig? config = null, CancellationToken cancellationToken = default)
    {
        var queryResult = specification.Selector is not null ? await base.ApplySpecification(specification).ToListAsync(cancellationToken) : await ApplySpecification(specification, false).ProjectToType<TResult>(config).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null ? queryResult : specification.PostProcessingAction(queryResult).ToList();
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, TypeAdapterConfig? config, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, false).ProjectToType<TResult>(config).FirstOrDefaultAsync(cancellationToken);
    }
}