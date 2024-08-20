namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ITicketRepository TicketRepository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        TicketRepository = Scope.ServiceProvider.GetRequiredService<ITicketRepository>();        
    }

    public Task Seed(int count, string seed) => TicketRepository.Seed(count, count, seed);            
}
