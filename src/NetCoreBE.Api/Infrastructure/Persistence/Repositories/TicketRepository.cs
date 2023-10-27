using CommonBE.Infrastructure.Search;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedList<Ticket>> Search(TicketSearchParameters ticketSearchParameters)
        {
            if (_context == null)            
                throw new ArgumentNullException(nameof(_context));            
            if (ticketSearchParameters == null)            
                throw new ArgumentNullException(nameof(ticketSearchParameters));            

            var collection = _context.Tickets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(ticketSearchParameters.Description))
            {
                var description = ticketSearchParameters.Description.Trim();
                //collection = collection.Where(a => a.Description == description);
                collection = collection.Where(a => a.Description.Contains(description));
            }

            if (!string.IsNullOrWhiteSpace(ticketSearchParameters.SearchQuery))
            {

                var searchQuery = ticketSearchParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Description.Contains(searchQuery)
                    || a.RequestedFor.Contains(searchQuery)
                    //|| a.IsSavedInDb.Contains(searchQuery)
                    );
            }

            if (!string.IsNullOrWhiteSpace(ticketSearchParameters.OrderBy))
            {
                // get property mapping dictionary
                var propertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<TicketDto, Ticket>();
                collection = collection.ApplySort(ticketSearchParameters.OrderBy,
                    propertyMappingDictionary);
            }
            var res = await PagedList<Ticket>.CreateAsync(collection,
                ticketSearchParameters.PageNumber,
                ticketSearchParameters.PageSize);
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
                await Task.Delay(10);
            }
            _repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
