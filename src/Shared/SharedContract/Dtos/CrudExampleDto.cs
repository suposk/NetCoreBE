namespace SharedContract.Dtos;

public class CrudExampleDto : DtoBase
{
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
