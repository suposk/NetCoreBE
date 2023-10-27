namespace CommonBE.Infrastructure.Search;

public class ResourceParameters
{
    /// <summary>
    /// 50, may be ovveriden for larger sets
    /// </summary>
    public virtual int MaxPageSize => 50;

    public bool? IsActive { get; set; }    
    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 1;

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
    public virtual string OrderBy { get; set; } = nameof(EntityBase.CreatedAt); //asc

    /// <summary>
    /// Defualt is CreatedAt desc
    /// </summary>
    //public virtual string OrderBy { get; set; } = $"{nameof(EntityBase.CreatedAt)} desc"; //desc 
}
