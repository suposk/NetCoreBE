namespace NetCoreBE.Application.OldTickets;

public interface IOldTicketRepositoryDecorator : IRepositoryDecoratorBase<OldTicket, OldTicketDto>
{
    
}

//public class TicketCache
//{
//    public static string PrimaryCacheKey = nameof(TicketCache);
//    public static string GetList = nameof(ITicketRepositoryDecorator.GetListDto);
//    public static string GetId = nameof(ITicketRepositoryDecorator.GetIdDto);
//}

public class OldTicketRepositoryDecorator : RepositoryDecoratorBase<OldTicket, OldTicketDto>, IOldTicketRepositoryDecorator
{
    public OldTicketRepositoryDecorator(
        IPublisher publisher,
        IDateTimeService dateTimeService,
        IOldTicketRepository repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory logger,
        ICacheProvider cacheProvider)
        : base(repository, apiIdentity, mapper, logger, cacheProvider)
    {
        _publisher = publisher;
        _dateTimeService = dateTimeService;
    }

    string? _Name;
    public override string Name => _Name ??= ToString()!;

    //public override string PrimaryCacheKey => TicketCache.PrimaryCacheKey;
    public override int CacheDurationMinutes => 5;
    
    private readonly IPublisher _publisher;
    private readonly IDateTimeService _dateTimeService;

    public async override Task<ResultCom<List<OldTicketDto>>> GetListDto()
    {
        await _publisher?.Publish(new GetTicketsEvent(new GetTickets(ApiIdentity.GetUserNameOrIp(), _dateTimeService.UtcNow))); //Example track request for example audit purpose
        return await base.GetListDto();
    }

    protected override Task<bool> CanModifyFunc(OldTicket entity) => base.CanModifyFunc(entity);

    public override Task<ResultCom<OldTicket>> AddEntityAsync(OldTicket entity, bool saveChanges = true)
    {
        entity.SetNewTicket();
        return base.AddEntityAsync(entity, saveChanges);
    }

}
