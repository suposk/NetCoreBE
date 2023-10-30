using Carter;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreBE.Api.Application.Features.Requests;

/// <summary>
/// Minimal API using Carter
/// </summary>
public class RequestModule : CarterModule
{
    public RequestModule()
        : base("api/request")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IRequestLogic _logic) =>
        {
            return Results.Ok(await _logic.GetListLogic().ConfigureAwait(false));
        });

        app.MapGet("/{id}", async (string id, IRequestLogic _logic) =>
        {
            return Results.Ok(await _logic.GetIdLogic(id).ConfigureAwait(false));
        });

        app.MapPost("/", async ([FromBody] RequestDto dto, IRequestLogic _logic) =>
        {
            var res = await _logic.AddAsyncLogic(dto).ConfigureAwait(false);
            return res != null ? Results.Ok(res) : Results.Problem($"Post {dto} Failed.");
        });

        app.MapPut("/", async (RequestDto dto, IRequestLogic _logic) =>
        {
            var res = await _logic.UpdateAsyncLogic(dto).ConfigureAwait(false);
            return res != null ? Results.Ok(res) : Results.Problem($"Put {dto} Failed.");
        });

        app.MapDelete("/{id}", async (string id, IRequestLogic _logic) =>
        {
            var res = await _logic.RemoveAsyncLogic(id).ConfigureAwait(false);
            return res ? Results.NoContent() : Results.Problem($"Delete {id} Failed.", ToString(), StatusCodes.Status500InternalServerError);
        });
    }
}