namespace CommonCleanArch.Base;

public interface IEntityEmailBase
{
    [MaxLength(200)]
    string? Email { get; set; }
}

public abstract class EntityEmailBase : EntityBase, IEntityEmailBase
{
    [MaxLength(200)]
    public string? Email { get; set; }
}
