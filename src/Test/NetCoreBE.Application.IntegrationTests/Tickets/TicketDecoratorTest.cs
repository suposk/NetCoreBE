namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class TicketDecoratorTest : TicketIntegrationTest
{
    private static readonly string TicketId = "Ticket-1";
    private static readonly string AddTicketId = "Ticket-01";

    private readonly ITicketRepositoryDecorator _decorator;

    public TicketDecoratorTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
    }

    [Fact]
    public async Task Add_ShouldReturn_Ok()
    {
        // Arrange        
        var dto = new TicketDto
        {
            Id = AddTicketId,
            TicketType = "New Laptop",            
            Note = "add test",
            CreatedBy = "Test",
            //CreatedAt = DateTime.Now
        };

        // Act
        var result = await _decorator.AddAsyncDto(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }


}
