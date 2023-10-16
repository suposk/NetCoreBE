using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(Id))]
[Index(nameof(Id))]
[Index(nameof(CreatedAt))]
[Index(nameof(CreatedBy))]
public abstract class EntityBase : IEntity<string?>
{
    private readonly List<DomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(DomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    //public virtual Guid Id { get; set; }

    /// <summary>
    /// Will store GUID as string
    /// </summary>
    [MaxLength(36)]
    public virtual string? Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    [MaxLength(200)]
    public virtual string? CreatedBy { get; set; }

    public virtual DateTime? ModifiedAt { get; set; }

    [MaxLength(200)]
    public virtual string? ModifiedBy { get; set; }

    //[Timestamp]
    //public byte[]? RowVersion { get; set; }
    public virtual bool IsSavedInDb => CreatedAt.HasValue && CreatedAt > DateTime.MinValue;    

    //public virtual bool IsSavedInDb => Id != Guid.Empty;
}
