namespace NetCoreBE.Application.CrudExamples;

public interface ICrudExampleRepository : IRepository<CrudExample>
{
    Task<List<CrudExample>> Seed(int addCount, int? MaxInDb, string UserId = "Seed");
}
