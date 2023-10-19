namespace NetCoreBE.Api.Application.Features.Request;

public class RequestDto : DtoBase
{
    public required string? RequestType { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public ICollection<RequestHistoryDto> RequestHistoryList { get; set; } = new List<RequestHistoryDto>();
}
