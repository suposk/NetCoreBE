namespace CommonBE.Base;

public interface IEntityEmailBase
{
    [MaxLength(200)]
    string? Email { get; set; }
}

public abstract class EntityEmailBase : EntityBase, IEntityEmailBase
{
    public string? Email { get; set; }
}
