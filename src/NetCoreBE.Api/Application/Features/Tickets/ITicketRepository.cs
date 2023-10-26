namespace NetCoreBE.Api.Application.Features.Tickets
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> Seed(int count, int? max, string UserId = "Seed");
    }
}
