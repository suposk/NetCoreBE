using CommonCleanArch.Application;
using NetCoreBE.Application.Tickets;
using SharedContract.Dtos;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class QueryTest : BaseIntegrationTest
{
    private static readonly string TicketId = "Ticket-1";

    public QueryTest(IntegrationTestWebAppFactory factory)
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

    [Fact]
    public async Task GetByIdQuery_ShouldReturn_NotFound()
    {
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = "Ticket-NotFound" };

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
        var q = new SearchTicketQuery { SearchParameters = new TicketSearchParameters() { TicketType = "Laptop" } };

        // Act
        var result = await Sender.Send(q);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value?.Results.Should().NotBeEmpty();        
    }
}