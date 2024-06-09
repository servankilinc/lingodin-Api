namespace Core.Utils.Pagination;

public class Paginate<TData>
{
    public Paginate()
    {
        Items = Array.Empty<TData>();
    }

    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<TData> Items { get; set; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
}