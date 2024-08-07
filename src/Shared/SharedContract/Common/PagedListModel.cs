namespace SharedContract.Common;

public class PagedListModel<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public List<T> Results { get; set; } = new();
    public string? Range { get; set; }
}
