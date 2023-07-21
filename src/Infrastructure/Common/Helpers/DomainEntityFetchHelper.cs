using System.Linq.Expressions;
using Cleanception.Application.Common.Interfaces;
using Cleanception.Domain.Common.Contracts;
using Cleanception.Infrastructure.Identity;
using Cleanception.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cleanception.Infrastructure.Common.Helpers;
public class DomainEntityFetchHelper : ITransientService
{
    private readonly ApplicationDbContext _context;

    public DomainEntityFetchHelper(ApplicationDbContext context)
    {
        _context = context;
    }

    public async IAsyncEnumerable<IEnumerable<T?>> FetchDomainEntityAsChunk<T>(Expression<Func<T, bool>> expression, bool asNoTracking = true, int chunkSize = 30)
        where T : AuditableEntity
    {
        if(chunkSize > 300)
        {
            chunkSize = 300;
        }

        int startIndex = 0;

        var baseQuery = _context.Set<T>();

        if (asNoTracking)
        {
            baseQuery.AsNoTracking();
        }

        while (true)
        {
            var result = await baseQuery.Where(expression).Skip(startIndex).Take(chunkSize).ToListAsync();
            if (result.Count() != 0)
            {
                yield return result;
                startIndex += result.Count();
            }
            else
            {
                break;
            }
        }
    }

    public async IAsyncEnumerable<IEnumerable<ApplicationUser?>> FetchUsersAsChunk(Expression<Func<ApplicationUser, bool>> expression, bool asNoTracking = true, int chunkSize = 30)
    {
        if (chunkSize > 300)
        {
            chunkSize = 300;
        }

        int startIndex = 0;

        var baseQuery = _context.Users;

        if (asNoTracking)
        {
            baseQuery.AsNoTracking();
        }

        while (true)
        {
            var result = await baseQuery.Where(expression).Skip(startIndex).Take(chunkSize).ToListAsync();
            if (result.Count() != 0)
            {
                yield return result;
                startIndex += result.Count();
            }
            else
            {
                break;
            }
        }
    }
}