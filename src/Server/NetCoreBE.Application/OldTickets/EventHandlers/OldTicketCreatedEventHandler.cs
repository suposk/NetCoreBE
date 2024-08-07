// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace NetCoreBE.Application.OldTickets.EventHandlers;

public class OldTicketCreatedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeService dateTimeService,
    ICacheProvider cacheProvider,
    ILogger<OldTicketCreatedEventHandler> logger
    ) : INotificationHandler<CreatedEvent<OldTicket>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<OldTicketCreatedEventHandler> _logger = logger;

    public async Task Handle(CreatedEvent<OldTicket> notification, CancellationToken cancellationToken)
    {
        //return;
        try
        {
            if (notification.Entity?.CreatedBy?.Contains("Seed") == true)
                return;

            var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var type = notification.GetType();
            OutboxDomaintEvent? outboxMessage = null;
            var _outboxDomaintEventRepository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxDomaintEventRepository>();

            //Process Stored Event
            if (notification.IsProcessing && notification.Id.HasValueExt())
            {
                outboxMessage = await _outboxDomaintEventRepository.GetId(notification.Id);
                if (outboxMessage == null)
                {
                    _logger.LogWarning("Process Domain Event: {DomainEvent} not found, {@notification}", type, notification);
                    return;
                }
                _logger.LogWarning("Process Domain Event: {DomainEvent} already stored, {@notification}", outboxMessage.Type, notification);
                var mediator = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
                var command = new ConfrimCommand { TicketId = notification.Entity.Id };
                var result = await mediator.Send(command);
                if (result.IsSuccess == false)
                {
                    _logger.LogWarning("Process Command: {Command} failed to confirm", @command);
                    outboxMessage.SetFailed(_dateTimeService.UtcNow, result.ErrorMessage);
                }
                else
                {
                    _logger.LogWarning("Process Command: {Command} sussess", @command);
                    outboxMessage.SetProcessed(_dateTimeService.UtcNow);
                }
                await _outboxDomaintEventRepository.UpdateAsync(outboxMessage, nameof(OldTicketCreatedEventHandler));
                return;

            }
            else
            {
                //Store event to OutboxDomaintEvent process later
                //check for duplicate messages
                if (await _outboxDomaintEventRepository.Exist(entityId: notification.Entity.Id, type.FullName))
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", type, notification);
                    return;
                }
                outboxMessage = OutboxDomaintEvent.Create(entityId: notification.Entity.Id, _dateTimeService.UtcNow, type?.FullName, json);
                var res = await _outboxDomaintEventRepository.AddAsync(outboxMessage, nameof(OldTicketCreatedEventHandler));
                _logger.LogDebug("Domain Event: {DomainEvent}", type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(OldTicketCreatedEventHandler)} failed", ex);
        }
    }
}