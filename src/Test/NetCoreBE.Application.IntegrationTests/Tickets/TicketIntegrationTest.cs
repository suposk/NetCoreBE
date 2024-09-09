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
        await Repository.Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }
}
