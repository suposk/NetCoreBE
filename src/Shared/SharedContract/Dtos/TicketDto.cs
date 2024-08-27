namespace SharedContract.Dtos;

public class TicketDto : DtoBase
{
    public required string TicketType { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public List<string> Notes { get; set; } = new();

    public List<TicketHistoryDto> TicketHistoryList { get; set; } = new();
}
