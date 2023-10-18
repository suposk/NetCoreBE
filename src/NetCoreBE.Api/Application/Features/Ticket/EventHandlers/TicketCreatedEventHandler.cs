// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<CreatedEvent<Ticket>>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;
    private readonly Stopwatch _timer;

    public TicketCreatedEventHandler(
        ILogger<TicketCreatedEventHandler> logger
    )
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task Handle(CreatedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        await Task.Delay(5000, cancellationToken);
        _timer.Stop();
        _logger.LogInformation("Domain Event: {DomainEvent},{ElapsedMilliseconds}ms", notification.GetType().FullName,
            _timer.ElapsedMilliseconds);
    }
}