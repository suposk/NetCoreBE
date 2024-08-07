namespace SharedContract.Dtos;

public class TicketHistoryDto : DtoBase
{
    public string? TicketId { get; set; }
    //public TicketDto? Ticket { get; set; } //circle reference exception
    public required string Operation { get; set; }
    public string? Details { get; set; }
}
