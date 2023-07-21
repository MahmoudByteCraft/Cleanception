using System.Linq.Expressions;
using Ardalis.Specification;
using Cleanception.Application.Common.Persistence;
using Cleanception.Application.Common.Specification;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Domain.Common.Events;
using Cleanception.Infrastructure.Persistence.Context;

namespace Cleanception.Infrastructure.Persistence.Repository;

/// <summary>
/// The repository that implements IRepositoryWithEvents.
/// Implemented as a decorator. It only augments the Add,
/// Update and Delete calls where it adds the respective
/// EntityCreated, EntityUpdated or EntityDeleted event
/// before delegating to the decorated repository.
/// </summary>
public class EventAddingRepositoryDecorator<T> : IRepositoryWithEvents<T>
    where T : class, IAggregateRoot
{
    private readonly IRepository<T> _decorated;
    private readonly ApplicationDbContext _dbContext;

    public EventAddingRepositoryDecorator(ApplicationDbContext dbContext, IRepository<T> decorated)
    {
        _dbContext = dbContext;
        _decorated = decorated;


    }

    #region Commands
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        return _decorated.AddAsync(entity, cancellationToken);
    }

    public async Task<T> AddAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
        return entity;
    }

    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        return _decorated.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.DomainEvents.Add(EntityCreatedEvent.WithEntity(entity));
        await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
        return entities;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        return _decorated.UpdateAsync(entity, cancellationToken);
    }

    public async Task<T> UpdateAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        _dbContext.Set<T>().Update(entity);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
        return entity;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        return _decorated.UpdateRangeAsync(entities, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.DomainEvents.Add(EntityUpdatedEvent.WithEntity(entity));
        _dbContext.Set<T>().UpdateRange(entities);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        return _decorated.DeleteAsync(entity, cancellationToken);
    }

    public async Task<T> DeleteAsync(T entity, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        _dbContext.Set<T>().Remove(entity);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
        return entity;
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        }

        return _decorated.DeleteRangeAsync(entities, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool withSaveChanges = true, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.DomainEvents.Add(EntityDeletedEvent.WithEntity(entity));
        }

        _dbContext.Set<T>().RemoveRange(entities);
        if (withSaveChanges)
            await _dbContext.SaveChangesAsync(withTracking, cancellationToken: cancellationToken);
    }
    #endregion

    // The rest of the methods are simply forwarded.
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _decorated.SaveChangesAsync(cancellationToken);

    public Task<int> SaveChangesAsync(bool withAuditing, CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(withAuditing, cancellationToken);

    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull =>
        _decorated.GetByIdAsync(id, cancellationToken);

    [Obsolete]
    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
        _decorated.GetBySpecAsync(specification, cancellationToken);

    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default) =>
        _decorated.ListAsync(cancellationToken);
    public Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.ListAsync(specification, cancellationToken);
    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
        _decorated.ListAsync(specification, cancellationToken);
    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.AnyAsync(specification, cancellationToken);
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _decorated.AnyAsync(cancellationToken);
    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        _decorated.CountAsync(specification, cancellationToken);
    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        _decorated.CountAsync(cancellationToken);

    [Obsolete]
    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
            _decorated.GetBySpecAsync(specification, cancellationToken);

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
            _decorated.FirstOrDefaultAsync(specification, cancellationToken);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
            _decorated.FirstOrDefaultAsync(specification, cancellationToken);

    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default) =>
            _decorated.SingleOrDefaultAsync(specification, cancellationToken);

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
            _decorated.SingleOrDefaultAsync(specification, cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
            _decorated.AnyAsync(new ExpressionSpecification<T>(expression), cancellationToken);

    public Task<T?> FirstByConditionAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default) =>
            _decorated.FirstOrDefaultAsync(new ExpressionSpecification<T>(expression, orderExpression), cancellationToken);

    public Task<TResult?> FirstByConditionAsync<TResult>(Expression<Func<T, bool>> expression, Expression<Func<T, object?>>? orderExpression = null, CancellationToken cancellationToken = default) =>
        _decorated.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<T, TResult>(expression, orderExpression), cancellationToken);

    public Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
           _decorated.CountAsync(new ExpressionSpecification<T>(expression), cancellationToken);

    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification) => _decorated.AsAsyncEnumerable(specification);

}