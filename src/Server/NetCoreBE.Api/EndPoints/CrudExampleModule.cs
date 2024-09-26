namespace NetCoreBE.Api.EndPoints;

public class CrudExampleModule : CarterModule
{
    public CrudExampleModule() : base(nameof(CrudExample))
    {
        WithTags(nameof(CrudExample))
            .RequireRateLimiting("FixedWindowLimiter");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (ICrudExampleRepositoryDecorator decorator) =>
        {
            var res = await decorator.GetListDto().ConfigureAwait(false);
            return res.GetIResultExt();
        });
                
        app.MapGet("/{id}", async (string id, ICrudExampleRepositoryDecorator decorator) =>
        {
            var res = await decorator.GetIdDto(id).ConfigureAwait(false);
            return res.GetIResultExt();
        });

        app.MapPost("/", async ([FromBody] CrudExampleDto dto, ICrudExampleRepositoryDecorator decorator) =>
        {
            var res = await decorator.AddAsyncDto(dto).ConfigureAwait(false);
            if (res.IsSuccess)
                return Results.Created($"/{res.Value?.Id}", res.Value);
            return res.GetIResultExt();
        }).AddEndpointFilter<ValidationFilterEndpoint<CrudExampleDto>>();

        app.MapPut("/", async ([FromBody] CrudExampleDto dto, ICrudExampleRepositoryDecorator decorator) =>
        {
            var res = await decorator.UpdateDtoAsync(dto).ConfigureAwait(false);
            return res.GetIResultExt();
        }).AddEndpointFilter<ValidationFilterEndpoint<CrudExampleDto>>();

        app.MapDelete("/{id}", async (string id, ICrudExampleRepositoryDecorator decorator) =>
        {
            var res = await decorator.RemoveAsync(id).ConfigureAwait(false);
            return res.GetIResultNoContentExt();
        });

#if DEBUG
        app.MapPost("/Seed/{countToCreate}", async (int countToCreate, ICrudExampleRepository repository, ICacheProvider cacheProvider) =>
        {
            var PrimaryCacheKey = $"{typeof(CrudExample).Name}{CommonCleanArch.Application.Helpers.OtherHelper.CACHECONST}";
            var res = await repository.Seed(countToCreate, null, "SEED API").ConfigureAwait(false);
            cacheProvider.ClearCacheForAllKeysAndIds(PrimaryCacheKey); //clear cache for all Ids
            return res;
        });
#endif
    }
}
