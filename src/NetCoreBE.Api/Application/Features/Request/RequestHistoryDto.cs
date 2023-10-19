namespace NetCoreBE.Api.Application.Features.Request;

public class RequestHistoryDto : DtoBase
{
    public string? RequestId { get; set; }
    public RequestDto? Request { get; set; }
    public required string? Operation { get; set; }
    public string? Details { get; set; }
}
