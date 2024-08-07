namespace NetCoreBE.Application.OldTickets;

public class ValidatorOldTicketSearchParameters : AbstractValidator<TicketSearchParameters>
{
    public ValidatorOldTicketSearchParameters()
    {
        RuleFor(p => p.PageSize).NotEmpty().GreaterThanOrEqualTo(1);
        RuleFor(p => p.CurrentPage).NotEmpty().GreaterThanOrEqualTo(1);
    }
}

