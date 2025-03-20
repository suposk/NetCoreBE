namespace NetCoreBE.Application.FunctionalTests.Tickets;

public class TicketV2Tests : BaseFunctionalTest, IDisposable, IAsyncLifetime
{
    private static readonly string _url = "api/v2/Ticket/";
    private readonly ITicketRepositoryDecorator _decorator;

    public TicketV2Tests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
        //Seed(4).Wait();
    }

    public Task InitializeAsync() => Seed(4);
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task Seed(int count)
    {
        var ctx = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        ctx.TicketHistorys.RemoveRange(ctx.TicketHistorys);
        ctx.Tickets.RemoveRange(ctx.Tickets);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        await Scope.ServiceProvider.GetRequiredService<ITicketRepository>().Seed(count, count, "Seed Test");
        DbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Get_ShouldReturn_Ok()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(_url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ShouldReturn_Ok()
    {
        // Arrange        

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(_url + TicketData.TicketId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Theory]
    //[InlineData(" ")]//return Ok
    [InlineData("1")]
    [InlineData("Ticket-Fake")]
    public async Task GetById_ShouldReturn_NotFound(string id)
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(_url + id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn_Ok()
    {
        // Arrange
        var request = TicketData.Add;

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(_url, request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Put_ShouldReturn_Ok()
    {
        //// Arrange
        //var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };
        //var request = (await Sender.Send(q)).Value;
        //DbContext.ChangeTracker.Clear();
        //request.Note = "Update test";

        //Arrange
        var request = (await _decorator.AddAsyncDto(TicketData.Add)).Value;
        request.Note = "Update test";
        DbContext.ChangeTracker.Clear();        

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync(_url, request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Delete_ShouldReturn_NoContent()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync(_url + TicketData.TicketId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturn_NotFound()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync(_url + "Fake");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }

}
