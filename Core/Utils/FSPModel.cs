using Core.Utils.Pagination;

namespace Core.Utils;

public class FSPModel
{
    public PagingRequest? PagingRequest { get; set; }
    public DynamicQuery.DynamicQuery? DynamicQuery { get; set; }
}
