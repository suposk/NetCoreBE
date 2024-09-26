namespace NetCoreBE.Application.CrudExamples;

public class ValidatorCrudExampleDto : AbstractValidator<CrudExampleDto>
{
    public ValidatorCrudExampleDto()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Name).NotEmpty().MinimumLength(5);
    }
}

