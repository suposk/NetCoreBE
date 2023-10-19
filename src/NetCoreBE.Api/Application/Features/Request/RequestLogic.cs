using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Application.Features.Request;

public interface IRequestLogic : IDomainLogicBase<Entities.Request, RequestDto>
{
}

public class RequestLogic : DomainLogicBase<Entities.Request, RequestDto>, IRequestLogic
{
    private readonly IRequestRepository _repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RequestLogic(
        DbContext context,
        IApiIdentity apiIdentity, 
        IDateTimeService dateTimeService, 
        IMapper mapper,
        IRequestRepository repository,
        IMediator mediator
        )
        : base(context, apiIdentity, dateTimeService, mapper)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async override Task<RequestDto> GetIdLogic(string id)
    {
        //return await base.GetIdLogic(id);
        var res = await _repository.GetId(id).ConfigureAwait(false);
        return res == null ? throw new NotFoundException($"{nameof(GetIdLogic)} id", id) : Mapper.Map<RequestDto>(res);
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

}
