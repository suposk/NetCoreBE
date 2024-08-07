namespace SharedContract.Common;

public class SearchParameters
{
    /// <summary>
    /// 50, may be ovveriden for larger sets
    /// </summary>
    public virtual int MaxPageSize => 50;

    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public int CurrentPage { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Defualt is CreatedAt asc.
    /// Add desc for descending order example: CreatedAt desc
    /// </summary>   
    public virtual string OrderBy { get; set; } = $"CreatedAt"; //asc

    /// <summary>
    /// Defualt is CreatedAt desc
    /// </summary>
    //public virtual string OrderBy { get; set; } = $"CreatedAt desc"; //desc 
}
