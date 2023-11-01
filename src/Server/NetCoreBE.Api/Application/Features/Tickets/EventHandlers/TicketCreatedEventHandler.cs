// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class RequestCreatedEventHandler : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly IOutboxMessageDomaintEventRepository _repository;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<RequestCreatedEventHandler> _logger;
    private readonly Stopwatch _timer;

    public RequestCreatedEventHandler(
        IOutboxMessageDomaintEventRepository repository,
        IDateTimeService dateTimeService,
        ILogger<RequestCreatedEventHandler> logger
    )
    {
        _repository = repository;
        _dateTimeService = dateTimeService;
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        try
        {            
            var json = JsonSerializer.Serialize(notification.Entity, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            });
            var outboxMessage = OutboxMessageDomaintEvent.Create(_dateTimeService.UtcNow, notification.GetType().Name, null, json);
            var res = await _repository.AddAsync(outboxMessage);
            _timer.Stop();
            _logger.LogInformation("Domain Event: {DomainEvent},{ElapsedMilliseconds}ms", notification.GetType().FullName, _timer.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(RequestCreatedEventHandler)} failed", ex);
        }
        _timer.Stop();
    }
}