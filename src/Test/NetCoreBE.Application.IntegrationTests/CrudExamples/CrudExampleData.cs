namespace NetCoreBE.Domain.UnitTests.CrudExamples;

internal static class CrudExampleData
{
    public static readonly string CrudExampleId = "CrudExample-1";
    public static readonly string AddCrudExample = "CrudExample-01";

    public static readonly CrudExampleDto Add = new()
    {
        Id = AddCrudExample,
        IsActive = true,
        Name = "Name 1",
        Description = "Description test 1",
        CreatedBy = "Test",
    };

    public static readonly CrudExampleDto Update = new()
    {
        Id = CrudExampleId,
        IsActive = false,
        Name = "Name put 1",
        Description = "Description put 1",
        RowVersion = 758,
    };
}
