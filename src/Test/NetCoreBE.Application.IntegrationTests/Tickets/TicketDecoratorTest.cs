using Microsoft.EntityFrameworkCore;
using NetCoreBE.Domain.UnitTests.CrudExamples;
using NetCoreBE.Domain.UnitTests.Tickets;
using Xunit.Abstractions;

namespace NetCoreBE.Application.IntegrationTests.Tickets;

public class TicketDecoratorTest : TicketIntegrationTest, IAsyncLifetime
{
    private readonly ITicketRepositoryDecorator _decorator;
    private readonly ITestOutputHelper _testOutputHelper;

    public TicketDecoratorTest(IntegrationTestWebAppFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ITicketRepositoryDecorator>();
        _testOutputHelper = testOutputHelper;
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
        ////// Arrange        
        ////var q = new GetByIdQuery<TicketDto> { Id = TicketData.TicketId };                
        ////var old = (await Sender.Send(q)).Value;
        ////DbContext.ChangeTracker.Clear();
        ////old.Note = "Update test";

        ////// Act        
        ////var result = await _decorator.UpdateDto(new TicketUpdateDto { Id = old.Id, Note = old.Note, RowVersion = old.RowVersion });

        //// Arrange       
        //var old = (await _decorator.GetIdDto(TicketData.TicketId)).Value;        
        //_decorator.DatabaseContext.ChangeTracker.Clear();
        //old.Note = "Update test";
        //_testOutputHelper.WriteLine($"Old Id: {old.Id},  RowVersion: {old?.RowVersion}");

        ////var ctx = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        //var ctx = _decorator.DatabaseContext as ApiDbContext;
        //var all = await ctx.Tickets.ToListAsync();
        //foreach (var item in all)        
        //    _testOutputHelper.WriteLine($"Id: {item.Id},  RowVersion: {item.RowVersion}");        
        //ctx.ChangeTracker.Clear();        

        //// Act        
        //var result = await _decorator.UpdateDto(new TicketUpdateDto { Id = old.Id, Note = old.Note, RowVersion = old.RowVersion });

        //works, returns same RowVersion 
        var ctx = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();        
        var old = await ctx.Tickets.FirstOrDefaultAsync(x => x.Id == TicketData.TicketId);
        _testOutputHelper.WriteLine($"Old Id: {old.Id},  RowVersion: {old?.RowVersion}");
        ctx.ChangeTracker.Clear();
        var dto = new TicketUpdateDto { Id = old.Id, Note = "Update t1", RowVersion = old.RowVersion };

        // Act        
        var result = await _decorator.UpdateDto(new TicketUpdateDto { Id = old.Id, Note = "Update t1", RowVersion = old.RowVersion });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
        result.Value?.RowVersion.Should().NotBe(old.RowVersion);
    }

    //[Fact(Skip = "Not using UpdateDtoAsync")]
    [Fact]
    public async Task Update_ShouldReturn_Failed()
    {
        // Arrange
        var ctx = Scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        var old = await ctx.Tickets.FirstOrDefaultAsync(x => x.Id == TicketData.TicketId);
        _testOutputHelper.WriteLine($"Old Id: {old.Id},  RowVersion: {old?.RowVersion}");
        ctx.ChangeTracker.Clear();
        var dto = new TicketUpdateDto { Id = old.Id, Note = "Update t1", RowVersion = old.RowVersion };
        dto.RowVersion += 1;

        // Act
        var result = await _decorator.UpdateDto(dto);        

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
