// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CommonCleanArch.Domain;
using CommonCleanArch.Domain.Base;

namespace NetCoreBE.Domain.Events;

public class OtherEvent<T> : DomainEvent where T : IEntity
{
    public OtherEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
