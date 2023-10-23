namespace NetCoreBE.Api.Application.TicketFeature;

public interface ITicketLogic : IDomainLogicBase<Ticket, TicketDto>
{
}

public class TicketLogic : DomainLogicBase<Ticket, TicketDto>, ITicketLogic
{
    private readonly IRepository<Ticket> _repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TicketLogic(
        DbContext context,
        IApiIdentity apiIdentity,
        IDateTimeService dateTimeService,
        IMapper mapper,
        IRepository<Ticket> repository,
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
    public async override Task<List<TicketDto>> GetListLogic()
    {
        _mediator?.Publish(new GetTicketsEvent(new GetTickets(ApiIdentity.GetUserNameOrIp(), DateTimeService.UtcNow))); //Example track request for example audit purpose
        //var notDel = _repository.GetListActive(); //error
        return await base.GetListLogic();
    }

}
