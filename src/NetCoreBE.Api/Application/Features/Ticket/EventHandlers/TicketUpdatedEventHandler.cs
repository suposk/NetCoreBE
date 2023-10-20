// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace NetCoreBE.Api.Application.Features.Ticket.EventHandlers;

public class TicketUpdatedEventHandler : INotificationHandler<UpdatedEvent<Entities.Ticket>>
{
    private readonly ILogger<TicketUpdatedEventHandler> _logger;

    public TicketUpdatedEventHandler(
        ILogger<TicketUpdatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(UpdatedEvent<Entities.Ticket> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);

        return Task.CompletedTask;
    }
}