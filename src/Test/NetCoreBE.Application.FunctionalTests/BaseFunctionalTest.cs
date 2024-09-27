namespace NetCoreBE.Application.FunctionalTests;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly IServiceScope Scope;
    protected readonly ISender Sender;
    protected readonly ApiDbContext DbContext;

    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();
        Sender = Scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        //DbContext.Database.EnsureDeleted();
        //DbContext.Database.EnsureCreated();

        HttpClient = factory.CreateClient();
    }

    /// <summary>
    /// to do: implement
    /// </summary>
    /// <returns></returns>
    protected async Task<string?> GetAccessToken()
    {
        await Task.Yield();
        return null;
    }

    protected async Task<bool> AddTokenToHeaderAsync()
    {
        var accessToken = await GetAccessToken();
        if (string.IsNullOrEmpty(accessToken))        
            return false;
            //throw new Exception("Access token is null or empty");

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return true;
    }
    
}
