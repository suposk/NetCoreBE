#if DEBUG
using Azure.Core;
using CommonCleanArch.Application.EventBus;
using CommonCleanArch.Domain;
using Microsoft.EntityFrameworkCore.Storage;
using NetCoreBE.Application.Tickets.IntegrationEvents;
using System.Data;

namespace NetCoreBE.Api.Controllers;

/// <summary>
/// Only for testing, in DEBUG mode
/// </summary>
[ApiController]
[ApiVersion(AppApiVersions.V2)]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ControllerBase
{
    //private readonly IOldTicketRepository _OldTicketRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IRepository<TicketHistory> _repositoryTicketHistory;
    string _id = "10000000-0000-0000-0000-000000000000";
    //string _id = "bfc90000-9ba5-98fa-9843-08dbcf4521f0"; //lite

    public TestController
        (
        //IOldTicketRepository OldTicketRepository, 
        ITicketRepository ticketRepository, 
        IRepository<TicketHistory> repositoryTicketHistory
        )
    {
        //_OldTicketRepository = OldTicketRepository;
        _ticketRepository = ticketRepository;
        _repositoryTicketHistory = repositoryTicketHistory;
    }

    //[HttpGet("CancelTicket")]
    [HttpGet("CancelTicket/{id}")]
    public async Task<IActionResult> CancelTicketId(string id, [FromServices] IEventBus eventBus)
    {
        try
        {
            await eventBus.PublishAsync(new TicketCanceledIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, id));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    [HttpGet("TestTransaction")]
    public async Task<object> TestTransaction()
    {
        IDbContextTransaction? dbTransaction = null;
        try
        {
            dbTransaction = _ticketRepository.GetTransaction();

            string Id = "Ticket-01";
            var entity = await _ticketRepository.GetId(Id);
            if (entity is null)
                return null;

            var upd = entity.Update(null, $"Up ${DateTime.Now.ToShortTimeString()}", DateTime.UtcNow);
            if (upd.IsFailure)
                return null;

            _repositoryTicketHistory.UseTransaction(dbTransaction);
            var resHistory = await _repositoryTicketHistory.AddAsync(entity.TicketHistoryList.Last());
            var res = await _ticketRepository.UpdateAsync(entity);

            dbTransaction?.Commit();
        }
        catch (Exception ex)
        {
            dbTransaction?.Rollback();
        }
        finally
        {
            dbTransaction?.Dispose();
        }

        return null;
    }

    //[HttpGet("appsettings/{key}")]
    //public async Task<object> Appsettings(string? key, [FromServices] IConfiguration configuration)
    //{
    //    await Task.Yield();

    //    StringBuilder sb = new();
    //    try
    //    {
    //        sb.AppendLine($"key = {key}");
    //        if (key is null)
    //            ;
    //        AzureAppConfigurationConfig? AzureAppConfigurationConfig = configuration.GetSection(nameof(AzureAppConfigurationConfig)).Get<AzureAppConfigurationConfig>();
    //        sb.AppendLine($"AzureAppConfigurationConfig = {AzureAppConfigurationConfig?.UseKeyVault}");
    //        if (AzureAppConfigurationConfig?.AppConfigurationList?.Count > 0)
    //        {
    //            sb.AppendLine($"AzureAppConfigurationConfig.AppConfigurationList.Count = {AzureAppConfigurationConfig.AppConfigurationList.Count}");
    //            foreach (var item in AzureAppConfigurationConfig.AppConfigurationList)
    //            {
    //                var val = configuration[item];
    //                sb.AppendLine($"Key: {item} = {val}");
    //            }
    //        }
    //        return Ok(sb.ToString());
    //    }
    //    catch (Exception ex)
    //    {
    //        sb.Append($"ex: {ex?.Message}");
    //    }
    //    return sb.ToString();
    //}

    //[HttpGet]
    [HttpGet("Test1")]
    public async Task<object> Test1()
    {
        try
        {
            //var obj = new RequestHistory { Operation = "test" };
            var name1 = $"{typeof(TicketHistory).Name}Cache";
            var name2 = typeof(TicketHistory).GetPrimaryCacheKeyExt();
            //var name3 = obj.GetPrimaryCacheKeyExt();
            return null;
        }
        catch (Exception ex)
        {

        }
        return null;
    }

}
#endif