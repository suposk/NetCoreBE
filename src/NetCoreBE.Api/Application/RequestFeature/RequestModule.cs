﻿using Carter;

namespace NetCoreBE.Api.Application.RequestFeature;

public class RequestModule : CarterModule
{
    //public RequestModule()
    //    : base("api/request")
    //{
    //}

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/request", async (IRequestLogic _logic) =>
        {
            return Results.Ok(await _logic.GetListLogic().ConfigureAwait(false));
        });

        app.MapGet("api/request/{id}", async (string id, IRequestLogic _logic) =>
        {
            return Results.Ok(await _logic.GetIdLogic(id).ConfigureAwait(false));
        });
    }
}