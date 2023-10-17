using CommonBE.Infrastructure.Persistence;
using NetCoreBE.Api.Infrastructure.Persistence;

namespace CSRO.Server.Services
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task Seed(int count = 10, string UserId = "Seed");
    }

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
            //return base.GetList();
            //var exist = await _repository.GetFilter(a => a.VersionFull == version);
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            //return _repository.GetListFilter(a => a.IsDeleted != true);
            return _repository.GetList();
        }

        public async Task Seed(int count = 10, string UserId = "Seed")
        {
            var countExisintg = await _repository.CountAsync();

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
                    ticket.Id = ticket.Id.Remove(0, 1);
                    ticket.Id = ticket.Id.Insert(0, i.ToString());
                }
                list.Add(ticket);
            }
            _repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return;
        }   
    }
}
