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
        var sc =  Factory.Services.CreateScope();
        var ctx = sc.ServiceProvider.GetRequiredService<ApiDbContext>();
        
        //await ctx.Database.ExecuteSqlRawAsync($"DELETE FROM CrudExample");
        //await ctx.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(ctx.CrudExamples)}");

        ctx.CrudExamples.RemoveRange(ctx.CrudExamples);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        await Repository.Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }
}
