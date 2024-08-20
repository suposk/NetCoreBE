using Microsoft.Extensions.DependencyInjection;
using NetCoreBE.Application.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ITicketRepository TicketRepository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        TicketRepository = Scope.ServiceProvider.GetRequiredService<ITicketRepository>();        
    }

    public Task Seed(int count, string seed)
    {
        var seedData = TicketRepository.Seed(count, count, seed);
        return Task.CompletedTask;
    }
}
