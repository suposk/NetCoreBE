using NetCoreBE.Application.FunctionalTests.Tickets;

namespace Bookify.Api.FunctionalTests.Users;

public class TicketTests : BaseFunctionalTest
{
    private static readonly string _url = "api/v2/Ticket/";

    public TicketTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Theory]
    [InlineData(" ")]
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
    public async Task Post_ShouldReturn_Ok()
    {
        // Arrange
        var request = TicketData.Add;

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(_url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    //[Fact]
    //public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    //{
    //    // Arrange
    //    var request = new RegisterUserRequest("create@test.com", "first", "last", "12345");

    //    // Act
    //    HttpResponseMessage response = await HttpClient.PostAsJsonAsync(_url, request);

    //    // Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //}
}
