// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace NetCoreBE.Application.OldTickets.EventHandlers;

public class OldTicketCreatedExampleEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeService dateTimeService,
    ICacheProvider cacheProvider,
    ILogger<OldTicketCreatedExampleEventHandler> logger
    ) : INotificationHandler<OldTicketCreatedExampleEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<OldTicketCreatedExampleEventHandler> _logger = logger;

    public async Task Handle(OldTicketCreatedExampleEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var type = notification.GetType();

            var outboxMessage = OutboxDomaintEvent.Create(entityId: notification.Item.Id, _dateTimeService.UtcNow, type?.FullName, json);
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxDomaintEventRepository>();
            if (await _repository.Exist(entityId: outboxMessage.EntityId, outboxMessage.Type))
            {
                _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", outboxMessage.Type, notification);
                return;
            }
            var res = await _repository.AddAsync(outboxMessage, nameof(OldTicketCreatedExampleEventHandler));
            _logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(OldTicketCreatedExampleEventHandler)} failed", ex);
        }
    }
}