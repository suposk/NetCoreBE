using CSRO.Server.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketRepository _ticketRepository2;
    string _id = "bfc90000-9ba5-98fa-06a0-08dbcf3daf45";

    public TestController(ITicketRepository ticketRepository, ITicketRepository ticketRepository2)
    {
        _ticketRepository = ticketRepository;
        _ticketRepository2 = ticketRepository2;
    }

    // GET: api/<TestController>
    [HttpGet]
    public async Task<IEnumerable<string>> Get()
    {
        try
        {
            var item = await _ticketRepository.GetId(_id);
            var copy = CopyObjectHelper.CreateDeepCopyXml(item);
            item.Description = $"{item.Description} -> Mod old {item.Version}";
            var up = await _ticketRepository.UpdateAsync(item);
            var itemUpdated = await _ticketRepository.GetId(_id);
            var ctx = _ticketRepository.DatabaseContext;
            ctx.ChangeTracker.Clear();

            copy.Description = $"{item.Description} -> copy {item.Version}";
            copy = await _ticketRepository2.UpdateAsync(copy);
        }
        catch (Exception ex)
        {

        }
        return new string[] { "value1", "value2" };
    }

    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public Task<Ticket> Get(string id)
    {
        return _ticketRepository.GetId(id);
    }

    // POST api/<TestController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<TestController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<TestController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
