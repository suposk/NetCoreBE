namespace NetCoreBE.Api.EndPoints;

/// <summary>
/// Minimal API using Carter
/// </summary>
public class TicketHistoryModule : CarterModule
{
    public TicketHistoryModule() : base(nameof(TicketHistory))
    {
        WithTags(nameof(TicketHistory))
            .RequireRateLimiting("FixedWindowLimiter");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (ISender sender) =>
        {
            var res = await sender.Send(new GetListQuery<TicketHistoryDto>()).ConfigureAwait(false);
            return res.GetIResultExt();
        });

        app.MapGet("/{id}", async (string id, ISender sender) =>
        {
            var res = await sender.Send(new GetByIdQuery<TicketHistoryDto> { Id = id }).ConfigureAwait(false);
            return res.GetIResultExt();
        });

    }
}