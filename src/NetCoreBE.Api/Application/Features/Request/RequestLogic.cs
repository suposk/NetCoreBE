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

    /// <summary>
    /// Can ovveride the base method to add custom logic. Example track request for example audit purpose.
    /// </summary>
    /// <returns></returns>
    public async override Task<List<RequestDto>> GetListLogic()
    {
        //return await base.GetListLogic();
        var res = await _repository.GetList().ConfigureAwait(false);
        return res == null ? default : Mapper.Map<List<RequestDto>>(res);
    }

    public async override Task<RequestDto> GetIdLogic(string id)
    {
        //return base.GetIdLogic(id);
        var res = await _repository.GetId(id).ConfigureAwait(false);
        return res == null ? default : Mapper.Map<RequestDto>(res);
    }

}
