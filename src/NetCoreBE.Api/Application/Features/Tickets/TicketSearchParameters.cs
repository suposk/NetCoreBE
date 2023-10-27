namespace NetCoreBE.Api.Application.Features.Tickets
{
    public sealed class TicketSearchParameters :  ResourceParameters
    {        
        public string? Description { get; set; }
        //public override string OrderBy { get; set; } = nameof(Ticket.Id);
    }
}
