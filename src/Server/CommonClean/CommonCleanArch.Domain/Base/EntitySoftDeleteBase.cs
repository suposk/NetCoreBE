namespace CommonCleanArch.Domain.Base;

/// <summary>
/// has Extra 2 properties IsDeleted and Email
/// </summary>
public abstract class EntitySoftDeleteBase : EntityBase, IEntityEmailBase
{
    public bool? IsDeleted { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }
}
