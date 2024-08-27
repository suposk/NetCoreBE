// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;

/// <summary>
/// Using RepositoryDecoratorBase<TEntity, TDto> to wrap the Repository and IMapper into a single class.
/// Best case scenario for simple CRUD API. Automatically Gets adds and maps.
/// For more comeplex scenarios, use commands and queries.
/// </summary>
[ApiController]
[ApiVersion(AppApiVersions.V1)]
[Route("api/v{version:apiVersion}/Ticket")]
[EnableRateLimiting("FixedWindowLimiter")]
public class TicketV1Controller : ControllerBase
{
    private readonly ITicketRepositoryDecorator _decorator;
    private readonly ILogger<TicketV1Controller> _logger;

    public TicketV1Controller(ITicketRepositoryDecorator decorator, ILogger<TicketV1Controller> logger)
    {
        _decorator = decorator;
        _logger = logger;
    }

    [HttpGet]
    //[ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
    public async Task<IResult> Get()
    {
        var res = await _decorator.GetListDto().ConfigureAwait(false);
        return res.GetIResultExt();
    }

    [HttpGet("{id}")]
    //[ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
    public async Task<IResult> Get(string id)
    {
        var res = await _decorator.GetIdDto(id).ConfigureAwait(false);
        return res.GetIResultExt();
        //return res.IsSuccess ? Results.Ok(res.Data) : Results.Problem(res.ErrorMessage, null, res.Error?.StatusCode);
    }

    [HttpPost]
    public async Task<IResult> Post([FromBody] TicketDto dto)
    {        
        var res = await _decorator.AddAsyncDto(dto).ConfigureAwait(false);
        if (res.IsSuccess)
            return Results.Created($"/{res.Value?.Id}", res.Value);
        return res.GetIResultExt();
    }

    [HttpPut()]
    public async Task<IResult> Put([FromBody] TicketUpdateDto dto)
    {        
        var res = await _decorator.UpdateDtoAsync(dto).ConfigureAwait(false);
        return res.GetIResultExt();
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(string id)
    {
        var res = await _decorator.RemoveAsync(id).ConfigureAwait(false);
        return res.GetIResultNoContentExt();
    }

    [HttpGet("Search")]
    public async Task<ActionResult<PagedListDto<TicketDto>>> Search(TicketSearchParameters searchParameters, [FromServices] IMediator mediator)
    {
        Log.Information("Search Starting {@searchParameters}", searchParameters);
        var query = new SearchTicketQuery { SearchParameters = searchParameters };
        var res = await mediator.Send(query).ConfigureAwait(false);
        if (res?.Value is null)
            return NotFound();
        return res.Value;
    }

    [HttpGet("SearchQuery")]
    [HttpHead]
    public async Task<ActionResult<PagedListDto<TicketDto>>> SearchQuery(
        [FromQuery] TicketSearchParameters searchParameters,
        [FromServices] IMediator mediator)
    {
        _logger.LogInformation("Starting {@searchParameters}", searchParameters);
        //Log.Information("SearchQuery Starting {@searchParameters}", searchParameters);
        var query = new SearchTicketQuery { SearchParameters = searchParameters };
        var res = await mediator.Send(query).ConfigureAwait(false);
        if (res?.Value is null)
            return NotFound();

        var resValue = res.Value;
        var previousPageLink = resValue.HasPrevious ? CreateResourceUri(searchParameters, ResourceUriType.PreviousPage) : null;
        var nextPageLink = resValue.HasNext ? CreateResourceUri(searchParameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = resValue.TotalCount,
            pageSize = resValue.PageSize,
            currentPage = resValue.CurrentPage,
            totalPages = resValue.TotalPages,
            previousPageLink,
            nextPageLink
        };
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return resValue;
    }

    private string CreateResourceUri(TicketSearchParameters parameters, ResourceUriType type)
    {
        switch (type)
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("SearchQuery",
                  new
                  {
                      orderBy = parameters.OrderBy,
                      pageNumber = parameters.CurrentPage - 1,
                      pageSize = parameters.PageSize,
                      searchQuery = parameters.SearchTerm
                  });
            case ResourceUriType.NextPage:
                return Url.Link("SearchQuery",
                  new
                  {
                      orderBy = parameters.OrderBy,
                      pageNumber = parameters.CurrentPage + 1,
                      pageSize = parameters.PageSize,
                      searchQuery = parameters.SearchTerm
                  });

            default:
                return Url.Link("SearchQuery",
                new
                {
                    orderBy = parameters.OrderBy,
                    pageNumber = parameters.CurrentPage,
                    pageSize = parameters.PageSize,
                    searchQuery = parameters.SearchTerm
                });
        }

    }

}
