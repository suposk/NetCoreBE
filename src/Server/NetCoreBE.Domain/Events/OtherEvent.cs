// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Domain.Events;

public class OtherEvent<T> : DomainEvent where T : IEntity
{
    public OtherEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
