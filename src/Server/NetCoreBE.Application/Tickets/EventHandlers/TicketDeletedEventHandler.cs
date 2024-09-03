namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<TicketDeletedEventHandler> logger
    ) : INotificationHandler<DeletedEvent<Ticket>>
{
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<TicketDeletedEventHandler> _logger = logger;

    public Task Handle(DeletedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            //_cacheProvider.ClearCacheOnlyKeyAndId(notification.GetPrimaryCacheKeyExt(), notification.Entity.Id); wrong cache key
            _cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //ok clear cache for all Ids
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}