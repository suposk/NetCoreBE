// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class TicketDeletedEventHandler : INotificationHandler<DeletedEvent<Ticket>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<TicketDeletedEventHandler> _logger;    

    public TicketDeletedEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeService dateTimeService,
        ICacheProvider cacheProvider,
        ILogger<TicketDeletedEventHandler> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeService = dateTimeService;
        _cacheProvider = cacheProvider;
        _logger = logger;        
    }

    public async Task Handle(DeletedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        //return;
        try
        {            
            var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            var repo = await _repository.GetId(notification.Entity.Id);
            if (repo == null)
            {
                _logger.LogInformation("Domain Event: already removed, {@notification}", notification.Entity);
                return;
            }
            var res = await _repository.RemoveAsync(notification.Entity.Id);
            _logger.LogDebug("Domain Event completed");

            //var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            ////store in cache for performance
            //var type = await _cacheProvider.GetOrAddAsync(nameof(TicketDeletedEventHandler), int.MaxValue, async () =>
            //{
            //    var type = notification.GetType();
            //    return type;
            //});
            //var outboxMessage = OutboxMessageDomaintEvent.Create(notification.Entity.Id, _dateTimeService.UtcNow, type?.FullName, null, json);
            //var _repository = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxMessageDomaintEventRepository>();
            //if (await _repository.Exist(outboxMessage.Id, outboxMessage.Type))
            //{
            //    _logger.LogWarning("Domain Event: {DomainEvent} already exist, {@notification}", outboxMessage.Type, notification);
            //    return;
            //}
            //var res = await _repository.AddAsync(outboxMessage);
            //_logger.LogDebug("Domain Event: {DomainEvent}", type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TicketDeletedEventHandler)} failed", ex);
        }
    }
}