namespace NetCoreBE.Api.Application.TicketFeature
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> Seed(int count, int? max, string UserId = "Seed");
    }
}
