namespace NetCoreBE.Domain.UnitTests.Tickets;

internal static class TicketData
{
    public const string TicketId = "Ticket-1";
    public const string AddTicketId = "Ticket-01";

    public static readonly TicketDto Add = new()
    {
        Id = AddTicketId,
        TicketType = "New Laptop",
        Note = "add test",
        CreatedBy = "Test",
    };

    public static readonly TicketUpdateDto Update = new()
    {
        Id = TicketId,        
        Note = "Update 1",
        //RowVersion = 739,
        RowVersion = 741,
    };
}
