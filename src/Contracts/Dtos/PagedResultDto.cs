namespace Contracts.Dtos;

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