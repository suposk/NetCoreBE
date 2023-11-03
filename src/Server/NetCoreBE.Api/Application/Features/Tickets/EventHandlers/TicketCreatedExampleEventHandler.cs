// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class TicketCreatedExampleEventHandler : INotificationHandler<TicketCreatedExampleEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<TicketCreatedExampleEventHandler> _logger;

    public TicketCreatedExampleEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeService dateTimeService,
        ICacheProvider cacheProvider,
        ILogger<TicketCreatedExampleEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeService = dateTimeService;
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task Handle(TicketCreatedExampleEvent notification, CancellationToken cancellationToken)
    {        
        try
        {
            var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            //store in cache for performance
            var type = await _cacheProvider.GetOrAddAsync(nameof(TicketCreatedExampleEventHandler), int.MaxValue, async () =>
            {
                return notification.GetType();
            });

            var outboxMessage = OutboxMessageDomaintEvent.Create(entityId: notification.Item.Id, _dateTimeService.UtcNow, type?.FullName, json);
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            if (await _repository.Exist(entityId: outboxMessage.EntityId, outboxMessage.Type))
            {
                _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", outboxMessage.Type, notification);
                return;
            }
            var res = await _repository.AddAsync(outboxMessage, nameof(TicketCreatedExampleEventHandler));
            _logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketCreatedExampleEventHandler)} failed", ex);
        }
    }
}