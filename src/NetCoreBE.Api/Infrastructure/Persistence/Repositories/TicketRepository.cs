using NetCoreBE.Api.Application.Features.Tickets;

namespace NetCoreBE.Api.Infrastructure.Persistence.Repositories
{

    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly IRepository<Ticket> _repository;
        private ApiDbContext _context;

        public TicketRepository(IRepository<Ticket> repository, ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
        {
            _repository = repository;
            _context = context;
        }

        public override Task<List<Ticket>> GetList()
        {
            //return _repository.GetListFilter(a => a.IsDeleted != true); //for soft delete
            return _repository.GetList();
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
                    Description = $"Description {i}",
                    RequestedFor = $"RequestedFor {i}",
                    IsOnBehalf = i % 2 == 0,
                };
                if (countExisintg == 0)
                    ticket.Id = i.GetSimpleGuidString();
                list.Add(ticket);
            }
            _repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
