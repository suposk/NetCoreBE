namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketDeletedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    ICacheProvider cacheProvider,
    ILogger<TicketDeletedEventHandler> logger
    ) : INotificationHandler<DeletedEvent<Ticket>>
{
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<TicketDeletedEventHandler> _logger = logger;

    public async Task Handle(DeletedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            //_cacheProvider.ClearCacheOnlyKeyAndId(notification.GetPrimaryCacheKeyExt(), notification.Entity.Id); wrong cache key
            _cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //ok clear cache for all Ids
                        
            #region not needed in most cases

            //var _repository = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxDomaintEventRepository>();
            //var repo = await _repository.GetUnProcessedList(entityId: notification.Entity.Id);
            //if (repo.IsNullOrEmptyCollection())
            //{
            //    logger.LogInformation("Domain Event: already removed, {@notification}", notification.Entity);
            //    return;
            //}
            //repo.ForEach(a => _repository.Remove(a));
            //var res = await _repository.SaveChangesAsync(); 

            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }        
    }
}