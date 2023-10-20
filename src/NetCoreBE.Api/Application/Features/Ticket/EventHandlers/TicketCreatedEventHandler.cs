// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace NetCoreBE.Api.Application.Features.Ticket.EventHandlers;

public class RequestCreatedEventHandler : INotificationHandler<CreatedEvent<Entities.Ticket>>
{
    private readonly ILogger<RequestCreatedEventHandler> _logger;
    private readonly Stopwatch _timer;

    public RequestCreatedEventHandler(
        ILogger<RequestCreatedEventHandler> logger
    )
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task Handle(CreatedEvent<Entities.Ticket> notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        await Task.Delay(5000, cancellationToken);
        _timer.Stop();
        _logger.LogInformation("Domain Event: {DomainEvent},{ElapsedMilliseconds}ms", notification.GetType().FullName,
            _timer.ElapsedMilliseconds);
    }
}