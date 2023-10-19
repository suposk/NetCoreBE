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
        return res == null ? default : Mapper.Map<RequestDto>(res);
    }

    public override Task<Entities.Request> AddAsync(Entities.Request entity, string UserId = null)
    {
        entity.AddInitialHistory();
        entity.Status = "Active";
        return base.AddAsync(entity, UserId);
    }

}
