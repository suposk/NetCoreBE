using CommonBE.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
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

    public TicketV1Controller(ITicketLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> Get() => Ok(await _logic.GetListLogic().ConfigureAwait(false));

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> Get(string id) => Ok(await _logic.GetIdLogic(id).ConfigureAwait(false));

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Post([FromBody] TicketDto dto)
    {
        var res = await _logic.AddAsyncLogic(dto).ConfigureAwait(false);
        return res != null ? Ok(res) : StatusCode(StatusCodes.Status500InternalServerError, $"{Post} {dto} Failed.");
    }

    [HttpPut()]
    public async Task<ActionResult<TicketDto>> Put(TicketDto dto)
    {
        var res = await _logic.UpdateAsyncLogic(dto).ConfigureAwait(false);
        return res != null ? Ok(res) : StatusCode(StatusCodes.Status500InternalServerError, $"{Put} {dto} Failed.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var res = await _logic.RemoveAsyncLogic(id).ConfigureAwait(false);
        return res ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError, $"{Delete} {id} Failed.");
    }

    [HttpGet("Search")]    
    [HttpHead]
    public async Task<ActionResult<PagedListDto<TicketDto>>> Search([FromQuery] TicketSearchParameters searchParameters,
        [FromServices] ITicketRepository ticketRepository, [FromServices] IPropertyMappingService propertyMappingService,
        [FromServices] IMapper mapper)
    {
        if (!propertyMappingService.ValidMappingExistsFor<TicketDto, Ticket>
            (searchParameters.OrderBy))        
            return BadRequest();
        
        var res = await ticketRepository.Search(searchParameters);   
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
        var dtos = mapper.Map<List<TicketDto>>(res);
        var mapped = new PagedListDto<TicketDto>(dtos, res.TotalCount, res.CurrentPage, res.PageSize);
        var result = new PagedResultDto<TicketDto>(mapped);
        return Ok(result);
    }

    private string CreateResourceUri(TicketSearchParameters parameters, ResourceUriType type)    
    {
        switch (type)
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("Search",
                  new
                  {
                      orderBy = parameters.OrderBy,
                      pageNumber = parameters.CurrentPage - 1,
                      pageSize = parameters.PageSize,                      
                      searchQuery = parameters.SearchQuery
                  });
            case ResourceUriType.NextPage:
                return Url.Link("Search",
                  new
                  {
                      orderBy = parameters.OrderBy,
                      pageNumber = parameters.CurrentPage + 1,
                      pageSize = parameters.PageSize,                      
                      searchQuery = parameters.SearchQuery
                  });

            default:
                return Url.Link("Search",
                new
                {
                    orderBy = parameters.OrderBy,
                    pageNumber = parameters.CurrentPage,
                    pageSize = parameters.PageSize,                    
                    searchQuery = parameters.SearchQuery
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
