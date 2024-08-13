namespace SharedKernel.Base;

public interface IDtoBase
{
    //Guid Id { get; set; }
    string? Id { get; set; }
    DateTime? CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    string? Email { get; set; }    
    bool? IsDeleted { get; set; }
    DateTime? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
    //byte[]? RowVersion { get; set; }
    int RowVersion { get; set; }
}
