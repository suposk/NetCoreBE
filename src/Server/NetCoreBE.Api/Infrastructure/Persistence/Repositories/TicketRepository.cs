namespace NetCoreBE.Api.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly IRepository<Ticket> _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ApiDbContext? _context;

        public TicketRepository(IRepository<Ticket> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IPropertyMappingService propertyMappingService) 
            : base(repository.DatabaseContext, apiIdentity, dateTimeService)
        {
            _repository = repository;
            _propertyMappingService = propertyMappingService;
            _context = repository.DatabaseContext as ApiDbContext;
        }

        public override Task<List<Ticket>> GetList()
        {
            //return _repository.GetListFilter(a => a.IsDeleted != true); //for soft delete
            return _repository.GetList();
        }

        public override void Add(Ticket entity, string UserId = null)
        {
            entity.SetNewTicket();            
            base.Add(entity, UserId);
            //entity.AddDomainEvent(new TicketCreatedEvent(entity)); //test only
            entity.AddDomainEvent(new CreatedEvent<Ticket>(entity)); //if called from repo, this would not get fired
        }

        public async Task<PagedList<Ticket>> Search(TicketSearchParameters searchParameters)
        {
            if (_context == null)            
                throw new ArgumentNullException(nameof(_context));            
            if (searchParameters == null)            
                throw new ArgumentNullException(nameof(searchParameters));            

            var collection = _context.Tickets.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchParameters.Description))
            {
                var description = searchParameters.Description.Trim();
                //collection = collection.Where(a => a.Description == description);
                collection = collection.Where(a => a.Description.Contains(description));
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.SearchTerm))
            {
                var searchQuery = searchParameters.SearchTerm.Trim();
                collection = collection.Where(a => a.Description.Contains(searchQuery)
                    || a.RequestedFor.Contains(searchQuery)
                    //|| a.IsSavedInDb.Contains(searchQuery)
                    );
            }

            //if (searchParameters.IsActive.HasValue) //if Entity has IsDeleted prop             
            //    collection = collection.Where(a => searchParameters.IsActive.Value ? a.IsDeleted != true : a.IsDeleted == true);            

            if (!string.IsNullOrWhiteSpace(searchParameters.OrderBy))
            {
                // get property mapping dictionary
                var propertyMappingDictionary = _propertyMappingService.GetPropertyMapping<TicketDto, Ticket>();
                collection = collection.ApplySort(searchParameters.OrderBy, propertyMappingDictionary);
            }
            var res = await PagedList<Ticket>.CreateAsync(collection,
                searchParameters.CurrentPage,
                searchParameters.PageSize);
            return res;
        }

        public async Task<List<Ticket>> Seed(int count, int? max, string UserId = "Seed")
        {
            if (count <= 0)
                return default;

            var countExisintg = await _repository.CountAsync();
            if (max.HasValue && countExisintg >= max)
                return default;

            var list = new List<Ticket>();
            for (int i = 1; i <= count; i++)
            {
                var ticket = new Ticket
                {
                    Description = $"Description {i}{(count >= 50? $" seed {count}": null)}",
                    RequestedFor = $"RequestedFor {i}",
                    IsOnBehalf = i % 2 == 0,
                    CreatedAt = DateTime.UtcNow, //test only
                };
                if (countExisintg == 0)
                    ticket.Id = i.GetSimpleGuidString();
                list.Add(ticket);
                Add(ticket, UserId);
                //await Task.Delay(10);
            }
            //_repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
