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
    public async Task GetById_ShouldReturn_Ok()
    {
        // Arrange        
        await Seed(4, "Seed Test");

        // Act
        var result = await _decorator.GetIdDto(TicketId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturn_NotFound()
    {
        // Arrange        
        await Seed(4, "Seed Test");

        // Act
        var result = await _decorator.GetIdDto("Ticket-Fake");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetList_ShouldReturn_Ok()
    {
        // Arrange        
        await Seed(4, "Seed Test");

        // Act
        var result = await _decorator.GetListDto();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
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
