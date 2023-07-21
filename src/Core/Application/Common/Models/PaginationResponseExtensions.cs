using Cleanception.Application.Common.Interfaces;
using Cleanception.Application.Common.Persistence;
using Cleanception.Domain.Common.Contracts;
using Mapster;

namespace Cleanception.Application.Common.Models;

public static class PaginationResponseExtensions
{
    public static async Task<PaginatedResult<TDestination>> PaginatedListAsync<T, TDestination>(
        this IReadRepositoryBase<T> repository, ISpecification<T, TDestination> spec, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        where T : class
        where TDestination : class, IDto
    {
        var list = await repository.ListAsync(spec, cancellationToken);
        int count = await repository.CountAsync(spec, cancellationToken);

        return PaginatedResult<TDestination>.Success(list, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedResult<TDestination>> PaginatedListAsync<T, TDestination>(
        this IReadRepository<T> repository, ISpecification<T, TDestination> spec, int pageNumber, int pageSize, TypeAdapterConfig? config, CancellationToken cancellationToken = default)
        where T : class, IAggregateRoot
        where TDestination : class, IDto
    {
        var list = await repository.ListAsync(spec, config, cancellationToken);
        int count = await repository.CountAsync(spec, cancellationToken);

        return PaginatedResult<TDestination>.Success(list, count, pageNumber, pageSize);
    }
}