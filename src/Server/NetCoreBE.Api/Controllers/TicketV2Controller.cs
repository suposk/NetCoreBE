// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using CommonCleanArch.Application.EventBus;
using NetCoreBE.Application.Tickets.IntegrationEvents;

namespace NetCoreBE.Api.Controllers;

/// <summary>
/// Using RepositoryDecoratorBase<TEntity, TDto> to wrap the Repository and IMapper into a single class.
/// Best case scenario for simple CRUD API. Automatically Gets adds and maps.
/// For more comeplex scenarios, use commands and queries.
/// </summary>
[ApiController]
[ApiVersion(AppApiVersions.V2)]
[Route("api/v{version:apiVersion}/Ticket")]
[EnableRateLimiting("FixedWindowLimiter")]
public class TicketV2Controller : ControllerBase
{
    private readonly ILogger<TicketV1Controller> _logger;

    public TicketV2Controller(ILogger<TicketV1Controller> logger)
    {
        _logger = logger;
    }

#if DEBUG
    /// <summary>
    /// Only for testing. Simulate internal bus event generated example from another micorservice
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventBus"></param>
    /// <returns></returns>
    [HttpGet("BusEventCancelTicket/{id}")]
    public async Task<IActionResult> BusEventCancelTicket(string id, [FromServices] IEventBus eventBus)
    {
        try
        {
            await eventBus.PublishAsync(new TicketCanceledIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, id));
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }        
    }
#endif

    //Only one endpoint for all the search queries
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
