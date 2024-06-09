namespace Core.Utils.Pagination;

public class PagingRequest
{
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 20;

    public PagingRequest()
    {
    }

    public PagingRequest(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
