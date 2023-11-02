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
            //var type = notification.GetType().GetTypeNameExt();
            //var type = notification.GetType().FullName;

            //store in cache for performance
            var type = await _cacheProvider.GetOrAddAsync(nameof(TicketCreatedEventHandler), int.MaxValue, async () =>
            {
                var type = notification.GetType();
                return type;
            });
            var outboxMessage = OutboxMessageDomaintEvent.Create(notification.Entity.Id, _dateTimeService.UtcNow, type?.FullName, null, json);
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            if (await _repository.Exist(outboxMessage.Id, outboxMessage.Type))
            {
                _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", outboxMessage.Type, notification);
                return;
            }
            var res = await _repository.AddAsync(outboxMessage, "process");
            _logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketCreatedEventHandler)} failed", ex);
        }
    }
}