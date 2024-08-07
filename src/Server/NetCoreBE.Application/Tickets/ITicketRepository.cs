namespace NetCoreBE.Application.Tickets;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<List<Ticket>> Seed(int addCount, int? MaxInDb, string UserId = "Seed");
}
