using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Application.Features.Ticket;

public interface ITicketLogic : IDomainLogicBase<Entities.Ticket, TicketDto>
{
}

public class TicketLogic : DomainLogicBase<Entities.Ticket, TicketDto>, ITicketLogic
{
    private readonly IRepository<Entities.Ticket> _repository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TicketLogic(
        DbContext context,
        IApiIdentity apiIdentity, 
        IDateTimeService dateTimeService, 
        IMapper mapper, 
        IRepository<Entities.Ticket> repository,
        IMediator mediator = null
        )
        : base(context, apiIdentity, dateTimeService, mapper)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public override Task<List<TicketDto>> GetListLogic()
    {
        _mediator?.Publish(new GetTicketsEvent(new GetTickets(ApiIdentity.GetUserNameOrIp(), DateTimeService.UtcNow)));
        return base.GetListLogic();
    }

}
