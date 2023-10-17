using NetCoreBE.Api.Infrastructure.Persistence;

namespace CSRO.Server.Services
{
    public interface ITicketRepository : IRepository<Ticket>
    {

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
    }
}
