namespace NetCoreBE.Application.Tickets.EventHandlers;

public class GetSingleTicketEventStatHandler(    
    ILogger<GetSingleTicketEventStatHandler> logger
    ) : INotificationHandler<GetSingleTicketEventStat>
{    
    private readonly ILogger<GetSingleTicketEventStatHandler> _logger = logger;

    public async Task Handle(GetSingleTicketEventStat notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started with = {@value}", notification.GetType().FullName, notification);
            await Task.CompletedTask;

            #region not needed in most cases


            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}