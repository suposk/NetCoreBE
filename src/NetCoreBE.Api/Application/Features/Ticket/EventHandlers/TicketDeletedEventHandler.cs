// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class TicketDeletedEventHandler : INotificationHandler<DeletedEvent<Ticket>>
{
    private readonly ILogger<TicketDeletedEventHandler> _logger;

    public TicketDeletedEventHandler(
        ILogger<TicketDeletedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Ticket> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}