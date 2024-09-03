namespace NetCoreBE.Application.Tickets;

public class ValidatorTicketSearchParameters : AbstractValidator<TicketSearchParameters>
{
    public ValidatorTicketSearchParameters()
    {
        RuleFor(p => p.PageSize).NotEmpty().GreaterThanOrEqualTo(1);
        RuleFor(p => p.CurrentPage).NotEmpty().GreaterThanOrEqualTo(1);
    }
}

