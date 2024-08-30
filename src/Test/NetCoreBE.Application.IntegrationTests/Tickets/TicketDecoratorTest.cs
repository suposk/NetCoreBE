using Microsoft.EntityFrameworkCore;
using NetCoreBE.Domain.UnitTests.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class TicketDecoratorTest : TicketIntegrationTest, IAsyncLifetime
{
    private readonly ITicketRepositoryDecorator _decorator;

    public TicketDecoratorTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
        //Seed(4).Wait();
    }
    public Task InitializeAsync() => Seed(4);
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_ShouldReturn_Ok()
    {
        // Arrange        

        // Act
        var result = await _decorator.GetIdDto(TicketData.TicketId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturn_NotFound()
    {
        // Arrange        

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

        // Act
        var result = await _decorator.GetListDto();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Add_ShouldReturn_Ok()
    {
        // Arrange        

        // Act
        var result = await _decorator.AddAsyncDto(TicketData.Add);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    //[Fact(Skip = "Not using UpdateDtoAsync")]
    [Fact]
    public async Task Update_ShouldReturn_Ok()
    {        
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };                
        var old = (await Sender.Send(q)).Value;
        DbContext.ChangeTracker.Clear();
        old.Note = "Update test";

        // Act        
        var result = await _decorator.UpdateDtoAsync2(new TicketUpdateDto { Id = old.Id, Note = old.Note, RowVersion = old.RowVersion });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact(Skip = "Not using UpdateDtoAsync")]
    //[Fact]
    public async Task Update_ShouldReturn_Failed()
    {
        // Arrange
        var dto = TicketData.Update;
        dto.Note = "Update test";

        // Act
        var result = await _decorator.UpdateDtoAsync2(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();        
        result.ErrorMessage.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Remove_ShouldReturn_NoContent()
    {
        // Act
        var result = await _decorator.RemoveAsync(TicketData.TicketId);

        // Assert
        result.IsSuccess.Should().BeTrue();        
        result.ErrorMessage.Should().BeNullOrEmpty();
    }    
}
