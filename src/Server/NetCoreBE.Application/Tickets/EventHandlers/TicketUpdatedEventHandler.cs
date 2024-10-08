namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<TicketUpdatedEventHandler> logger
    ) : INotificationHandler<UpdatedEvent<Ticket>>
{
    public Task Handle(UpdatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);            
            //cacheProvider.ClearCacheOnlyKeyAndId(notification.GetPrimaryCacheKeyExt(), notification.Entity.Id); //wrong cache key
            cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //ok clear cache for all Ids
            cacheProvider.ClearCacheOnlyKeyAndId(Ticket.EmptyTicket.GetPrimaryCacheKeyExt(), notification.Entity.Id); //ok clear cache for all Ids
        }
        catch (Exception ex)
        {
            logger.LogError(ex, null, ex);
        }
        return Task.CompletedTask;
    }
}