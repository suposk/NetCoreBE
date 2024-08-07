// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Domain.Events;

public class OldTicketCreatedExampleEvent : DomainEvent
{
    public OldTicketCreatedExampleEvent(OldTicket item)
    {
        Item = item;
    }
    public OldTicket Item { get; }
}
