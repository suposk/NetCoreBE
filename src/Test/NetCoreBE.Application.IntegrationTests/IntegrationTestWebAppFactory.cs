using NetCoreBE.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
//using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using NetCoreBE.Api;
using NetCoreBE.Infrastructure.Persistence;
using NetCoreBE.Application.Interfaces;
using Quartz;
using Microsoft.Extensions.Hosting;
//using Testcontainers.Redis;

namespace NetCoreBE.Application.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<ProgramApi>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("netcorebe")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");            

            services.RemoveAll(typeof(DbContextOptions<ApiDbContext>));
            services.RemoveAll(typeof(IApiDbContext));
            services.RemoveAll(typeof(IHostedService)); //backround jobs

            string connectionString = $"{_dbContainer.GetConnectionString()};Pooling=False";

            services.AddDbContext<ApiDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());

            services.AddScoped<IApiDbContext>(provider => provider.GetService<ApiDbContext>());                   
        });

        builder.UseEnvironment("Production");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
