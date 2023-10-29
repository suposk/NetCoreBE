namespace Contracts.Dtos;

public class PagedListDto<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Results => this.ToList();

    public PagedListDto(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }
}

public class PagedResultDto<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Results { get; }

    public PagedResultDto(PagedListDto<T> values)
    {
        TotalCount = values.TotalCount;
        PageSize = values.PageSize;
        CurrentPage = values.CurrentPage;
        TotalPages = values.TotalPages;
        Results = values.Results;
    }
}