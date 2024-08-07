#if DEBUG
using CommonCleanArch.Domain;

namespace NetCoreBE.Api.Controllers;

/// <summary>
/// Only for testing, in DEBUG mode
/// </summary>
[ApiController]
[ApiVersion(AppApiVersions.V2)]
[Route("api/v{version:apiVersion}/test")]
public class TestController : ControllerBase
{
    private readonly IOldTicketRepository _ticketRepository;
    private readonly IOldTicketRepository _ticketRepository2;
    string _id = "10000000-0000-0000-0000-000000000000";
    //string _id = "bfc90000-9ba5-98fa-9843-08dbcf4521f0"; //lite

    public TestController(IOldTicketRepository ticketRepository, IOldTicketRepository ticketRepository2)
    {
        _ticketRepository = ticketRepository;
        _ticketRepository2 = ticketRepository2;
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
    [HttpGet("Search/{text}")]
    public async Task<PagedList<OldTicket>> Search(string commnad)
    {
        try
        {
            TicketSearchParameters p1 = new();
            p1.Description = commnad;
            var res = await _ticketRepository.Search(p1);

            var previousPageLink = res.HasPrevious;
            var nextPageLink = res.HasNext;
            var totalCount = res.TotalCount;
            var pageSize = res.PageSize;
            var currentPage = res.CurrentPage;
            var totalPages = res.TotalPages;
            return res;
        }
        catch (Exception ex)
        {

        }
        return null;
    }

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


    //[HttpGet]
    //public async Task<IEnumerable<string>> Get()
    //{
    //    try
    //    {
    //        var item = await _ticketRepository.GetId(_id);
    //        var copy = CopyObjectHelper.CreateDeepCopyXml(item);
    //        //OldTicket copy = new OldTicket {  CreatedAt = item.CreatedAt , Description = item.Description, Id = item.Id, 
    //        //    IsOnBehalf = item.IsOnBehalf, ModifiedAt = item.ModifiedAt, RequestedFor = item.RequestedFor, Version = item.Version };

    //        item.Description = $"{item.Description} -> Mod old ";
    //        var up = await _ticketRepository.UpdateAsync(item);
    //        var itemUpdated = await _ticketRepository.GetId(_id);
    //        var ctx = _ticketRepository.DatabaseContext;
    //        ctx.ChangeTracker.Clear();

    //        copy.Description = $"{item.Description} -> copy ";
    //        copy = await _ticketRepository2.UpdateAsync(copy);
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return new string[] { "value1", "value2" };
    //}

}
#endif