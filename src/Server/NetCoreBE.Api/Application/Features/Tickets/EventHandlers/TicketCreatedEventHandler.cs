// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<TicketCreatedEventHandler> _logger;

    public TicketCreatedEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeService dateTimeService,
        ILogger<TicketCreatedEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }

    public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
    {        
        try
        {
            //var json = JsonSerializer.Serialize(notification, new JsonSerializerOptions
            //{
            //    ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true
            //});
            //var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All}); //not safe
            //var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var type = notification.GetType().GetTypeNameExt();
            var outboxMessage = OutboxMessageDomaintEvent.Create(notification.Item.Id, _dateTimeService.UtcNow, type, null, json);
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            if (await _repository.Exist(outboxMessage.Id, outboxMessage.Type))
            {
                _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", outboxMessage.Type, notification);
                return;
            }
            var res = await _repository.AddAsync(outboxMessage);
            _logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketCreatedEventHandler)} failed", ex);
        }
    }
}