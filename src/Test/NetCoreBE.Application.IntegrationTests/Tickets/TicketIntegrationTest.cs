namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ITicketRepository Repository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        Repository = Scope.ServiceProvider.GetRequiredService<ITicketRepository>();        
    }

    public async  Task Seed(int count)
    {
        var ctx = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        ctx.TicketHistorys.RemoveRange(ctx.TicketHistorys);
        ctx.Tickets.RemoveRange(ctx.Tickets);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        await Repository.Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }
}
