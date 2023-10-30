namespace NetCoreBE.Api.Domain.Entities;

public class RequestHistory : EntityBase
{
    [MaxLength(36)]
    public string? RequestId { get; set; }
    public Request? Request { get; set; }
    
    [MaxLength(50)]
    public required string? Operation { get; set; }

    [MaxLength(500)]
    public string? Details { get; set; }
}
