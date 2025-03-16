﻿using NetCoreBE.Application.Tickets.IntegrationEvents;

namespace NetCoreBE.Infrastructure.EventBus
{
    public class TicketCanceledIntegrationEventConsumer(ILogger<TicketCanceledIntegrationEventConsumer> logger) 
        : IConsumer<TicketCanceledIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<TicketCanceledIntegrationEvent> context)
        {
            logger.LogInformation("TicketCanceledIntegrationEventConsumer: {DomainEvent}, started = {@value}", context.Message.GetType().FullName, context.Message);
            
            //throw new BusConsumeException($"Test exception {context.MessageId} {context.Message.TicketId} {context.Message}");
            await Task.CompletedTask;
            return;

            //throw new NotImplementedException();
        }
    }
}
