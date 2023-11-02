// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<TicketCreatedEventHandler> _logger;    

    public TicketCreatedEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeService dateTimeService,
        ICacheProvider cacheProvider,
        ILogger<TicketCreatedEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeService = dateTimeService;
        _cacheProvider = cacheProvider;
        _logger = logger;        
    }

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        //return;
        try
        {            
            var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            //store in cache for performance
            var type = await _cacheProvider.GetOrAddAsync(nameof(TicketCreatedEventHandler), int.MaxValue, async () =>
            {
                return notification.GetType();                
            });            
            OutboxMessageDomaintEvent? outboxMessage = null;
            var _outboxMessageRepository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            
            //Process Stored Event
            if (notification.IsProcessing && notification.Id.HasValueExt())
            {
                outboxMessage = await _outboxMessageRepository.GetId(notification.Id);
                if (outboxMessage == null)
                {
                    _logger.LogWarning("Process Domain Event: {DomainEvent} not found, {@notification}", type, notification);
                    return;
                }
                _logger.LogWarning("Process Domain Event: {DomainEvent} already stored, {@notification}", outboxMessage.Type, notification);
                var mediator = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
                var command = new ConfrimCommand { TicketId = notification.Entity.Id };
                var result = await mediator.Send(command);
                if (result == false)
                {
                    _logger.LogWarning("Process Command: {Command} failed to confirm", @command);
                    outboxMessage.SetFailed(_dateTimeService.UtcNow, $"SOME ERROR");
                }
                else
                {
                    _logger.LogWarning("Process Command: {Command} sussess", @command);
                    outboxMessage.SetProcessed(_dateTimeService.UtcNow);
                }
                await _outboxMessageRepository.UpdateAsync(outboxMessage, nameof(TicketCreatedEventHandler));
                return;
                
            }
            else
            {
                //Store event to OutboxMessageDomaintEvent process later
                //check if exist
                if (await _outboxMessageRepository.Exist(entityId: notification.Entity.Id, type.FullName))
                {
                    _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", type, notification);
                    return;
                }
                outboxMessage = OutboxMessageDomaintEvent.Create(entityId: notification.Entity.Id, _dateTimeService.UtcNow, type?.FullName, json);
                var res = await _outboxMessageRepository.AddAsync(outboxMessage, nameof(TicketCreatedEventHandler));
                _logger.LogDebug("Domain Event: {DomainEvent}", type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketCreatedEventHandler)} failed", ex);
        }
    }
}