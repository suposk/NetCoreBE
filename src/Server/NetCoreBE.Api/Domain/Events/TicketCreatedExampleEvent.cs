// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Domain.Events;

public class TicketCreatedExampleEvent : DomainEvent
{
    public TicketCreatedExampleEvent(Ticket item)
    {
        Item = item;
    }
    public Ticket Item { get; }
}
