namespace NetCoreBE.Domain.UnitTests.Tickets;

internal static class TicketData
{
    public static readonly Ticket Ticket = new Ticket
    {
        Id = "Ticket-1-test",
        TicketType = "New Laptop",
        Note = "Description Post",
        Status = "Status",
        CreatedBy = "test",
    };
}
