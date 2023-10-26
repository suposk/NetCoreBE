namespace NetCoreBE.Api.Application.Features.Requests;

public interface IRequestLogic : IDomainLogicBase<Request, RequestDto>
{
    Task<RequestHistory> AddHistory(RequestHistory add);
}

public static class RequestLogicCache
{
    public static string GetListLogic = "GetListLogic";
    public static string GetIdLogic = "GetIdLogic";
}

public class RequestLogic : DomainLogicBase<Request, RequestDto>, IRequestLogic
{
    private readonly IRequestRepository _repository;
    private readonly IRepository<RequestHistory> _repositoryRequestHistory;
    private readonly ICacheProvider _cacheProvider;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private string _userId = null;

    public RequestLogic(
        //DbContext context, //can't create instance. use repository.DatabaseContext
        IApiIdentity apiIdentity,
        IDateTimeService dateTimeService,
        IMapper mapper,
        IRequestRepository repository,
        IMediator mediator,
        IRepository<RequestHistory> repositoryRequestHistory,
        ICacheProvider cacheProvider
        )
        : base(repository.DatabaseContext, apiIdentity, dateTimeService, mapper)
    {
        _repository = repository;
        _repositoryRequestHistory = repositoryRequestHistory;
        _cacheProvider = cacheProvider;
        _mediator = mediator;
        _userId = ApiIdentity.GetUserNameOrIp();
    }

    ////No Cache
    //public async override Task<RequestDto> GetIdLogic(string id)
    //{
    //    //return await base.GetIdLogic(id);
    //    var repo = await _repository.GetId(id).ConfigureAwait(false);
    //    return repo == null ? throw new NotFoundException($"{nameof(GetIdLogic)} id", id) : Mapper.Map<RequestDto>(repo);
    //}

    //With Cache
    public async override Task<RequestDto> GetIdLogic(string id)
    {
        var cache = _cacheProvider.GetFromCache<RequestDto>(RequestLogicCache.GetIdLogic, id);
        if (cache != null)
            return cache;
        else
        { 
            var repo = await _repository.GetId(id).ConfigureAwait(false);
            if (repo == null)
                throw new NotFoundException($"{nameof(GetIdLogic)} id", id);
            var mapped = Mapper.Map<RequestDto>(repo);
            _cacheProvider.SetCache(RequestLogicCache.GetIdLogic, id, mapped, 1 * 60 );
            return mapped;
        };
    }

    public override Task<Request> AddAsyncLogicEntity(Request entity, bool saveChanges = true)
    {
        if (entity == null)
            throw new BadRequestException($"{nameof(entity)} {nameof(AddAsyncLogicEntity)}");
        entity.AddInitialHistory();
        entity.Status = "Active";
        return base.AddAsyncLogicEntity(entity, saveChanges);
    }

    public async Task<RequestHistory> AddHistory(RequestHistory add)
    {
        if (add == null)
            throw new BadRequestException($"{nameof(add)} {nameof(AddHistory)}");
        var repo = await _repository.GetId(add.RequestId).ConfigureAwait(false);//verify if still exists in db
        if (repo == null)
            throw new NotFoundException($"{nameof(AddHistory)} add", add);

        if (repo.Status == "Closed")
            return default; //already closed, open a new request

        await _repositoryRequestHistory.AddAsync(add, add.CreatedBy);
        return add;
    }

    public async override Task<bool> RemoveAsyncLogic(string id, bool saveChanges = true)
    {
        if (await base.RemoveAsyncLogic(id, saveChanges: false))
        {
            var repoObj = await _repository.GetId(id).ConfigureAwait(false);
            foreach (var his in repoObj.RequestHistoryList)
            {
                _repositoryRequestHistory.Remove(his);
                his.AddDomainEvent(new DeletedEvent<RequestHistory>(his));
            }
            if (!saveChanges || await _repository.SaveChangesAsync())
                return true;

        }
        return false;
    }

}
