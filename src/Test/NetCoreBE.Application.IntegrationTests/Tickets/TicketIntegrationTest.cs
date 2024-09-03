namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ITicketRepository TicketRepository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        TicketRepository = Scope.ServiceProvider.GetRequiredService<ITicketRepository>();        
    }

    public async  Task Seed(int count)
    {
        await TicketRepository.Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }
}
