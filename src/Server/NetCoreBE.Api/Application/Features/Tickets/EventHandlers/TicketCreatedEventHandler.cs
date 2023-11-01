// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class RequestCreatedEventHandler : INotificationHandler<CreatedEvent<Ticket>>
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

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        //will store in outbox table
        _timer.Stop();
        _logger.LogInformation("Domain Event: {DomainEvent},{ElapsedMilliseconds}ms", notification.GetType().FullName,
            _timer.ElapsedMilliseconds);
    }
}