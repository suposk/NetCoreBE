using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Infrastructure.Persistence
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<List<Request>> Seed(int count, int? max, string UserId = "Seed");
    }

    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly IRepository<Request> _repository;
        private ApiDbContext _context;

        public RequestRepository(IRepository<Request> repository, ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
        {
            _repository = repository;
            _context = context;
        }

        public override Task<Request> GetId(string id)
        {
            //return base.GetId(id);
            return _repository.GetFilter(a => a.Id == id, a => a.RequestHistoryList);
        }

        public async Task<List<Request>> Seed(int count, int? max, string UserId = "Seed")
        {
            if (count <= 0)
                return default;

            var countExisintg = await _repository.CountAsync();
            if (max.HasValue && countExisintg >= max)
                return default;

            var list = new List<Request>();
            for (int i = 1; i <= count; i++)
            {
                var Request = new Request
                {
                    Note = $"Description {i}",
                    Status = $"Active",                    
                    RequestType = (i % 2 == 0) ? "Read Access" : "New Laptop",
                };
                if (countExisintg == 0)                
                    Request.Id = i.GetSimpleGuidString();
                Request.AddInitialHistory();
                list.Add(Request);
            }
            _repository.AddRange(list, UserId);
            await _repository.SaveChangesAsync();
            return list;
        }
    }
}
