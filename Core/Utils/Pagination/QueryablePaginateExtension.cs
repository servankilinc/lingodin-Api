using Microsoft.EntityFrameworkCore;

namespace Core.Utils.Pagination;

public static class QueryablePaginateExtension
{
    public static Paginate<TData> ToPaginate<TData>(this IQueryable<TData> data, int index = default, int size = default)
    {
        int count = data.Count();

        if (index == default || index < 0) index = 0;
        if (size == default || size <= 0) size = count;

        List<TData> items = data.Skip(index * size).Take(size).ToList();
        Paginate<TData> list = new()
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (count <= 0 || size <= 0) ? 0 : (int)Math.Ceiling(count / (double)size)
        };
        return list;
    }


    public static async Task<Paginate<TData>> ToPaginateAsync<TData>(this IQueryable<TData> data, int index = default, int size = default, CancellationToken cancellationToken = default)
    {
        int count = await data.CountAsync(cancellationToken).ConfigureAwait(false);

        if (index == default || index < 0) index = 0;
        if (size == default || size <= 0) size = count;

        List<TData> items = await data.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);
        Paginate<TData> list = new()
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
            Pages = (count <= 0 || size <= 0) ? 0 : (int)Math.Ceiling(count / (double)size)
        };
        return list;
    }
}
