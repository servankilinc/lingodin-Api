using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Core.Utils.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheService> _logger;
    public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }


    public CacheResponse GetFromCache(string cacheKey)
    {
        if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentNullException(cacheKey);

        byte[]? cachedData = _distributedCache.Get(cacheKey);
        if (cachedData != null)
        {
            var response = Encoding.UTF8.GetString(cachedData);
            if (string.IsNullOrEmpty(response)) return new CacheResponse(IsSuccess: false);

            _logger.LogInformation($"CacheService Get, Successfully => key: ({cacheKey}), data: {response}");
            return new CacheResponse(IsSuccess: true, Source: response);
        }
        else
        {
            _logger.LogInformation($"CacheService Get, Couldnt Found => key: ({cacheKey})");
            return new CacheResponse(IsSuccess: false);
        }
    }


    public void AddToCache<TData>(string cacheKey, string[] cacheGroupKeys, TData data)
    {
        if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentNullException(cacheKey);

        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromDays(2),
            AbsoluteExpiration = DateTime.Now.AddDays(6)
        };

        string serializedData = JsonSerializer.Serialize(data);
        byte[]? bytedData = Encoding.UTF8.GetBytes(serializedData);

        _distributedCache.Set(cacheKey, bytedData, cacheEntryOptions);
        _logger.LogInformation($"CacheService Add, Successfully => key: ({cacheKey}), data: {serializedData}");

        if (cacheGroupKeys.Length > 0) AddCacheKeyToGroups(cacheKey, cacheGroupKeys, cacheEntryOptions);
    }


    public void RemoveFromCache(string cacheKey)
    {
        if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentNullException(cacheKey);

        _distributedCache.Remove(cacheKey);
        _logger.LogInformation($"CacheService Remove, Successfully => key: ({cacheKey})");
    }


    public void RemoveCacheGroupKeys(string[] cacheGroupKeyList)
    {
        if (cacheGroupKeyList.Length == 0) throw new ArgumentNullException(nameof(cacheGroupKeyList));

        foreach (string cacheGroupKey in cacheGroupKeyList)
        {
            byte[]? keyListFromCache = _distributedCache.Get(cacheGroupKey);
            _distributedCache.Remove(cacheGroupKey);

            if (keyListFromCache == null)
            {
                _logger.LogInformation($"CacheService Group Remove, Successfully (but not exist any key !!!) => groupKey: ({cacheGroupKey})");
                continue;
            }

            string stringKeyList = Encoding.Default.GetString(keyListFromCache);
            HashSet<string>? keyListInGroup = JsonSerializer.Deserialize<HashSet<string>>(stringKeyList);
            if (keyListInGroup != null)
            {
                foreach (var key in keyListInGroup)
                {
                    _distributedCache.Remove(key);
                }
            }

            _logger.LogInformation($"CacheService Group Remove, Successfully => groupKey: ({cacheGroupKey}) keyList: ({stringKeyList})");
        }
    }


    private void AddCacheKeyToGroups(string cacheKey, string[] cacheGroupKeys, DistributedCacheEntryOptions groupCacheEntryOptions)
    {
        foreach (string cacheGroupKey in cacheGroupKeys)
        {
            HashSet<string>? keyListInGroup;
            byte[]? cachedGroupData = _distributedCache.Get(cacheGroupKey);
            if (cachedGroupData != null)
            {
                keyListInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cachedGroupData));
                if (keyListInGroup != null && !keyListInGroup.Contains(cacheKey))
                {
                    keyListInGroup.Add(cacheKey);
                }
            }
            else
            {
                keyListInGroup = new HashSet<string>(new[] { cacheKey });
            }
            string serializedData = JsonSerializer.Serialize(keyListInGroup);
            byte[]? bytedKeyList = Encoding.UTF8.GetBytes(serializedData); 
            //byte[]? bytedKeyList = JsonSerializer.SerializeToUtf8Bytes(keyListInGroup);

            _distributedCache.Set(cacheGroupKey, bytedKeyList, groupCacheEntryOptions);
            _logger.LogInformation($"CacheService Keylist to Group, Successfully group: ({cacheGroupKey}) new keyList : ({serializedData})");
        }
    }
}