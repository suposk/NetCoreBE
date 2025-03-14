// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CommonCleanArch.Domain;

/// <summary>
/// Marker interface for in-memory events. Don't store in Outbox
/// </summary>
public interface IMemoryEvent
{

}
