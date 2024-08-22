using CommonCleanArch.Application.Services;

namespace NetCoreBE.Infrastructure.Persistence.Repositories;

public class OutboxDomaintEventRepository : Repository<OutboxDomaintEvent>, IOutboxDomaintEventRepository
{
    private readonly IRepository<OutboxDomaintEvent> _repository;
    private readonly ApiDbContext? _context;

    public OutboxDomaintEventRepository(IRepository<OutboxDomaintEvent> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService)
        : base(repository.DatabaseContext, apiIdentity, dateTimeService)
    {
        _repository = repository;
        _context = repository.DatabaseContext as ApiDbContext;
    }

    public override Task<List<OutboxDomaintEvent>> GetList()
    {
        //return _repository.GetListFilter(a => a.IsDeleted != true); //for soft delete
        return _repository.GetList();
    }

    public Task<bool> Exist(string entityId, string type) => _context.OutboxDomaintEvents.AsNoTracking()
                                    .AnyAsync(a => a.EntityId == entityId && a.Type == type && a.IsProcessed == null);    
    public Task<List<OutboxDomaintEvent>> GetUnProcessedList(string entityId) => _context.OutboxDomaintEvents.AsNoTracking()
                                                                .Where(a => a.EntityId == entityId && a.IsProcessed != true).ToListAsync();
    public Task<List<OutboxDomaintEvent>> GetListToProcess(int countToProcess = 50)
    {
        // not using AsNoTracking() because will update them
        var collection = _context.OutboxDomaintEvents
            .Where(a => (a.IsProcessed == null && a.NextRetryUtc == null) || (a.IsProcessed != true && a.NextRetryUtc <= DateTimeService.UtcNow))
            .OrderBy(a => a.OccuredUtc)
            .Take(countToProcess);
        return collection.ToListAsync();
    }

    public Task<List<OutboxDomaintEvent>> GetAllByEntityId(string entityId) => _context.OutboxDomaintEvents.AsNoTracking()
                                        .Where(a => a.EntityId == entityId).ToListAsync();

    public Task<List<OutboxDomaintEvent>> GetAllByEntityIdType(string entityId, string type) => _context.OutboxDomaintEvents.AsNoTracking()
                                        .Where(a => a.EntityId == entityId && a.Type == type).ToListAsync();

    public async Task<List<OutboxDomaintEvent>> RemoveAllEntityId(string entityId) 
    {
        var toDel = await _context.OutboxDomaintEvents.Where(a => a.EntityId == entityId).ToListAsync();   
        _context.OutboxDomaintEvents.RemoveRange(toDel);
        await _context.SaveChangesAsync();
        return toDel;
    }

    public Task<bool> SaveStatus(OutboxDomaintEvent domaintEvent, bool deleteProcessed = false)
    {
        if (deleteProcessed && domaintEvent.IsProcessed == true)
            Remove(domaintEvent);
        else
            Update(domaintEvent);
        return SaveChangesAsync();
    }

}
