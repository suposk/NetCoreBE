using CommonCleanArch.Infrastructure;
using CommonCleanArch.Infrastructure.Persistence;
using System.Diagnostics;

namespace NetCoreBE.Application.Tickets.EventHandlers;

public class TicketCreatedEventHandler(
        ICacheProvider cacheProvider,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<TicketCreatedEventHandler> logger
    ) : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
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
            //simulate some processsing
            await Task.Delay(20 * 1000, cancellationToken);

            var scope = _serviceScopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var add = new TicketHistory(ticketId: notification.Entity.Id, operation: "AwaitingConfirmation", createdBy: $"{nameof(TicketCreatedEventHandler)}", details : $"Email sent to {notification.Entity.CreatedBy} for request {notification.Entity.Id}", null);            

            //simulate email service
            await emailService.SendEmail("abc.com", "toemaul", "Ticket Confirmation", "Please confirm your request", true);

            //must use IServiceScopeFactory. context is already disposed.
            var RequestRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            var request = await RequestRepository.GetId(add.TicketId).ConfigureAwait(false);
            if (request is null)
            {
                _logger.LogWarning("Ticket not found for RequestId: {RequestId}", add.TicketId);
                return;
            }
            if (request.CanAddHistory() is false)
            {
                _logger.LogWarning("Ticket status is Closed for RequestId: {RequestId}", add.TicketId);
                return;
            }

            var RequestHistoryRepository = scope.ServiceProvider.GetRequiredService<IRepository<TicketHistory>>();
            add.AddDomainEvent(new CreatedEvent<TicketHistory>(add));
            await RequestHistoryRepository.AddAsync(add, add.CreatedBy);

            _cacheProvider.ClearCacheOnlyKeyAndId(notification.GetPrimaryCacheKeyExt(), notification.Entity.Id); //clear cache for all Ids

            _timer.Stop();
            _logger.LogInformation("Domain Event: {DomainEvent}, end {ElapsedMilliseconds}ms", notification.GetType().FullName,
                _timer.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, null, ex);
        }
    }
}