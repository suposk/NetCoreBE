// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Application.Interfaces;

public interface IApiDbContext
{
    DbSet<TicketHistory> TicketHistorys { get; set; }
    DbSet<Ticket> Tickets { get; set; }
    DbSet<OutboxDomaintEvent> OutboxDomaintEvents { get; set; }
    DbSet<CrudExample> CrudExamples { get; set; }

    ///// <summary>
    ///// Not sure if needed
    ///// </summary>
    ///// <param name="cancellationToken"></param>
    ///// <returns></returns>
    //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
