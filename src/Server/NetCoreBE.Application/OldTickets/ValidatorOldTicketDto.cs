namespace NetCoreBE.Application.OldTickets;

public class ValidatorOldTicketDto : AbstractValidator<OldTicketDto>
{
    public ValidatorOldTicketDto()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Description).NotEmpty().MinimumLength(5);
        RuleFor(p => p.RequestedFor).NotEmpty().MinimumLength(5);
    }
}

