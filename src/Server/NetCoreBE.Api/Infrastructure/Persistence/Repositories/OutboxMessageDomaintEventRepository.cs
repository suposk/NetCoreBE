namespace NetCoreBE.Api.Infrastructure.Persistence.Repositories;

public class OutboxMessageDomaintEventRepository : Repository<OutboxMessageDomaintEvent>, IOutboxMessageDomaintEventRepository
{
    private readonly IRepository<OutboxMessageDomaintEvent> _repository;
    private readonly ApiDbContext? _context;

    public OutboxMessageDomaintEventRepository(IRepository<OutboxMessageDomaintEvent> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService)
        : base(repository.DatabaseContext, apiIdentity, dateTimeService)
    {
        _repository = repository;
        _context = repository.DatabaseContext as ApiDbContext;
    }

    public override Task<List<OutboxMessageDomaintEvent>> GetList()
    {
        //return _repository.GetListFilter(a => a.IsDeleted != true); //for soft delete
        return _repository.GetList();
    }

    public Task<bool> Exist(string entityId, string type) => _context.OutboxMessageDomaintEvents.AsNoTracking()
                                    .AnyAsync(a => a.EntityId == entityId && a.Type == type && a.IsProcessed == null);
     
    public Task<List<OutboxMessageDomaintEvent>> GetUnProcessedList(string entityId) => _context.OutboxMessageDomaintEvents.AsNoTracking()
                                                                .Where(a => a.EntityId == entityId && a.IsProcessed != true).ToListAsync();
    public Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50)
    {
        // not using AsNoTracking() because will update them
        var collection = _context.OutboxMessageDomaintEvents
            .Where(a => (a.IsProcessed == null && a.NextRetryUtc == null) || (a.IsProcessed != true && a.NextRetryUtc <= DateTimeService.UtcNow))
            .OrderBy(a => a.OccuredUtc)
            .Take(countToProcess);
        return collection.ToListAsync();
    }
}
