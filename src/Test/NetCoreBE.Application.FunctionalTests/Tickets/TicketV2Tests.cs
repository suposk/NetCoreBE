using NetCoreBE.Application.FunctionalTests.Tickets;

namespace Bookify.Api.FunctionalTests.Users;

public class TicketV2Tests : BaseFunctionalTest, IDisposable
{
    private static readonly string _url = "api/v2/Ticket/";

    public TicketV2Tests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
        Seed(4).Wait();
    }

    private Task Seed(int count) => Scope.ServiceProvider.GetRequiredService<ITicketRepository>().Seed(count, count, "Seed Test");

    [Theory]
    //[InlineData(" ")]//return Ok
    [InlineData("1")]
    [InlineData("Ticket-Fake")]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid(
        string id)
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(_url  + id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
    //put should return no content
    public async Task Put_ShouldReturn_NoContent()
    {
        // Arrange
        var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };
        var request = (await Sender.Send(q)).Value;
        DbContext.ChangeTracker.Clear();
        request.Note = "Update test";

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync(_url, request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        content.Should().BeNullOrEmpty();
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
