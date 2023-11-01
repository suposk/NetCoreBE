// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class TicketCreatedGenericEventHandler : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<TicketCreatedGenericEventHandler> _logger;    

    public TicketCreatedGenericEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeService dateTimeService,
        ILogger<TicketCreatedGenericEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeService = dateTimeService;
        _logger = logger;        
    }

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {        
        try
        {            
            var json = JsonSerializer.Serialize(notification, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true
            });
            var type = notification.GetType().GetTypeNameExt();
            var outboxMessage = OutboxMessageDomaintEvent.Create(notification.Entity.Id, _dateTimeService.UtcNow, type, null, json);
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            if (await _repository.Exist(outboxMessage.Id, outboxMessage.Type))
            {
                _logger.LogWarning("Domain Event: {DomainEvent} already exist", outboxMessage.Type);
                return;
            }
            var res = await _repository.AddAsync(outboxMessage);
            _logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketCreatedGenericEventHandler)} failed", ex);
        }
    }
}