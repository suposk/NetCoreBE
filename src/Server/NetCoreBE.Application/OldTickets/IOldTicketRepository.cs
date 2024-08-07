using CommonCleanArch.Domain;

namespace NetCoreBE.Application.OldTickets
{
    public interface IOldTicketRepository : IRepository<OldTicket>
    {
        Task<PagedList<OldTicket>> Search(TicketSearchParameters ticketSearchParameters);
        Task<List<OldTicket>> Seed(int addCount, int? MaxInDb, string UserId = "Seed");
    }
}
