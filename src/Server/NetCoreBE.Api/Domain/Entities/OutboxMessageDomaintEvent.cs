﻿namespace NetCoreBE.Api.Domain.Entities;

/// <summary>
/// OutboxMessage pattern for some DomaintEvent
/// </summary>
[Index(nameof(Type), nameof(IsSuccess), nameof(NextRetryUtc))]
public sealed class OutboxMessageDomaintEvent : EntityBase
{
    [NotMapped]
    public const int MaxRetryCount = 5;

    public bool? IsSuccess { get; set; }
    
    [MaxLength(500)]
    public string? Error { get; set; }

    [MaxLength(200)]
    public string Type { get; set; }

    [MaxLength(200)]
    public string? TypeDetail { get; set; }

    public string? Content { get; set; }
    public DateTime OccuredUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryUtc { get; set; }

    public static OutboxMessageDomaintEvent Create(string? id, DateTime utcNow, string type, string? typeDetail, string content)
    {
        return new OutboxMessageDomaintEvent
        {            
            Id = id,
            Type = type ?? throw new ArgumentNullException(nameof(type)),
            TypeDetail = typeDetail,
            Content = content ?? throw new ArgumentNullException(nameof(type)),
            OccuredUtc = utcNow,
            RetryCount = 0
        };
    }

    public void Completed(DateTime utcNow)
    {
        //possible some domain event is completed
        IsSuccess = true;
        ProcessedUtc = utcNow;        
        NextRetryUtc = null;        
    }

    /// <summary>
    /// Failed to process domain event after some retry
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="nextRetryUtc"></param>
    public void Failed(DateTime utcNow, string? error, DateTime? nextRetryUtc)
    {
        //possible some domain event failed
        if (error.HasValueExt()) Error = error; //dont want to ovveride error message        
        if (RetryCount >= MaxRetryCount)
        {
            IsSuccess = false; //final failed
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