using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Application.Features.Ticket;

[Route("api/Ticket")]
//[Route("api/v2/Ticket")]
//[Route("api/[controller]")]
[ApiController]
public class TicketV2Controller : ControllerBase
{
    string _id = "10000000-0000-0000-0000-000000000000";
    private readonly ITicketLogic _logic;

    public TicketV2Controller(ITicketLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> Get()
    {
        var res = await _logic.GetListLogic().ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> Get(string id)
    {
        var res = await _logic.GetIdLogic(id).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Post([FromBody] TicketDto dto)
    {
		var res = await _logic.AddAsyncLogic(dto).ConfigureAwait(false);
		return res != null ? Ok(res) : StatusCode(StatusCodes.Status500InternalServerError, $"{Post} {dto} Failed.");
    }
    /*
    {
      "id": "00000000-1000-0000-0000-000000000000",     
      "createdBy": "Api",
      "description": "From Api Call",
      "requestedFor": "Some User"     
    } 
    */

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