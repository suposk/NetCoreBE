namespace NetCoreBE.Domain.Entities;

/// <summary>
/// OutboxMessage pattern for some DomaintEvent
/// </summary>
[Index(nameof(Type), nameof(IsProcessed), nameof(NextRetryUtc), nameof(EntityId))]
public sealed class OutboxDomaintEvent : EntityBase
{
    //OutboxMessageDomaintEvent -> OutboxDomaintEvent

    private OutboxDomaintEvent()
    {
        //EF
    }

    [NotMapped]
#if DEBUG
    public const int MaxRetryCount = 2;
#else
    public const int MaxRetryCount = 5;
#endif

    /// <summary>
    /// EntityId related to this domain event
    /// Will store GUID as string
    /// </summary>
    [MaxLength(36)]
    public string? EntityId { get; set; }

    public bool? IsProcessed { get; private set; }

    /// <summary>
    /// Event is processed successfully
    /// </summary>
    public bool? IsSuccess { get; private set; }

    [MaxLength(500)]
    public string? Error { get; set; }

    [MaxLength(200)]
    public string? Type { get; set; }

    //[MaxLength(200)]
    //public string? TypeDetail { get; set; }//not needed most likely

    public string? Content { get; set; }
    public DateTime OccuredUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public int RetryCount { get; private set; }
    public DateTime? NextRetryUtc { get; private set; }

    public static OutboxDomaintEvent Create(string? entityId, DateTime utcNow, string type, string content)
    {
        return new OutboxDomaintEvent
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
        //domain event is completed
        IsSuccess = true;
        IsProcessed = true;
        ProcessedUtc = utcNow;        
        NextRetryUtc = null;        
    }

    /// <summary>
    /// Failed to process domain event after some retry
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="nextRetryUtc"></param>
    public void SetFailed(DateTime utcNow, string? error, DateTime? nextRetryUtc = null)
    {
        IsSuccess = false;

        //possible some domain event failed
        if (error.HasValueExt() && !string.Equals(error, Error))
            Error += error; //dont want to ovveride error message        
                
        if (RetryCount > 0 && (RetryCount % MaxRetryCount == 0)) //allow 
        {
            SetToIgnored(utcNow); //final failed after some retry
        }
        else
        {            
            ProcessedUtc = utcNow;
            RetryCount++;
            if (nextRetryUtc.HasValue && nextRetryUtc > utcNow)
                NextRetryUtc = nextRetryUtc.Value;
        }
    }

    /// <summary>
    /// If need to process again/replay domain event
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="nextRetryUtc"></param>
    public void ReplayEvent(DateTime utcNow)
    {
        IsSuccess = null;
        IsProcessed = false;
        ProcessedUtc = utcNow;
        NextRetryUtc = null;
        RetryCount = 0;
    }

    /// <summary>
    /// If need to ignore domain event, after some retry
    /// </summary>
    /// <param name="utcNow"></param>
    private void SetToIgnored(DateTime utcNow)
    {               
        IsProcessed = true;
        ProcessedUtc = utcNow;
        NextRetryUtc = null;
    }
}