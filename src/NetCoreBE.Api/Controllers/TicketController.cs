using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NetCoreBE.Api.Infrastructure.Persistence;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    string _id = "10000000-0000-0000-0000-000000000000";    

    public TicketController(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;        
    }

    [HttpGet]
    public async Task<ActionResult<List<Ticket>>> Get()
    {
        var res = await _ticketRepository.GetList();        ;
        return Ok(res);
    }
        
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> Get(string id)
    {
        var res = await _ticketRepository.GetId(id);
        return Ok(res);
    }
        
    [HttpPost]
    public async Task<ActionResult<Ticket>> Post([FromBody] Ticket value)
    {
        var res = await _ticketRepository.AddAsync(value, UserId: value?.CreatedBy);
        return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
        //return Ok(res);
    }
    /*
    {
      "id": "00000000-1000-0000-0000-000000000000",     
      "createdBy": "Api",
      "description": "From Api Call",
      "requestedFor": "Some User"     
    } 
    */

#if DEBUG
    [HttpPost("Seed/{count}")]
    public async Task<ActionResult<List<Ticket>>> Seed(int count)
    {
        var res = await _ticketRepository.Seed(count, null, "SEED API");
        return res;
    }
#endif

    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var res = await _ticketRepository.RemoveAsync(id);
        return res ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError, $"{Delete} {id} Failed.");
    }
}

//[HttpGet("GetFileBytes/{id}")]
//public async Task<ActionResult<byte[]>> GetFileBytes(int id)
//{
//    _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetFileBytes)} Started");
//    var result = await _logic.GetFileBytes(id).ConfigureAwait(false);
//    return Ok(result);
//}

//[HttpGet, Route(nameof(GetFiles))]
//public ActionResult<List<string>> GetFiles()
//{
//    _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetFiles)} Started");
//    var result = _logic.GetFiles();
//    return Ok(result);
//}