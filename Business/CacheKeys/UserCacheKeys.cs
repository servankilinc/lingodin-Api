namespace Business.CacheKeys;

public static class UserCacheKeys
{
    public static string UserGroup => "User-Cache-Group-Key";
    public static string UserByDetailGroup => "User-Cache-Detail-Group-Key";
    public static string AllUserList => "AllUserListCacheKey";
    public static string UserInfoById(Guid userId) => $"UserInfoByIdCacheKey-{userId}";
    public static string UserInfoByMail(string mail) => $"UserInfoByMailCacheKey-{mail}";
    public static string UserDetailByEmail(string mail) => $"UserDetailByEmailCacheKey-{mail}";
    public static string UserDetailById(Guid userId) => $"UserDetailByIdCacheKey-{userId}";
    public static string IsUserExistByEmail(string mail) => $"IsUserExistByEmailCacheKey-{mail}";
    public static string PaginatedUserList(int pageIndex, int pageSize) => $"PaginatedUserListCacheKey-{pageIndex}-{pageSize}";
}