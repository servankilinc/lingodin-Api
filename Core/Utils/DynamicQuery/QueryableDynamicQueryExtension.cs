using System.Linq.Dynamic.Core;
using System.Text;

namespace Core.Utils.DynamicQuery;

public static class QueryableDynamicQueryExtension
{
    private static readonly string[] _orderDirs = { "asc", "desc" };
    private static readonly string[] _logics = { "and", "or" };
    private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
        {
            { "base", " " },
            { "eq", "=" },
            { "neq", "!=" },
            { "lt", "<" },
            { "lte", "<=" },
            { "gt", ">" },
            { "gte", ">=" },
            { "isnull", "== null" },
            { "isnotnull", "!= null" },
            { "startswith", "StartsWith" },
            { "endswith", "EndsWith" },
            { "contains", "Contains" },
            { "doesnotcontain", "Contains" }
        };

    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
    {
        if (dynamicQuery == null) return query;
        if (dynamicQuery.Filter is not null) query = Filter(query, dynamicQuery.Filter);
        if (dynamicQuery.Sort is not null && dynamicQuery.Sort.Any()) query = Sort(query, dynamicQuery.Sort);
        return query;
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        List<Filter> filterList = new();
        GetFilters(filterList, filter);

        foreach (Filter item in filterList) // Validation before processes
        {
            if (item.Operator == "base" && _logics.Contains(item.Logic)) continue;
            if (string.IsNullOrEmpty(item.Field))
                throw new ArgumentException("Empty Field For Filter Process");
            if (string.IsNullOrEmpty(item.Operator) || !_operators.ContainsKey(item.Operator))
                throw new ArgumentException("Invalid Opreator Type For Filter Process");
            if (string.IsNullOrEmpty(item.Value) && (item.Operator == "isnull" || item.Operator == "isnotnull")) // those operators do not need value
                throw new ArgumentException("Invalid Value For Filter Process");
            if (string.IsNullOrEmpty(item.Logic) == false && _logics.Contains(item.Logic) == false)
                throw new ArgumentException("Invalid Logic Type For Filter Process");
        }

        string?[] values = filterList.Select(f => f.Value).ToArray();
        string where = Transform(filter, filterList);

        if (!string.IsNullOrWhiteSpace(where)) 
            queryable = queryable.Where(where, values);

        return queryable;
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        if (sort.Any())
        {
            foreach (Sort item in sort)
            {
                if (string.IsNullOrEmpty(item.Field))
                    throw new ArgumentException("Empty Field For Sorting Process");
                if (string.IsNullOrEmpty(item.Dir) || !_orderDirs.Contains(item.Dir))
                    throw new ArgumentException("Invalid Order Type For Sorting Process");
            }

            string ordering = string.Join(separator: ",", values: sort.Select(s => $"{s.Field} {s.Dir}"));
            return queryable.OrderBy(ordering);
        }

        return queryable;
    }

    private static void GetFilters(IList<Filter> filterList, Filter filter)
    {
        filterList.Add(filter);
        if (filter.Filters is not null && filter.Filters.Any())
            foreach (Filter item in filter.Filters)
                GetFilters(filterList, item);
    } 
    
    public static string Transform(Filter filter, IList<Filter> filters)
    {
        int index = filters.IndexOf(filter);
        string comparison = _operators[filter.Operator!];
        StringBuilder where = new();

        switch (filter.Operator)
        {
            case "base":
                where.Append($" ");
                break;
            case "eq":
                where.Append($"np({filter.Field}) == @{index}");
                break;
            case "neq":
                where.Append($"np({filter.Field}) != @{index}");
                break;
            case "lt":
                where.Append($"np({filter.Field}) < @{index}");
                break;
            case "lte":
                where.Append($"np({filter.Field}) <= @{index}");
                break;
            case "gt":
                where.Append($"np({filter.Field}) > @{index}");
                break;
            case "gte":
                where.Append($"np({filter.Field}) >= @{index}");
                break;
            case "isnull":
                where.Append($"np({filter.Field}) == null");
                break;
            case "isnotnull":
                where.Append($"np({filter.Field}) != null");
                break;
            case "startswith":
                where.Append($"np({filter.Field}).StartsWith(@{index})");
                break;
            case "endswith":
                where.Append($"np({filter.Field}).EndsWith(@{index})");
                break;
            case "contains":
                where.Append($"np({filter.Field}).Contains(@{index})");
                break;
            case "doesnotcontain":
                where.Append($"!np({filter.Field}).Contains(@{index})");
                break;
            default:
                throw new ArgumentException($"Invalid Operator Type For Filter Process ({filter.Operator})");
        }
         


        if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
        {
            string baseLogic = filter.Operator == "base" ? "" : filter.Logic;
            return $"({where} {baseLogic} {string.Join(separator: $" {filter.Logic} ", value: filter.Filters.Select(f => Transform(f, filters)).ToArray())})";
        }

        return where.ToString();
    }
}
