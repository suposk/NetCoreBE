namespace NetCoreBE.Application.Tickets;

public class ValidatorTicketDto : AbstractValidator<TicketDto>
{
    public ValidatorTicketDto()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.TicketType).NotEmpty().MinimumLength(5); //TODO Checf string types
    }
}

public class ValidatorTicketUpdateDto : AbstractValidator<TicketUpdateDto>
{
    public ValidatorTicketUpdateDto()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.RowVersion).GreaterThan(uint.MinValue);
    }
}

