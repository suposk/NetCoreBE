namespace NetCoreBE.Api.Application.RequestFeature;

public class ValidatorRequestDto : AbstractValidator<RequestDto>
{
    public ValidatorRequestDto()
    {
        RuleFor(p => p.Id).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(p => p.RequestType).NotEmpty().MinimumLength(5); //TODO Checf string types
    }
}

