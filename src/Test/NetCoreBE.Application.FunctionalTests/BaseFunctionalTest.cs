namespace NetCoreBE.Application.FunctionalTests;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
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
