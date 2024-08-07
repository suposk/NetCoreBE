using MediatR;

namespace NetCoreBE.Api.EndPoints;

/// <summary>
/// Minimal API using Carter
/// </summary>
public class TicketModuleV2 : CarterModule
{
    public TicketModuleV2() : base(nameof(Ticket))
    {
        WithTags(nameof(Ticket))
            .RequireRateLimiting("FixedWindowLimiter");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapGet("/", async (ITicketRepositoryDecorator decorator) =>
        //{
        //    var res = await decorator.GetListDto().ConfigureAwait(false);
        //    return res.GetIResultExt();
        //}).MapToApiVersion(AppApiVersions.V2);

        ////v2
        //app.MapGet("/{id}", async (string id, ITicketRepositoryDecorator decorator) =>
        //{
        //    var res = await decorator.GetIdDto(id).ConfigureAwait(false);
        //    return res.GetIResultExt();
        //}).MapToApiVersion(AppApiVersions.V2);

        app.MapGet("/", async (IMediator mediator) =>
        {
            var q = new GetListQuery<TicketDto>();
            var res = await mediator.Send(q).ConfigureAwait(false);
            return res.GetIResultExt();
        }).MapToApiVersion(AppApiVersions.V2);

        app.MapGet("/{id}", async (string id, IMediator mediator) =>
        {
            var q = new GetByIdQuery<TicketDto> { Id = id };
            var res = await mediator.Send(q).ConfigureAwait(false);
            return res.GetIResultExt();
        }).MapToApiVersion(AppApiVersions.V2);

        app.MapGet("/Search", async ([FromBody] TicketSearchParameters searchParameters, IMediator mediator) =>
        {
            var query = new SearchTicketQuery { SearchParameters = searchParameters };
            var res = await mediator.Send(query).ConfigureAwait(false);           
            return res.GetIResultExt();
        }).MapToApiVersion(AppApiVersions.V2);

        //SearchQuery in TicketV2Controller

        app.MapPost("/", async ([FromBody] TicketDto dto, ITicketRepositoryDecorator decorator) =>
        {
            var res = await decorator.AddAsyncDto(dto).ConfigureAwait(false);
            return res.GetIResultExt();
        }).MapToApiVersion(AppApiVersions.V2)
        .AddEndpointFilter<ValidationFilterEndpoint<TicketDto>>();

        app.MapPut("/", async (TicketDto dto, ITicketRepositoryDecorator decorator) =>
        {
            var res = await decorator.UpdateDtoAsync(dto).ConfigureAwait(false);
            return res.GetIResultExt();
        }).MapToApiVersion(AppApiVersions.V2)
        .AddEndpointFilter<ValidationFilterEndpoint<TicketDto>>();

        app.MapDelete("/{id}", async (string id, ITicketRepositoryDecorator decorator) =>
        {
            var res = await decorator.RemoveAsync(id).ConfigureAwait(false);            
            return res.GetIResultNoContentExt();
        }).MapToApiVersion(AppApiVersions.V2);

#if DEBUG
        app.MapPost("/Seed/{countToCreate}", async (int countToCreate, ITicketRepository repository, ITicketRepositoryDecorator decorator, ICacheProvider cacheProvider) =>
        {
            var PrimaryCacheKey = $"{typeof(Ticket).Name}{CommonCleanArch.Application.Helpers.OtherHelper.CACHECONST}";
            var res = await repository.Seed(countToCreate, null, "SEED API").ConfigureAwait(false);            
            cacheProvider.ClearCacheForAllKeysAndIds(PrimaryCacheKey); //clear cache for all Ids
            return res;
        }).MapToApiVersion(AppApiVersions.V2);
#endif
    }
}