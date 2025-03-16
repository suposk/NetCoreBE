namespace NetCoreBE.Application.Tickets;

///// <summary>
///// V1. Replaced IRepository<Ticket> with  IRequestRepository
///// </summary>
//public class GetByIdQueryHandlerRequest(IRequestRepository repository, IApiIdentity apiIdentity, IMapper mapper, ILoggerFactory loggerFactory, ICacheProvider cacheProvider)
//    : GetByIdQueryHandler<Ticket, RequestDto>(repository, apiIdentity, mapper, loggerFactory, cacheProvider)
//{
//    public override TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
//}


/// <summary>
/// v2. Added IRepository<Ticket> with  IRequestRepository to constructor and override GetEntity
/// </summary>
public class GetByIdQueryHandlerRequest(IMediator mediator, ITicketRepository repository, IRepository<Ticket> repositoryGeneric, IApiIdentity apiIdentity, IMapper mapper, ILoggerFactory loggerFactory, ICacheProvider cacheProvider) 
    : GetByIdQueryHandler<Ticket, TicketDto>(repositoryGeneric, apiIdentity, mapper, loggerFactory, cacheProvider)
{
    public override Task<Ticket> GetById(string id) => repository.GetId(id);

    /// <summary>
    /// Override this to add custom logic before execution, To avoid caching problems.
    /// Sent event to get ticket stat
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public override async ValueTask OnStart(string Id) => 
        await mediator.Publish(new GetSingleTicketEventStat(new TicketStat(Id, ApiIdentity.GetUserNameOrIp())));

    public override TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
}