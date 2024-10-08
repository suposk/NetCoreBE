﻿
using Microsoft.EntityFrameworkCore;
using NetCoreBE.Domain.UnitTests.CrudExamples;
using Xunit.Abstractions;

namespace NetCoreBE.Application.IntegrationTests.CrudExamples;
public class CrudExampleDecoratorTest : CrudExampleIntegrationTest, IAsyncLifetime
{
    private readonly ICrudExampleRepositoryDecorator _decorator;
    private readonly ITestOutputHelper _testOutputHelper;

    public CrudExampleDecoratorTest(IntegrationTestWebAppFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory)
    {
        _decorator = Scope.ServiceProvider.GetRequiredService<ICrudExampleRepositoryDecorator>();
        _testOutputHelper = testOutputHelper;
    }

    public Task InitializeAsync() => Seed(4);
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_ShouldReturn_Ok()
    {
        // Arrange        

        // Act
        var result = await _decorator.GetIdDto(CrudExampleData.CrudExampleId);

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
        var result = await _decorator.GetIdDto("CrudExample-Fake");

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
    public async Task Create_ShouldReturn_Ok()
    {
        // Arrange        
        var dto = CrudExampleData.Add;

        // Act
        var result = await _decorator.AddAsyncDto(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturn_Ok()
    {
        // Arrange        
        //var dto = CrudExampleData.Update;

        //Arange with Read from Seed Record
        var old = await _decorator.GetIdDto(CrudExampleData.CrudExampleId);
        _decorator.DatabaseContext.ChangeTracker.Clear();
        var dto = old.Value;
        dto.Name = CrudExampleData.Update.Name;
        dto.Description = CrudExampleData.Update.Description;


        //await ArrangeUpdate();
        _testOutputHelper.WriteLine($"RowVersion: {dto?.RowVersion}");

        // Act
        var result = await _decorator.UpdateDtoAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNullOrEmpty();
        result.Value?.RowVersion.Should().NotBe(dto.RowVersion);

        _testOutputHelper.WriteLine($"RowVersion: {result.Value?.RowVersion}");        
        _testOutputHelper.WriteLine($"ErrorMessage", result.ErrorMessage);
    }

    [Fact]
    public async Task Update_ShouldReturn_Failed()
    {
        //// Arrange, bug will set static       
        //var dto = CrudExampleData.Update;

        //Arange with Read from Seed Record
        var old = await _decorator.GetIdDto(CrudExampleData.CrudExampleId);
        _decorator.DatabaseContext.ChangeTracker.Clear();
        var dto = old.Value;
        dto.Name = CrudExampleData.Update.Name;
        dto.Description = CrudExampleData.Update.Description;

        dto.RowVersion += 1;

        // Act
        var result = await _decorator.UpdateDtoAsync(dto);
        dto.RowVersion -= 1;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Remove_ShouldReturn_Ok()
    {
        // Arrange        

        // Act
        var result = await _decorator.RemoveAsync(CrudExampleData.CrudExampleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }
}
