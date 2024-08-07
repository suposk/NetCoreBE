public class Ticket : EntityBase
{
    [MaxLength(50)]
    public required string TicketType { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public List<TicketHistory> TicketHistoryList { get; set; } = new();

    private void AddInitialHistory(DateTime utc)
    {        
        if (TicketHistoryList?.Count == 0)        
            TicketHistoryList.Add(new TicketHistory(ticketId: Id, operation: "Submited", null, details: "User Submited Ticket", createdAt: utc));
                
    }

    public void Create(DateTime utc)
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
