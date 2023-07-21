using Cleanception.Application.Common.Caching;

namespace Cleanception.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{

    public CacheKeyService()
    {

    }

    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}