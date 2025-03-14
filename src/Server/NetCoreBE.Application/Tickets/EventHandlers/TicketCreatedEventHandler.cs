using CommonCleanArch.Domain;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketCreatedEventHandler(
    ICacheProvider cacheProvider,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeService dateTimeService,
    ILogger<TicketCreatedEventHandler> logger
    ) : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly ILogger<TicketCreatedEventHandler> _logger = logger;
    private readonly Stopwatch _timer = new Stopwatch();    

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        try
        {
            if (notification.Entity?.CreatedBy?.Contains("Seed") == true)
                return;

            _logger.LogInformation("Domain Event: {DomainEvent}, started", notification.GetType().FullName);
            _timer.Start();
            var type = notification.GetType();                        
            OutboxDomaintEvent? outboxMessage = null;
            var scope = _serviceScopeFactory.CreateScope();
            var outboxDomaintEventRepository = scope.ServiceProvider.GetRequiredService<IOutboxDomaintEventRepository>();

            //2. Process Stored Event
            var result = ResultCom.Success();
            IDbContextTransaction? dbTransaction = null;
            try
            {
                outboxMessage = await outboxDomaintEventRepository.GetId(notification.Id);
                if (outboxMessage is null)
                {
                    _logger.LogWarning("Process Domain Event: {DomainEvent} not found, {@notification}", type, notification);
                    return;
                }

                //simulate some processsing
                //await Task.Delay(10 * 1000, cancellationToken);

                var addHistory = TicketHistory.Create(ticketId: notification.Entity.Id, operation: "ConfimationSent", createdBy: $"{nameof(TicketCreatedEventHandler)}", details: $"Email sent to {notification.Entity.CreatedBy} for request {notification.Entity.Id}", null);

                //must use IServiceScopeFactory. context is already disposed.
                var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var ticket = await repository.GetId(addHistory.TicketId!).ConfigureAwait(false);
                if (ticket is null)
                {
                    _logger.LogWarning("Ticket not found for TicketId: {TicketId}", addHistory.TicketId);
                    result = ResultCom.Failure($"Ticket not found for TicketId: {addHistory.TicketId}");                    
                }

                if (result.IsSuccess && Ticket.CanAddUpdate(notification.Entity.Status) is false)
                {
                    _logger.LogWarning("Can't update Ticket, status is {status} for TicketId: {TicketId}", ticket?.Status, addHistory.TicketId);
                    result = ResultCom.Failure($"Can't update Ticket, status is {ticket?.Status} for TicketId: {addHistory.TicketId}");                    
                }

                if (result.IsFailure)
                {
                    //outboxMessage.SetToIgnored(_dateTimeService.UtcNow, result.ErrorMessage);
                    outboxMessage.SetFailed(_dateTimeService.UtcNow, result.ErrorMessage);
                    await outboxDomaintEventRepository.UpdateAsync(outboxMessage, nameof(TicketCreatedEventHandler));
                    return;
                }

                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await emailService.SendEmail("abc.com", "toemaul", "Ticket Confirmation", "This is confirmation of your request", true);

                var ticketHistoryRepository = scope.ServiceProvider.GetRequiredService<IRepository<TicketHistory>>();
                addHistory.AddDomainEvent(new CreatedEvent<TicketHistory>(addHistory));
                outboxMessage.SetProcessed(_dateTimeService.UtcNow);
                ticket?.Update(ticket.Status, "Email sent to user", _dateTimeService.UtcNow);

                dbTransaction = outboxDomaintEventRepository.GetTransaction(); //use transaction, to garanty consistency
                repository.UseTransaction(dbTransaction);
                ticketHistoryRepository.UseTransaction(dbTransaction);                

                await ticketHistoryRepository.AddAsync(addHistory, addHistory.CreatedBy);                
                await repository.UpdateAsync(ticket, nameof(TicketCreatedEventHandler));                
               // await outboxDomaintEventRepository.UpdateAsync(outboxMessage, nameof(TicketCreatedEventHandler)); //v1 set to processed                
                //await outboxDomaintEventRepository.RemoveAsync(outboxMessage?.Id); //v2 delete, keep only items that are not processed and move this to some history table
                dbTransaction.Commit();
                
                _logger.LogInformation("Domain Event: {DomainEvent} processed", type.FullName);
                _cacheProvider.ClearCacheOnlyKeyAndId(notification.Entity.GetPrimaryCacheKeyExt(), notification.Entity.Id); //ok clear cache for all Ids
            }
            catch (Exception ex)
            {
                dbTransaction?.Rollback();
                _logger.LogError(ex, null, ex);
            }
            finally 
            {
                //scope.Dispose(); //not needed
                dbTransaction?.Dispose();
                _timer.Stop();
                _logger.LogInformation("Domain Event: {DomainEvent}, end {ElapsedMilliseconds}ms", type.FullName, _timer.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}

/// <summary>
/// Exaple of additional handler for Domain Event, example for statistics
/// </summary>
/// <param name="cacheProvider"></param>
/// <param name="serviceScopeFactory"></param>
/// <param name="dateTimeService"></param>
/// <param name="logger"></param>
public class TicketCreatedEventStatHandler(
    ILogger<TicketCreatedEventHandler> logger
    ) : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger = logger;    

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Yield();
            _logger.LogInformation("Domain Event Stat: {DomainEvent}, started", notification.GetType().FullName);                       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}