namespace NetCoreBE.Domain.UnitTests.Tickets;

/// <summary>
/// Domian data for Ticket
/// </summary>
internal static class TicketData
{
    public static readonly string TicketId = "Ticket-1";
    public static readonly string AddTicketId = "Ticket-01";

    public static readonly Ticket Ticket = Ticket.Create(AddTicketId, TicketTypeEnum.Access.ToString(), "Description Post", "test");
}
