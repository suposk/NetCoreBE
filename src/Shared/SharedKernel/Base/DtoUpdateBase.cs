namespace SharedKernel.Base;

/// <summary>
/// Id and RowVersion for update
/// </summary>
public class DtoUpdateBase
{
    public string? Id { get; set; }
    public uint RowVersion { get; set; }
}
