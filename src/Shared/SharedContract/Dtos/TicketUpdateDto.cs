namespace SharedContract.Dtos;

public class TicketUpdateDto : DtoUpdateBase
{    
    public string? Note { get; set; }
    public string? Status { get; set; }
}
