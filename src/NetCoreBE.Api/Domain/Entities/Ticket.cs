namespace NetCoreBE.Api.Domain.Entities;

//public class Ticket : EntitySoftDeleteBase
public class Ticket : EntityBase
{
    public string? Description { get; set; }

    public string? RequestedFor { get; set; }

    public bool IsOnBehalf { get; set; }
}
