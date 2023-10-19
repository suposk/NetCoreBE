using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Application.Features.Request;

public interface IRequestLogic : IDomainLogicBase<Entities.Request, RequestDto>
{
}

public class RequestLogic : DomainLogicBase<Entities.Request, RequestDto>, IRequestLogic
{
    private readonly IRepository<Entities.Request> _repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RequestLogic(
        DbContext context,
        IApiIdentity apiIdentity, 
        IDateTimeService dateTimeService, 
        IMapper mapper, 
        IRepository<Entities.Request> repository,
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
        return await base.GetListLogic();
    }

    /// <summary>
    /// Adding child StatusHistory to Request
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="UserId"></param>
    /// <returns></returns>
    public override Task<Entities.Request> AddAsync(Entities.Request entity, string UserId = null)
    {
        entity?.AddInitialHistory();
        return base.AddAsync(entity, UserId);
    }

}
