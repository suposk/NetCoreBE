using Microsoft.AspNetCore.Mvc;
using NetCoreBE.Api.Infrastructure.Persistence;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreBE.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketRepository _ticketRepository2;
    string _id = "10000000-0000-0000-0000-000000000000";
    //string _id = "bfc90000-9ba5-98fa-9843-08dbcf4521f0"; //lite

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
            //Ticket copy = new Ticket {  CreatedAt = item.CreatedAt , Description = item.Description, Id = item.Id, 
            //    IsOnBehalf = item.IsOnBehalf, ModifiedAt = item.ModifiedAt, RequestedFor = item.RequestedFor, Version = item.Version };

            item.Description = $"{item.Description} -> Mod old ";
            var up = await _ticketRepository.UpdateAsync(item);
            var itemUpdated = await _ticketRepository.GetId(_id);
            var ctx = _ticketRepository.DatabaseContext;
            ctx.ChangeTracker.Clear();

            copy.Description = $"{item.Description} -> copy ";
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

}
