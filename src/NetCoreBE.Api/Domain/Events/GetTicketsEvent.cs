// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Domain.Events;

public class GetTicketsEvent : DomainEvent
{
    public GetTicketsEvent(GetTickets item)
    {
        Item = item;
    }

    public GetTickets Item { get; }
}

public record GetTickets(string UsedIdOrIp, DateTime DateTime);
