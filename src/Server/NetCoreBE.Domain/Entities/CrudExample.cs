namespace NetCoreBE.Domain.Entities;

public class CrudExample : EntityBase
{
    public bool IsActive { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }
}
