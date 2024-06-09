namespace Business.CacheKeys;
public static class WordCacheKeys
{
    public static string WordGroup => "Word-Cache-Group-Key";
    public static string WordInfoById => "WordInfoByIdCacheKey";
    public static string AllWordList => "AllWordListCacheKey";
    public static string WordListByCategory(Guid categoryId) => $"WordListByCategoryCacheKey-{categoryId}";
    public static string WordUserGroup(Guid userId) => $"WordListForUserBaseCacheGroupKey-{userId}";
    public static string WordListByCategoryForUser(Guid categoryId, Guid userId) => $"WordListByCategoryForUserCacheKey-{categoryId}-{userId}";
    public static string FavoriteWordListForUser(Guid userId) => $"FavoriteWordListForUserCacheKey-{userId}";
    public static string LearnedWordListForUser(Guid userId) => $"LearnedWordListForUserCacheKey-{userId}";
}