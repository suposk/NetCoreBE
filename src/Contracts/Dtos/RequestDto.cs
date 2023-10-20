namespace Contracts.Dtos;

public class RequestDto : DtoBase
{
    public required string? RequestType { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public List<RequestHistoryDto> RequestHistoryList { get; set; } = new();
}
