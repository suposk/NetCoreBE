using Microsoft.Extensions.DependencyInjection;
using NetCoreBE.Application.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ITicketRepository TicketRepository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        TicketRepository = Scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var seed = TicketRepository.Seed(4, 4, "SEED Startup").Result;
    }
}
