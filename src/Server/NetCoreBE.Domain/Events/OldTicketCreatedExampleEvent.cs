// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommonCleanArch.Domain;

namespace NetCoreBE.Domain.Events;

public class OldTicketCreatedExampleEvent : DomainEvent
{
    public OldTicketCreatedExampleEvent(OldTicket item)
    {
        Item = item;
    }
    public OldTicket Item { get; }
}
