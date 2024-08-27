namespace NetCoreBE.Domain.Entities;

public class TicketHistory : EntityBase
{
    private TicketHistory() { }

    //public TicketHistory(string ticketId, string operation, string createdBy, string? details, DateTime? createdAt)
    //{
    //    if (string.IsNullOrEmpty(ticketId))        
    //        throw new ArgumentException($"'{nameof(ticketId)}' cannot be null or empty.", nameof(ticketId));
    //    if (string.IsNullOrEmpty(operation))        
    //        throw new ArgumentException($"'{nameof(operation)}' cannot be null or empty.", nameof(operation));

    //    Id ??= StringHelper.GetStringGuidExt();

    //    TicketId = ticketId;        
    //    Operation = operation;
    //    Details = details;

    //    CreatedBy = createdBy;
    //    CreatedAt = createdAt;        
    //}

    public static TicketHistory Create(string ticketId, string operation, string createdBy, string? details, DateTime? createdAt)
    {
        //if (string.IsNullOrEmpty(ticketId))
        //    throw new ArgumentException($"'{nameof(ticketId)}' cannot be null or empty.", nameof(ticketId));
        //if (string.IsNullOrEmpty(operation))
        //    throw new ArgumentException($"'{nameof(operation)}' cannot be null or empty.", nameof(operation));

        TicketHistory ticketHistory = new();
        ticketHistory.Id ??= StringHelper.GetStringGuidExt();

        ticketHistory.TicketId = ticketId;
        ticketHistory.Operation = operation;
        ticketHistory.Details = details;

        ticketHistory.CreatedBy = createdBy;
        ticketHistory.CreatedAt = createdAt;
        return ticketHistory;
    }

    [MaxLength(36)]
    public string? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    
    [MaxLength(50)]
    public string? Operation { get; set; }

    [MaxLength(500)]
    public string? Details { get; set; }
}
