// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;

/// <summary>
/// Using RepositoryDecoratorBase<TEntity, TDto> to wrap the Repository and IMapper into a single class.
/// Best case scenario for simple CRUD API. Automatically Gets adds and maps.
/// For more comeplex scenarios, use commands and queries.
/// </summary>
[ApiController]
[ApiVersion(AppApiVersions.V1)]
[Route("api/v{version:apiVersion}/OldTicket")]
[EnableRateLimiting("FixedWindowLimiter")]
public class OldTicketV1Controller : ControllerBase
{
    private readonly IOldTicketRepositoryDecorator _decorator;
    private readonly ILogger<OldTicketV1Controller> _logger;

    public OldTicketV1Controller(IOldTicketRepositoryDecorator decorator, ILogger<OldTicketV1Controller> logger)
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
    public async Task<IResult> Post([FromBody] OldTicketDto dto)
    {        
        var res = await _decorator.AddAsyncDto(dto).ConfigureAwait(false);
        return res.GetIResultExt();
    }

    [HttpPut()]
    public async Task<IResult> Put(OldTicketDto dto)
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
    public async Task<ActionResult<PagedListDto<OldTicketDto>>> Search(TicketSearchParameters searchParameters, [FromServices] IMediator mediator)
    {
        Log.Information("Search Starting {@searchParameters}", searchParameters);
        var query = new SearchOldTicketQuery { SearchParameters = searchParameters };
        var res = await mediator.Send(query).ConfigureAwait(false);
        if (res is null)
            return NotFound();
        return res;
    }
    #region request body
    /*
{
  "isActive": null,
  "searchTerm": null,
  "currentPage": 2,
  "pageSize": 15,
  "orderBy": "CreatedAt",
  "description": "62"
}
    */
    #endregion

    [HttpGet("SearchQuery")]
    [HttpHead]
    public async Task<ActionResult<PagedListDto<OldTicketDto>>> SearchQuery(
        [FromQuery] TicketSearchParameters searchParameters,
        [FromServices] IMediator mediator)
    {
        _logger.LogInformation("Starting {@searchParameters}", searchParameters);
        //Log.Information("SearchQuery Starting {@searchParameters}", searchParameters);
        var query = new SearchOldTicketQuery { SearchParameters = searchParameters };
        var res = await mediator.Send(query).ConfigureAwait(false);
        if (res is null)
            return NotFound();

        var previousPageLink = res.HasPrevious ? CreateResourceUri(searchParameters, ResourceUriType.PreviousPage) : null;
        var nextPageLink = res.HasNext ? CreateResourceUri(searchParameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = res.TotalCount,
            pageSize = res.PageSize,
            currentPage = res.CurrentPage,
            totalPages = res.TotalPages,
            previousPageLink,
            nextPageLink
        };
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return res;
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

#if DEBUG
    [HttpPost("Seed/{countToCreate}")]
    public async Task<ActionResult<List<OldTicket>>> Seed(int countToCreate, [FromServices] IOldTicketRepository repository)
    {
        var res = await repository.Seed(countToCreate, null, "SEED API").ConfigureAwait(false);
        return res;
    }
#endif

}
