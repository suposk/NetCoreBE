namespace SharedContract.Dtos;

public class OldTicketDto : DtoBase
{
    public required string? Description { get; set; }
    public required string? RequestedFor { get; set; }
    public bool IsOnBehalf { get; set; }
}
