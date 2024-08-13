using CommonCleanArch.Application;
using SharedContract.Dtos;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class GetByIdTest : BaseIntegrationTest
{
    private static readonly string TicketId = "Ticket-1";

    public GetByIdTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetByIdQuery_ShouldReturn_Ok()
    {
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = TicketId };

        // Act
        var result = await Sender.Send(q);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }
}
