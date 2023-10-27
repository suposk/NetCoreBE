namespace NetCoreBE.Api.Application.Features.Tickets
{
    /// <summary>
    /// Inherts from ResourceParameters
    /// </summary>
    public sealed class TicketSearchParameters :  ResourceParameters
    {        
        public string? Description { get; set; }
        //public override string OrderBy { get; set; } = nameof(Ticket.Id);
    }
}
