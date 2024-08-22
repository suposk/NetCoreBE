using Microsoft.EntityFrameworkCore;
using NetCoreBE.Domain.UnitTests.Tickets;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class TicketDecoratorTest : TicketIntegrationTest
{
    private readonly ITicketRepositoryDecorator _decorator;

    public TicketDecoratorTest(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
        Seed(4).Wait();
    }


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

    [Fact]
    public async Task Update_ShouldReturn_Ok()
    {        
        // Arrange        
        var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };                
        var old = (await Sender.Send(q)).Value;
        DbContext.ChangeTracker.Clear();
        old.Note = "Update test";

        // Act        
        var result = await _decorator.UpdateDtoAsync(old);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturn_Failed()
    {
        // Act
        var obj = TicketData.Add;
        obj.Note = "Update test";

        // Act
        var result = await _decorator.UpdateDtoAsync(obj);

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
