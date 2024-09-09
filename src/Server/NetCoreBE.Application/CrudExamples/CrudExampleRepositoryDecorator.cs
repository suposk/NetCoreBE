namespace NetCoreBE.Application.CrudExamples;

public interface ICrudExampleRepositoryDecorator : IRepositoryDecoratorBase<CrudExample, CrudExampleDto>
{
}

public class CrudExampleRepositoryDecorator : RepositoryDecoratorBase<CrudExample, CrudExampleDto>, ICrudExampleRepositoryDecorator
{
    private readonly IDateTimeService _dateTimeService;

    public CrudExampleRepositoryDecorator(
        IDateTimeService dateTimeService,
        ICrudExampleRepository repository,
        IApiIdentity apiIdentity,
        IMapper mapper,
        ILoggerFactory logger,
        ICacheProvider cacheProvider)
        : base(repository, apiIdentity, mapper, logger, cacheProvider)
    {
        _dateTimeService = dateTimeService;
    }

    public override int CacheDurationMinutes => 5;

    string? _Name;
    public override string Name => _Name ??= ToString()!;        

    protected override Task<bool> CanModifyFunc(CrudExample entity) => base.CanModifyFunc(entity);

    ///// only if you want to override the base methods /////

    //public override Task<ResultCom<CrudExampleDto>> AddAsyncDto(CrudExampleDto dto, bool saveChanges = true)
    //{
    //    return base.AddAsyncDto(dto, saveChanges);
    //}

    //public override Task<ResultCom<CrudExampleDto>> UpdateDtoAsync(CrudExampleDto dto, bool saveChanges = true)
    //{
    //    return base.UpdateDtoAsync(dto, saveChanges);
    //}

    //public override Task<ResultCom<CrudExampleDto>> AddOrUpdateDtoAsync(CrudExampleDto dto, bool saveChanges = true)
    //{
    //    return base.AddOrUpdateDtoAsync(dto, saveChanges);
    //}

    //public override Task<ResultCom> RemoveAsync(string id, bool saveChanges = true)
    //{
    //    return base.RemoveAsync(id, saveChanges);
    //}

}
