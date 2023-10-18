using CommonBE.Infrastructure.Persistence;

namespace NetCoreBE.Api.Application.Features.Ticket;

public interface ITicketLogic : IDomainLogicBase<Entities.Ticket, TicketDto>
{
}

public class TicketLogic : DomainLogicBase<Entities.Ticket, TicketDto>, ITicketLogic
{
    private readonly IRepository<Entities.Ticket> _repository;
    private readonly IMapper _mapper;

    public TicketLogic(
        DbContext context,
        IApiIdentity apiIdentity, 
        IDateTimeService dateTimeService, 
        IMapper mapper, 
        IRepository<Entities.Ticket> repository
        )
        : base(context, apiIdentity, dateTimeService, mapper)
    {
        _repository = repository;
    }

}
