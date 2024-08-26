using CommonCleanArch.Application.Services;

namespace NetCoreBE.Application.Tickets;

public interface ITicketRepositoryDecorator : IRepositoryDecoratorBase<Ticket, TicketDto>
{
}

//public class TicketCache
//{
//    public static string PrimaryCacheKey = nameof(TicketCache);
//    public static string GetList = nameof(ITicketRepositoryDecorator.GetListDto);
//    public static string GetId = nameof(ITicketRepositoryDecorator.GetIdDto);
//}

public class TicketRepositoryDecorator : RepositoryDecoratorBase<Ticket, TicketDto>, ITicketRepositoryDecorator
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IRepository<TicketHistory> _repositoryTicketHistory;

    public TicketRepositoryDecorator(
        IDateTimeService dateTimeService,
        IRepository<TicketHistory> repositoryTicketHistory,
        ITicketRepository repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory logger,
        ICacheProvider cacheProvider)
        : base(repository, apiIdentity, mapper, logger, cacheProvider)
    {
        _dateTimeService = dateTimeService;
        _repositoryTicketHistory = repositoryTicketHistory;
    }

    //public override string PrimaryCacheKey => TicketCache.PrimaryCacheKey;
    public override int CacheDurationMinutes => 5;

    string? _Name;
    public override string Name => _Name ??= ToString()!;        

    protected override Task<bool> CanModifyFunc(Ticket entity) => base.CanModifyFunc(entity);

    public async override Task<ResultCom<Ticket>> AddEntityAsync(Ticket entity, bool saveChanges = true)
    {
        entity?.Init(_dateTimeService.UtcNow);
        return await base.AddEntityAsync(entity, saveChanges);        
    }

    public async override Task<ResultCom> RemoveAsync(string id, bool saveChanges = true)
    {
        //must remove all history for id
        //first mark request as deleted
        var res = await base.RemoveAsync(id, saveChanges: false);
        if (res.IsFailure)
            return res;
        else
        {
            var repoObj = await Repository.GetId(id).ConfigureAwait(false);
            foreach (var history in repoObj.TicketHistoryList)
            {
                _repositoryTicketHistory.Remove(history);
                //may be not needed here, just example
                history.AddDomainEvent(new DeletedEvent<TicketHistory>(history));
            }
            //remove reqquest & all history in one transaction
            if (saveChanges && await Repository.SaveChangesAsync())
                return ResultCom.Success();
            else
                //should not happen
                return ResultCom.Failure($"Failed to {nameof(RemoveAsync)} {id}", HttpStatusCode.InternalServerError);
        }        
    }

}
