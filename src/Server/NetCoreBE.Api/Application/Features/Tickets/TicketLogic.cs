namespace NetCoreBE.Api.Application.Features.Tickets;

public interface ITicketLogic : IDomainLogicBase<Ticket, TicketDto>
{
}

public class TicketLogic : DomainLogicBase<Ticket, TicketDto>, ITicketLogic
{
    //private readonly IRepository<Ticket> _repository;
    private readonly ITicketRepository _repository;
    private readonly IMediator _mediator;

    public TicketLogic(
        //DbContext context, //can't create instance. use repository.DatabaseContext
        IApiIdentity apiIdentity,
        IDateTimeService dateTimeService,
        IMapper mapper,
        //IRepository<Ticket> repository, //ok use generic IRepository if no custom logic needed
        ITicketRepository repository,
        IMediator mediator
        )
        : base(repository.DatabaseContext, apiIdentity, dateTimeService, mapper)
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

    //public override Task<Ticket> AddAsyncLogicEntity(Ticket entity, bool saveChanges = true)
    //{
    //    entity.SetNewTicket();
    //    return base.AddAsyncLogicEntity(entity, saveChanges);
    //}

}
