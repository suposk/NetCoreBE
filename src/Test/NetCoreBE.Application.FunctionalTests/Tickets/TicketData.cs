namespace NetCoreBE.Application.FunctionalTests.Tickets;

internal static class TicketData
{
    public static readonly string TicketId = "Ticket-1";
    public static readonly string AddTicketId = "Ticket-01";

    public static readonly TicketDto Add = new()
    {
        Id = AddTicketId,
        TicketType = "New Laptop",
        Note = "add test",
        CreatedBy = "Test",
    };
}
