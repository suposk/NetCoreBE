public class Ticket : EntityBase
{
    public static Ticket EmptyTicket = Create("", "", "", "");

    private Ticket() { }

    private Ticket(string? id,
                   string? ticketType,
                   string? note,
                   string? status,
                   string? createdBy,
                   List<TicketHistory>? ticketHistoryList)
    {
        Id = id ?? StringHelper.GetStringGuidExt();
        TicketType = ticketType;
        if (string.IsNullOrWhiteSpace(ticketType))
            throw new ArgumentException("ticketType must be provided");
        Note = note;
        Status = status;
        CreatedBy = createdBy;
        TicketHistoryList = ticketHistoryList ?? new();
    }

    [MaxLength(50)]
    public string? TicketType { get; private set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MaxLength(50)]
    public string? Status { get; private set; }

    public List<TicketHistory> TicketHistoryList { get; set; } = new();

    private void AddInitialHistory(DateTime utc)
    {        
        if (TicketHistoryList?.Count == 0)        
            TicketHistoryList.Add(new TicketHistory(ticketId: Id, operation: "Submited", null, details: "User Submited Ticket", createdAt: utc));                
    }

    public static Ticket Create(string? id, string? ticketType, string? note, string? createdBy)
    {
        var ticket = new Ticket(id, ticketType, note, status: "Submited", createdBy, null);        
        ticket.AddInitialHistory(DateTime.UtcNow);
        return ticket;
    }

    public void Init(DateTime utc)
    {
        if (CanAddHistory() is false) //exidently can be called multiple times
            return;

        Id ??= StringHelper.GetStringGuidExt();
        AddInitialHistory(utc);
        if (Status is not "Submited")
            Status = "Submited";
        CreatedAt = utc;
    }

    public bool CanAddHistory() => Status is null || Status != "Closed";    
}
