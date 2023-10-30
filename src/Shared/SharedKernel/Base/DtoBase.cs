namespace SharedKernel.Base;

public class DtoBase : IDtoBase
{
    public Guid Id { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public byte[]? RowVersion { get; set; }
    public string? Email { get; set; }
}
