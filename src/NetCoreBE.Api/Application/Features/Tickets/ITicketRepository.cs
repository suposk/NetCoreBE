namespace NetCoreBE.Api.Application.Features.Tickets
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<PagedList<Ticket>> Search(TicketSearchParameters ticketSearchParameters);
        Task<List<Ticket>> Seed(int count, int? max, string UserId = "Seed");
    }
}
