namespace NetCoreBE.Infrastructure.Persistence.Repositories;

public class CrudExampleRepository : Repository<CrudExample>, ICrudExampleRepository
{
    private readonly IRepository<CrudExample> _repository;
    private readonly IPropertyMappingService _propertyMappingService;
    private IApiDbContext? _context;

    public CrudExampleRepository(IRepository<CrudExample> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IPropertyMappingService propertyMappingService)
        : base(repository.DatabaseContext, apiIdentity, dateTimeService)
    {
        _repository = repository;
        _propertyMappingService = propertyMappingService;
        _context = repository.DatabaseContext as ApiDbContext;        
    }

    public async Task<List<CrudExample>> Seed(int addCount, int? MaxInDb, string UserId = "Seed")
    {
        if (addCount <= 0)
            return default;

        var countExisintg = await _repository.CountAsync();
        if (MaxInDb.HasValue && countExisintg >= MaxInDb)
            return default;

        var list = new List<CrudExample>();
        var cycle = 0;
        var newTotal = addCount + countExisintg;
        for (int i = countExisintg + 1; i <= newTotal; i++)
        {
            cycle++;
            string? id = null;
            if (countExisintg == 0)
                id = $"{nameof(CrudExample)}-{i}";

            CrudExample crudExample = new() { Id = id, Name = $"{UserId} Name {i}" };
            crudExample.IsActive = (i % 2 == 0);
            crudExample.Description = (i % 3 == 0) ? $"{UserId} Description {cycle} / {addCount} / {newTotal}" : null;            
            list.Add(crudExample);
        }
        _repository.AddRange(list, UserId);
        await _repository.SaveChangesAsync();
        return list;
    }
}
