namespace Business.CacheKeys;

public static class CategoryCacheKeys
{
    public static string CategoryGroup => "Category-Cache-Group-Key";
    public static string CategoryUserGroup => "Category-User-Cache-Group-Key"; // word insert ve delete işlemlerinde istatistik bilgilerini revize etmek için...
    public static string CategoryUserGroupByUserId(Guid userId) => $"CategoryUserCacheGroupKeyByUserId-{userId}"; // word learn işlemlerinde sadece iligili user gurubunu silmek için...
    public static string CategoryInfoForUser(Guid categoryId, Guid userId) => $"CategoryInfoForUserCacheKey-{categoryId}-{userId}"; // word learn işlemlerinde silinecek gurupta bu key'ler bulunacak
    public static string AllCategoryListForUser(Guid userId) => $"AllCategoryListForUserCacheKey-{userId}";
    public static string CategoryInfoById(Guid categoryId) => $"CategoryInfoByIdCacheKey-{categoryId}";
    public static string AllCategoryList => "AllCategoryListCacheKey";
}