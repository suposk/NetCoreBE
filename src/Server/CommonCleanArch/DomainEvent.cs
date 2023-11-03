// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CommonCleanArch;

public abstract class DomainEvent : INotification
{
    protected DomainEvent()
    {
        DateOccurred = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Set true, to resubmit DomainEvent after DomainEvent was saved in outbox
    /// </summary>
    public bool IsProcessing { get; set; }

    /// <summary>
    /// Id of OutboxMessage
    /// </summary>
    public string? Id { get; set; } 
    public DateTimeOffset DateOccurred { get; protected set; }

    /// <summary>
    /// Set to IsProcessing=true, when DomainEvent is being processed
    /// </summary>
    /// <param name="id"></param>
    public void SetToProcess(string id)
    {
        IsProcessing = true;
        Id = id;
    }
}
