namespace NetCoreBE.Infrastructure.Persistence.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    private readonly IRepository<Ticket> _repository;
    private readonly IPropertyMappingService _propertyMappingService;
    private IApiDbContext? _context;

    public TicketRepository(IRepository<Ticket> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IPropertyMappingService propertyMappingService)
        : base(repository.DatabaseContext, apiIdentity, dateTimeService)
    {
        _repository = repository;
        _propertyMappingService = propertyMappingService;
        _context = repository.DatabaseContext as ApiDbContext;        
    }

    public override Task<Ticket> GetId(string id)
    {
        //return base.GetId(id);
        return _repository.GetFilter(a => a.Id == id, a => a.TicketHistoryList);
    }

    public async Task<List<Ticket>> Seed(int addCount, int? MaxInDb, string UserId = "Seed")
    {
        if (addCount <= 0)
            return default;

        var countExisintg = await _repository.CountAsync();
        if (MaxInDb.HasValue && countExisintg >= MaxInDb)
            return default;

        var list = new List<Ticket>();
        var cycle = 0;
        var newTotal = addCount + countExisintg;
        for (int i = countExisintg + 1; i <= newTotal; i++)
        {
            cycle++;
            var Ticket = new Ticket
            {
                Note = $"note {cycle} / {addCount} / {newTotal}",                
                TicketType = i % 2 == 0 ? "Access" : "New Laptop",
            };
            if (countExisintg == 0)
                Ticket.Id = $"{nameof(Ticket)}-{i}";
            Ticket.Create(DateTime.UtcNow);
            list.Add(Ticket);
        }
        _repository.AddRange(list, UserId);
        await _repository.SaveChangesAsync();
        return list;
    }
}
