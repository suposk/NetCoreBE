namespace NetCoreBE.Api.Application.RequestFeature;

public interface IRequestLogic : IDomainLogicBase<Request, RequestDto>
{
    Task<RequestHistory> AddHistory(RequestHistory add);
}

public class RequestLogic : DomainLogicBase<Request, RequestDto>, IRequestLogic
{
    private readonly IRequestRepository _repository;
    private readonly IRepository<RequestHistory> _repositoryRequestHistory;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ApiDbContext? _context;

    public RequestLogic(
        DbContext context,
        IApiIdentity apiIdentity,
        IDateTimeService dateTimeService,
        IMapper mapper,
        IRequestRepository repository,
        IMediator mediator,
        IRepository<RequestHistory> repositoryRequestHistory
        )
        : base(context, apiIdentity, dateTimeService, mapper)
    {
        _repository = repository;
        _repositoryRequestHistory = repositoryRequestHistory;
        _mediator = mediator;
        _context = context as ApiDbContext;
    }

    public async override Task<RequestDto> GetIdLogic(string id)
    {
        //return await base.GetIdLogic(id);
        var repo = await _repository.GetId(id).ConfigureAwait(false);
        return repo == null ? throw new NotFoundException($"{nameof(GetIdLogic)} id", id) : Mapper.Map<RequestDto>(repo);
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

        //verify if still exists in db
        var repo = await _repository.GetId(add.RequestId).ConfigureAwait(false);
        if (repo == null)
            throw new NotFoundException($"{nameof(AddHistory)} add", add);

        if (repo.Status == "Closed")
            return default; //already closed, open a new request

        await _repositoryRequestHistory.AddAsync(add, add.CreatedBy);
        return add;
    }

    public async override Task<bool> RemoveAsyncLogic(string id, bool saveChanges = true)
    {
        if(await base.RemoveAsyncLogic(id, saveChanges: false))
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
