using NetCoreBE.Domain.UnitTests.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class CommandsTest : TicketIntegrationTest, IAsyncLifetime
{
    public CommandsTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        //Seed(4).Wait();
    }
    public Task InitializeAsync() => Seed(4);
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdateTicketCommand_ShouldReturn_Ok()
    {

        // Arrange
        var dto = TicketData.Update;
        dto.Note = $"Update {nameof(UpdateTicketCommand_ShouldReturn_Ok)}";
        // Arrange
        var command = new UpdateTicketCommand() { Dto = dto };

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();        
    }
}


