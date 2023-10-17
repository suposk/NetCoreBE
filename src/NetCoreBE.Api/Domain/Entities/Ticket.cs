namespace NetCoreBE.Api.Domain.Entities;

//public class Ticket : EntitySoftDeleteBase
public class Ticket : EntityBase
{
    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? RequestedFor { get; set; }

    public bool IsOnBehalf { get; set; }
}
