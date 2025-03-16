namespace NetCoreBE.Application.Tickets.IntegrationEvents;

public sealed class TicketCanceledIntegrationEvent : IntegrationEvent
{
    public TicketCanceledIntegrationEvent(Guid id, DateTime occurredOnUtc, string ticketId)
        : base(id, occurredOnUtc)
    {
        TicketId = ticketId;
    }
    public string TicketId { get; init; }
}