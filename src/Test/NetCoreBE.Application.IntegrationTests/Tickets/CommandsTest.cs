using NetCoreBE.Domain.UnitTests.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class CommandsTest : TicketIntegrationTest, IAsyncLifetime
{
    private readonly ITicketRepositoryDecorator _decorator;

    public CommandsTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
        //Seed(4).Wait();
    }
    public async Task InitializeAsync()
    {
        await Seed(4);
        DbContext.ChangeTracker.Clear();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdateTicketCommand_ShouldReturn_Ok()
    {

        //// Arrange NOT RELIABLE. ROWVERSION IS CHANGING
        //var dto = TicketData.Update;
        //dto.Note = $"Update {nameof(UpdateTicketCommand_ShouldReturn_Ok)}";

        //Arrange
        var old = (await _decorator.AddAsyncDto(TicketData.Add)).Value;
        DbContext.ChangeTracker.Clear();
        var dto = new TicketUpdateDto { Id = old.Id, Note = "Update t1", RowVersion = old.RowVersion };

        var command = new UpdateTicketCommand() { Dto = dto };

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();        
    }

    [Fact]
    public async Task UpdateTicketStatusCommand_ShouldReturn_Cancelled()
    {
        // Arrange
        var command = new UpdateTicketStatusCommand() { StatusEnum = StatusEnum.Cancelled, TicketId = TicketData.TicketId };

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value?.StatusEnum.Should().Be(StatusEnum.Cancelled);
        result.ErrorMessage.Should().BeNullOrEmpty();
    }
}


