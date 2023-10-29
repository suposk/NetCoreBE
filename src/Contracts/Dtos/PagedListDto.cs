namespace Contracts.Dtos;

public class PagedListDto<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedListDto(List<T> items, int totalPages, int currentPage, int pageSize)
    {
        TotalCount = totalPages;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalPages / (double)pageSize);
        AddRange(items);
    }
}
