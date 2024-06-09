namespace Core.Utils.Caching;

public interface ICacheService
{
    CacheResponse GetFromCache(string cacheKey);
    void AddToCache<TData>(string CacheKey, string[] CacheGroupKeys, TData data);
    void RemoveFromCache(string CacheKey);
    void RemoveCacheGroupKeys(string[] cacheGroupKeys);
}