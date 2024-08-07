namespace NetCoreBE.Application.OldTickets.EventHandlers;

public class OldTicketDeletedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeService dateTimeService,
    ICacheProvider cacheProvider,
    ILogger<OldTicketDeletedEventHandler> logger
    ) : INotificationHandler<DeletedEvent<OldTicket>>
{
    public async Task Handle(DeletedEvent<OldTicket> notification, CancellationToken cancellationToken)
    {
        //return;
        try
        {
            var _repository = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxDomaintEventRepository>();
            var repo = await _repository.GetUnProcessedList(entityId: notification.Entity.Id);
            if (repo.IsNullOrEmptyCollection())
            {
                logger.LogInformation("Domain Event: already removed, {@notification}", notification.Entity);
                return;
            }
            repo.ForEach(a => _repository.Remove(a));
            var res = await _repository.SaveChangesAsync();
            logger.LogDebug("Domain Event completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{nameof(OldTicketDeletedEventHandler)} failed", ex);
        }
    }
}