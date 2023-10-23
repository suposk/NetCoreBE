using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Application.Features.Request;

public interface IRequestLogic : IDomainLogicBase<Entities.Request, RequestDto>
{
    Task<RequestHistory> AddHistory(RequestHistory add);
}

public class RequestLogic : DomainLogicBase<Entities.Request, RequestDto>, IRequestLogic
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

    public override Task<Entities.Request> AddAsyncLogicEntity(Entities.Request entity)
    {
        if (entity == null)
            throw new BadRequestException($"{nameof(entity)} {nameof(AddAsyncLogicEntity)}");
        entity.AddInitialHistory();
        entity.Status = "Active";        
        entity.AddDomainEvent(new CreatedEvent<Entities.Request>(entity));
        return AddAsync(entity);
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

    public async override Task<bool> RemoveAsyncLogic(string id)
    {
        if (id.IsNotNullValidIdExt())
            throw new BadRequestException($"{nameof(id)} {nameof(RemoveAsyncLogic)}");

        var repoObj = await _repository.GetId(id).ConfigureAwait(false);
        if (repoObj == null)
            throw new NotFoundException($"{nameof(RemoveAsyncLogic)}", id);

        if (await CanModify(repoObj, CanModifyFunc).ConfigureAwait(false))
        {
            Remove(repoObj);
            repoObj.AddDomainEvent(new DeletedEvent<Entities.Request>(repoObj));
            foreach(var his in repoObj.RequestHistoryList)
            {
                //_context?.RequestHistorys.Remove(his);
                _repositoryRequestHistory.Remove(his);
                his.AddDomainEvent(new DeletedEvent<Entities.RequestHistory>(his));
            }
            if (await _repository.SaveChangesAsync())
                return true;
        }
        return false;
    }

}
