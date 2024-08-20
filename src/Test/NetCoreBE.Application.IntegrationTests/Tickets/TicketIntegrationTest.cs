using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreBE.Application.Tickets;
using NetCoreBE.Infrastructure.Persistence;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public abstract class TicketIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ApiDbContext DbContext;
    protected readonly ITicketRepository TicketRepository;

    protected TicketIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
        TicketRepository = _scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var seed = TicketRepository.Seed(4, 4, "SEED Startup").Result;
    }
}
