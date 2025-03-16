// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommonCleanArch.Domain;

namespace NetCoreBE.Domain.Events;

/// <summary>
///  Event for statistics example
/// </summary>
/// <param name="item"></param>
public class GetSingleTicketEventStat(TicketStat item) : DomainEvent, IMemoryEvent
{
    public TicketStat Item { get; } = item;
}

public record TicketStat(string? Id, string? UsedIdOrIp);
