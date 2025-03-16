using CommonCleanArch.Application.EventBus;
using MassTransit;

namespace CommonCleanArch.Infrastructure.Infrastructure.EventBus;

public sealed class EventBus(IBus bus) : IEventBus
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await bus.Publish(integrationEvent, cancellationToken);
    }
}
