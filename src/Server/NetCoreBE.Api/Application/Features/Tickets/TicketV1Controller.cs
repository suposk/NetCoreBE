using CommonCleanArch.Infrastructure.Enums;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Application.Features.Tickets;

/// <summary>
/// Using DomainLogicBase<TEntity, TDto> to wrap the Repository and IMapper into a single class.
/// Best case scenario for simple CRUD API. Automatically Gets adds and maps.
/// For more comeplex scenarios, use commands and queries.
/// </summary>
[Route("api/Ticket")]
//[Route("api/v1/Ticket")]
[ApiController]
public class TicketV1Controller : ControllerBase
{
    private readonly ITicketLogic _logic;
    private readonly ILogger<TicketV1Controller> _logger;

    public TicketV1Controller(ITicketLogic logic, ILogger<TicketV1Controller> logger)
    {
        _logic = logic;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> Get()
    {
        Log.Information("GetListLogic");
        return Ok(await _logic.GetListLogic().ConfigureAwait(false));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> Get(string id)
    {
        Log.Information("GetIdLogic {id}", id);
        return Ok(await _logic.GetIdLogic(id).ConfigureAwait(false));
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Post([FromBody] TicketDto dto)
    {
        Log.Information("Post Starting {@dto}", dto);
        var res = await _logic.AddAsyncLogic(dto).ConfigureAwait(false);
        return res != null ? Ok(res) : StatusCode(StatusCodes.Status500InternalServerError, $"{Post} {dto} Failed.");
    }

    [HttpPut()]
    public async Task<ActionResult<TicketDto>> Put(TicketDto dto)
    {
        Log.Information("Put Starting {@dto}", dto);
        var res = await _logic.UpdateAsyncLogic(dto).ConfigureAwait(false);
        return res != null ? Ok(res) : StatusCode(StatusCodes.Status500InternalServerError, $"{Put} {dto} Failed.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        Log.Information("Delete Starting {id}", id);
        var res = await _logic.RemoveAsyncLogic(id).ConfigureAwait(false);
        return res ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError, $"{Delete} {id} Failed.");
    }

    [HttpGet("Search")]    
    public async Task<ActionResult<PagedListDto<TicketDto>>> Search(TicketSearchParameters searchParameters, [FromServices] IMediator mediator)
    {
        Log.Information("Search Starting {@searchParameters}", searchParameters);
        var query = new SearchQuery { SearchParameters = searchParameters };
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
    public async Task<ActionResult<PagedListDto<TicketDto>>> SearchQuery(
        [FromQuery] TicketSearchParameters searchParameters,        
        [FromServices] IMediator mediator)
    {        
        _logger.LogInformation("Starting {@searchParameters}", searchParameters);                
        //Log.Information("SearchQuery Starting {@searchParameters}", searchParameters);
        var query = new SearchQuery { SearchParameters = searchParameters };
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
    public async Task<ActionResult<List<Ticket>>> Seed(int countToCreate, [FromServices] ITicketRepository repository)
    {
        var res = await repository.Seed(countToCreate, null, "SEED API").ConfigureAwait(false);
        return res;
    }
#endif

}

#region Post Request Body
/*
{
  "id": "00000000-1000-0000-0000-000000000000",     
  "createdBy": "Api",
  "description": "From Api Call",
  "requestedFor": "Some User"     
} 
*/
#endregion
