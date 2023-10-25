namespace NetCoreBE.Api.Application.RequestFeature;

public interface IRequestRepository : IRepository<Request>
{
    Task<List<Request>> Seed(int count, int? max, string UserId = "Seed");
}
