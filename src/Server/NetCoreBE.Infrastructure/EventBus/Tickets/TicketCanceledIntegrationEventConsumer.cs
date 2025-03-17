using NetCoreBE.Application.Tickets.IntegrationEvents;

namespace NetCoreBE.Infrastructure.EventBus.Tickets
{
    public class TicketCanceledIntegrationEventConsumer(
        IMediator mediator,
        ILogger<TicketCanceledIntegrationEventConsumer> logger) 
        : IConsumer<TicketCanceledIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<TicketCanceledIntegrationEvent> context)
        {
            logger.LogInformation("TicketCanceledIntegrationEventConsumer: {DomainEvent}, started = {@value}", context.Message.GetType().FullName, context.Message);

            //throw new BusConsumeException($"Test exception {context.MessageId} {context.Message.TicketId} {context.Message}");sitory.UpdateAsync(ticket, nameof(TicketCanceledIntegrationEventConsumer));            
            
            var res = await mediator.Send(new UpdateTicketStatusCommand { TicketId = context.Message.TicketId, StatusEnum = StatusEnum.Cancelled });
            if (res.IsFailure)
            {
                logger.LogWarning("TicketCanceledIntegrationEventConsumer: Entity with id {Id} not updated, Error = {er}", context.Message.TicketId, res.ErrorMessage);
                return;
            }
        }
    }
}
