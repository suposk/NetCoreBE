using NetCoreBE.Domain.UnitTests.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class QueryTest : TicketIntegrationTest
{
    public QueryTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        Seed(4).Wait();    
    }

    [Fact]
    public async Task GetByIdQuery_ShouldReturn_Ok()
    {
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };

        // Act
        var result = await Sender.Send(q);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetByIdQuery_ShouldReturn_NotFound()
    {
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = "Ticket-Fake" };

        // Act
        var result = await Sender.Send(q);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetListQuery_ShouldReturn_Ok()
    {
        // Arrange        
        var q = new GetListQuery<TicketDto>();

        // Act
        var result = await Sender.Send(q);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchTicketQuery_Laptop_ShouldReturn_Ok()
    {
        // Arrange        
        var q = new SearchTicketQuery { SearchParameters = new() { TicketType = TicketTypeEnum.Access.ToString() } };

        // Act
        var result = await Sender.Send(q);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value?.Results.Should().NotBeEmpty();        
    }
}