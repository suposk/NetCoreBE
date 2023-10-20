namespace NetCoreBE.Api.Application.Features.Ticket;

public class ValidatorTicketDto : AbstractValidator<TicketDto>
{
    public ValidatorTicketDto()
    {
        RuleFor(p => p.Id).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(p => p.Description).NotEmpty().MinimumLength(5);
        RuleFor(p => p.RequestedFor).NotEmpty().MinimumLength(5);
    }
}

