namespace CommonBE.Base;

public interface IDtoBase
{
    DateTime? CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    string? Email { get; set; }
    Guid Id { get; set; }
    bool? IsDeleted { get; set; }
    DateTime? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
    byte[]? RowVersion { get; set; }
}
