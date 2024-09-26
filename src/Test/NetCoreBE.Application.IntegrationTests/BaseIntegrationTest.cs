namespace NetCoreBE.Application.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IServiceScope Scope;
    protected readonly ISender Sender;
    protected readonly ApiDbContext DbContext;
    protected readonly IntegrationTestWebAppFactory Factory;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();

        Sender = Scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        Factory = factory;

        //DbContext.Database.EnsureDeleted();
        //DbContext.Database.EnsureCreated();
    }    
}
