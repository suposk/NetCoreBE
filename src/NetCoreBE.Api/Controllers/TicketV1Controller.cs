using Microsoft.AspNetCore.Mvc;
using NetCoreBE.Api.Application.Features.Ticket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;
[Route("api/[controller]")]
//[Route("api/v1/[controller]")]
[ApiController]
public class TicketV1Controller : ControllerBase
{
    private readonly ITicketRepository _repository;
    private readonly IMapper _mapper;
    string _id = "10000000-0000-0000-0000-000000000000";    

    public TicketV1Controller(ITicketRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<Ticket>>> Get()
    {
        var res = await _repository.GetList().ConfigureAwait(false);
        return res.HasAnyInCollection() ? Ok(res) : Ok(_mapper.Map<List<Ticket>>(res));
    }
        
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> Get(string id)
    {
        var res = await _repository.GetId(id).ConfigureAwait(false);
        return Ok(res);
    }
        
    [HttpPost]
    public async Task<ActionResult<TicketDto>> Post([FromBody] TicketDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty)
            return BadRequest();

        var repoObj = _mapper.Map<Ticket>(dto);
        var res = await _repository.AddAsync(repoObj, UserId: repoObj?.CreatedBy).ConfigureAwait(false);
        if (res == null)
            return StatusCode(StatusCodes.Status500InternalServerError, $"{nameof(Post)} Failed.");
        var mapped = _mapper.Map<TicketDto>(res);
        return CreatedAtAction(nameof(Get), new { id = res.Id }, mapped);
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
        var res = await _repository.Seed(count, null, "SEED API").ConfigureAwait(false);
        return res;
    }
#endif

    [HttpPut()]
    public async Task<ActionResult<TicketDto>> Put(TicketDto dto)
    {
        if (dto == null || dto.Id == Guid.Empty)
            return BadRequest();                

        var repoObj = await _repository.GetId(dto.Id.ToString()).ConfigureAwait(false);
        if (repoObj == null)
            return BadRequest($"{nameof(Put)} {dto.Id} not Found");

        repoObj = _mapper.Map<Ticket>(dto);
        var res = await _repository.UpdateAsync(repoObj);
        if (await _repository.SaveChangesAsync())
            return Ok(res);
            //return _mapper.Map<VmTicketDto>(repoObj);
        else
            return Conflict("Conflict detected, refresh and try again.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var res = await _repository.RemoveAsync(id);
        return res ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError, $"{Delete} {id} Failed.");
    }

}