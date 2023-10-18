using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Infrastructure.Persistence
{
    public interface ITicketRepository : IRepository<Entities.Ticket>
    {
        Task<List<Entities.Ticket>> Seed(int count, int? max, string UserId = "Seed");
    }

    public class TicketRepository : Repository<Entities.Ticket>, ITicketRepository
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
            //return base.GetList();
            //var exist = await _repository.GetFilter(a => a.VersionFull == version);
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            //return _repository.GetListFilter(a => a.IsDeleted != true);
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
                {
                    ticket.Id = Guid.Empty.ToString();
                    ticket.Id = ticket.Id.Remove(0, i.ToString().Length);
                    ticket.Id = ticket.Id.Insert(0, i.ToString());
                }
                list.Add(ticket);
            }
            _repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
