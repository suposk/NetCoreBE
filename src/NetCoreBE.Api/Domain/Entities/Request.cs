namespace NetCoreBE.Api.Domain.Entities;

public class Request : EntityBase
{
    [MaxLength(50)]
    public required string? RequestType { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public ICollection<RequestHistory> RequestHistoryList { get; set; } = new List<RequestHistory>();
}
