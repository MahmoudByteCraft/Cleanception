namespace Cleanception.Application.Common.Models;

public class PaginationFilter : BaseFilter
{
    public PaginationFilter()
    {
        if (PageSize < 0 || PageSize > 30)
            PageSize = 30;
    }

    protected int _pageSize = 30;

    public int PageNumber { get; set; }

    public virtual int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value < 0 || value > 30)
                _pageSize = 30;
            else
                _pageSize = value;
        }
    }

    public string[]? OrderBy { get; set; }
    public void SetPageSize(int pageSize)
    {
        _pageSize = pageSize;
    }
}

public static class PaginationFilterExtensions
{
    public static bool HasOrderBy(this PaginationFilter filter) =>
        filter.OrderBy?.Any() is true;
}