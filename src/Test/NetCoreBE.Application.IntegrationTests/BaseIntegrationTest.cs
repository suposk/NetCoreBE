using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetCoreBE.Infrastructure.Persistence;

namespace NetCoreBE.Application.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IServiceScope Scope;
    protected readonly ISender Sender;
    protected readonly ApiDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();

        Sender = Scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }
}
