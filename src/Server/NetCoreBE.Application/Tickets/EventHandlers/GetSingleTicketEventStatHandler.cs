namespace NetCoreBE.Application.Tickets.EventHandlers;

public class GetSingleTicketEventStatHandler(    
    ILogger<GetSingleTicketEventStatHandler> logger
    ) : INotificationHandler<GetSingleTicketEventStat>
{    
    private readonly ILogger<GetSingleTicketEventStatHandler> _logger = logger;

    public async Task Handle(GetSingleTicketEventStat notification, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException("Testing publisher"); //testing publisher
        try
        {
            _logger.LogInformation("Domain Event: {DomainEvent}, started = {@value}", notification.GetType().FullName, notification.Item);
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