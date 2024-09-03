namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketHistoryCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<TicketHistoryCreatedEventHandler> logger
    ) : INotificationHandler<CreatedEvent<TicketHistory>>
{
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<TicketHistoryCreatedEventHandler> _logger = logger;

    public Task Handle(CreatedEvent<TicketHistory> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            _cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //clear history cache for all Ids
            _cacheProvider.ClearCacheOnlyKeyAndId(Ticket.EmptyTicket.GetPrimaryCacheKeyExt(), notification.Entity.TicketId); //clear Ticket cache for all Ids
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}