using CommonCleanArch.Application.Search;
using CommonCleanArch.Application.Services;
using CommonCleanArch.Domain;

namespace NetCoreBE.Infrastructure.Persistence.Repositories
{
    public class OldTicketRepository : Repository<OldTicket>, IOldTicketRepository
    {
        private readonly IRepository<OldTicket> _repository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ApiDbContext? _context;

        public OldTicketRepository(IRepository<OldTicket> repository, IApiIdentity apiIdentity, IDateTimeService dateTimeService, IPropertyMappingService propertyMappingService) 
            : base(repository.DatabaseContext, apiIdentity, dateTimeService)
        {
            _repository = repository;
            _propertyMappingService = propertyMappingService;
            _context = repository.DatabaseContext as ApiDbContext;
        }

        public override Task<List<OldTicket>> GetList()
        {
            //return _repository.GetListFilter(a => a.IsDeleted != true); //for soft delete
            return _repository.GetList();
        }

        public override void Add(OldTicket entity, string UserId = null)
        {
            entity.SetNewTicket();            
            base.Add(entity, UserId);
            //entity.AddDomainEvent(new TicketCreatedExampleEvent(entity)); //example, test only
            entity.AddDomainEvent(new CreatedEvent<OldTicket>(entity)); //if called from repo, this would not get fired
        }

        public async Task<PagedList<OldTicket>> Search(TicketSearchParameters searchParameters)
        {
            if (_context == null)            
                throw new ArgumentNullException(nameof(_context));            
            if (searchParameters == null)            
                throw new ArgumentNullException(nameof(searchParameters));            

            var collection = _context.OldTickets.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchParameters.Description))
            {
                var description = searchParameters.Description.Trim();                
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
                var propertyMappingDictionary = _propertyMappingService.GetPropertyMapping<OldTicketDto, OldTicket>();
                collection = collection.ApplySort(searchParameters.OrderBy, propertyMappingDictionary);
            }
            var res = await PagedList<OldTicket>.CreateAsync(collection,
                searchParameters.CurrentPage,
                searchParameters.PageSize);
            return res;
        }
                
        public async Task<List<OldTicket>> Seed(int addCount, int? MaxInDb, string UserId = "Seed")
        {
            if (addCount <= 0)
                return default;

            var countExisintg = await _repository.CountAsync();
            if (MaxInDb.HasValue && countExisintg >= MaxInDb)
                return default;

            var list = new List<OldTicket>();
            for (int i = countExisintg + 1; i <= addCount; i++)
            {
                var OldTicket = new OldTicket
                {
                    Description = $"Description {i}{(addCount >= 50? $" seed {addCount}": null)}",
                    RequestedFor = $"RequestedFor {i}",
                    IsOnBehalf = i % 2 == 0,
                    CreatedAt = DateTime.UtcNow, //test only
                };
                if (countExisintg == 0)
                    OldTicket.Id = $"{nameof(OldTicket)}-{i}";
                list.Add(OldTicket);
                Add(OldTicket, UserId);
                //await Task.Delay(10);
            }
            //_repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
