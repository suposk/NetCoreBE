using CommonCleanArch;

namespace NetCoreBE.Domain.Test;

public abstract class BaseTest
{
    public static T AssertDomainEventWasPublished<T>(EntityBase entity)
        where T : DomainEvent
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();
        if (domainEvent is null)
            //throw new Exception($"{typeof(T).Name} was not published");        
            throw new Exception($"{typeof(T).Name} was not triggered");

        return domainEvent;
    }

    public static readonly DateTime UtcNow = new DateTime(2021, 1, 1, 1, 1, 1);    
}
