namespace NetCoreBE.Application.IntegrationTests.CrudExamples;

public abstract class CrudExampleIntegrationTest : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ICrudExampleRepository Repository;

    protected CrudExampleIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        Repository = Scope.ServiceProvider.GetRequiredService<ICrudExampleRepository>();        
    }

    public async  Task Seed(int count)
    {
        await Repository.Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }
}
