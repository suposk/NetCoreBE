// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Application.Interfaces;

public interface IApiDbContext
{
    DbSet<RequestHistory> RequestHistorys { get; set; }
    DbSet<Request> Requests { get; set; }
    DbSet<Ticket> Tickets { get; set; }
    DbSet<OutboxMessageDomaintEvent> OutboxMessageDomaintEvents { get; set; }
}
