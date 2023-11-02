namespace NetCoreBE.Api.Domain.Entities;

/// <summary>
/// OutboxMessage pattern for some DomaintEvent
/// </summary>
[Index(nameof(Type), nameof(IsProcessed), nameof(NextRetryUtc), nameof(EntityId))]
public sealed class OutboxMessageDomaintEvent : EntityBase
{
    [NotMapped]
    public const int MaxRetryCount = 5;

    /// <summary>
    /// EntityId related to this domain event
    /// Will store GUID as string
    /// </summary>
    [MaxLength(36)]
    public string? EntityId { get; set; }

    public bool? IsProcessed { get; set; }
    
    [MaxLength(500)]
    public string? Error { get; set; }

    [MaxLength(200)]
    public string Type { get; set; }

    //[MaxLength(200)]
    //public string? TypeDetail { get; set; }//not needed most likely

    public string? Content { get; set; }
    public DateTime OccuredUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryUtc { get; set; }

    public static OutboxMessageDomaintEvent Create(string? entityId, DateTime utcNow, string type, string content)
    {
        return new OutboxMessageDomaintEvent
        {            
            EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId)),
            Type = type ?? throw new ArgumentNullException(nameof(type)),            
            Content = content ?? throw new ArgumentNullException(nameof(type)),
            //TypeDetail = typeDetail,
            OccuredUtc = utcNow,
            RetryCount = 0
        };
    }

    public void SetProcessed(DateTime utcNow)
    {
        //possible some domain event is completed
        IsProcessed = true;
        ProcessedUtc = utcNow;        
        NextRetryUtc = null;        
    }

    /// <summary>
    /// Failed to process domain event after some retry
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="nextRetryUtc"></param>
    public void SetFailed(DateTime utcNow, string? error, DateTime? nextRetryUtc)
    {
        //possible some domain event failed
        if (error.HasValueExt()) Error = error; //dont want to ovveride error message        
        if (RetryCount >= MaxRetryCount)
        {
            IsProcessed = false; //final failed
            ProcessedUtc = utcNow;            
            NextRetryUtc = null;            
        }
        else
        {            
            ProcessedUtc = utcNow;
            RetryCount++;
            if (nextRetryUtc.HasValue && nextRetryUtc > utcNow)
                NextRetryUtc = nextRetryUtc.Value;
        }
    }
}