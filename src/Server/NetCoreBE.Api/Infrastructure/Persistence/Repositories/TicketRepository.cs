using NetCoreBE.Api.Application.Features.OutboxMessageDomaintEvents;

namespace NetCoreBE.Api.Infrastructure.Persistence.Repositories
{
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

        public Task<List<OutboxMessageDomaintEvent>> GetListToProcess(int countToProcess = 50)
        {
            // not using AsNoTracking() because will update them
            var collection = _context.OutboxMessageDomaintEvents
                .Where(a => (a.IsSuccess == null && a.NextRetryUtc == null) || (a.IsSuccess != true && a.NextRetryUtc <= DateTimeService.UtcNow))
                .OrderBy(a => a.OccuredUtc)
                .Take(countToProcess);
            return collection.ToListAsync();
        }
    }
}
