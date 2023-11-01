using Microsoft.VisualBasic;

﻿namespace NetCoreBE.Api.Domain.Entities;

/// <summary>
/// OutboxMessage pattern for some DomaintEvent
/// </summary>
public sealed class OutboxMessageDomaintEvent : EntityBase
{
    public const int MaxRetryCount = 5;

    public bool? IsSuccess { get; set; }
    public string? Error { get; set; }
    public string Type { get; set; }
    public string? TypeDetail { get; set; }
    public string? Content { get; set; }
    public DateTime OccuredUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryUtc { get; set; }
}