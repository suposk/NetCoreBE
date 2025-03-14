using CommonCleanArch.Domain;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace CommonCleanArch.Infrastructure.Interceptors;
public class OutboxDispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly IDateTimeService _dateTimeService;

    public OutboxDispatchDomainEventsInterceptor(IMediator mediator, IDateTimeService dateTimeService)
    {
        _mediator = mediator;
        _dateTimeService = dateTimeService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public record outPut(IReadOnlyCollection<IDomainEvent> domainEvents, string json);

    public async Task DispatchDomainEvents(DbContext? context, CancellationToken cancellationToken)
    {
        if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<EntityBase>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .Where(e => e is not IMemoryEvent)
            .SelectMany(entity =>
            {
                IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;
                domainEvents.ToList().ForEach(e => e.SetToProcess(entity.Id));
                return domainEvents;
            }).ToList();

        var domainEventsInMemory = domainEvents
            .Where(e => e is IMemoryEvent)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        List<OutboxDomaintEvent> OutboxDomaintEvents = [];
        foreach (var domainEvent in domainEvents)
        {
            string json = JsonConvert.SerializeObject(domainEvent, CaHelper.JsonSerializerSettingsNone);
            Type? type = domainEvent.GetType();
            OutboxDomaintEvent outboxMessage = OutboxDomaintEvent.Create(entityId: domainEvent.Id, _dateTimeService.UtcNow, type?.FullName, json);
            outboxMessage.Id ??= StringHelper.GetStringGuidExt();
            outboxMessage.CreatedAt ??= _dateTimeService.UtcNow;
            OutboxDomaintEvents.Add(outboxMessage);
        }
        if (OutboxDomaintEvents.Any())
            context.Set<OutboxDomaintEvent>().AddRange(OutboxDomaintEvents);

        //only publish in memory events
        foreach (var domainEvent in domainEventsInMemory)
            await _mediator.Publish(domainEvent, cancellationToken);
    }

}

