﻿namespace NetCoreBE.Domain.UnitTests.Tickets;

/// <summary>
/// Integration test data for Ticket
/// </summary>
internal static class TicketData
{
    public const string TicketId = "Ticket-1";
    public const string AddTicketId = "Ticket-01";

    public static readonly TicketDto Add = new()
    {
        Id = AddTicketId,
        TicketType = TicketTypeEnum.Access.ToString(),
        Note = "add test",
        CreatedBy = "Test",
    };

    //public static readonly TicketUpdateDto Update = new()
    //{
    //    Id = TicketId,        
    //    Note = "Update 1",
    //    //RowVersion = 739,
    //    //RowVersion = 741,
    //    RowVersion = 750, //unreliable
    //};
}
