namespace NetCoreBE.Domain.UnitTests.Tickets;

internal static class TicketData
{
    public static readonly string TicketId = "Ticket-1";
    public static readonly string AddTicketId = "Ticket-01";

    public static readonly Ticket Ticket = Ticket.Create(TicketId, "New Laptop", "Description Post", "test");
}
