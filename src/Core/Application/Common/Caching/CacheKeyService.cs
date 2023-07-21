﻿using Cleanception.Application.Common.Interfaces;

namespace Cleanception.Application.Common.Caching;

public interface ICacheKeyService : IScopedService
{
    public string GetCacheKey(string name, object id);
}