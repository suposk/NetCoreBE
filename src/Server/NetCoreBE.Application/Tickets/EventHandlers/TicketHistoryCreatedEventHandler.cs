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
            //_cacheProvider.ClearCache(TicketCache.PrimaryCacheKey, TicketCache.GetId + notification?.Entity?.Id);
            //_cacheProvider.ClearCache(TicketCache.PrimaryCacheKey, TicketCache.GetList);            
            //_cacheProvider.ClearCacheForAllKeysAndIds(TicketCache.PrimaryCacheKey); //clear cache for all Ids       
            //_cacheProvider.ClearCacheForAllKeysAndIds(notification.GetPrimaryCacheKeyExt()); //clear cache for all Ids
            _cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //clear cache for all Ids
            _cacheProvider.ClearCacheOnlyKeyAndId(Ticket.EmptyTicket.GetPrimaryCacheKeyExt(), notification.Entity.TicketId); //clear cache for all Ids
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}