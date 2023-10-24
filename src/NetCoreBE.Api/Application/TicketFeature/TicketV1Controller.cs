using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Application.TicketFeature;

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
