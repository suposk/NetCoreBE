﻿namespace NetCoreBE.Api.Application.Features.Tickets.EventHandlers;

public class GetTicketsEventHandler : INotificationHandler<GetTicketsEvent>
{
    public async Task Handle(GetTicketsEvent notification, CancellationToken cancellationToken)
    {
        notification.IsPublished = true;
        var par = notification.Item;
        await Task.Delay(1000);
        return;
    }
}