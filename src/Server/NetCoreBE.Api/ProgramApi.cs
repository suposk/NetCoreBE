using FluentValidation.AspNetCore;
using NetCoreBE.Api;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using NetCoreBE.Api.OpenApi;
using SharedCommon;
using System.Configuration;
using NetCoreBE.Api.Middleware;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using CommonCleanArch.Infrastructure.Infrastructure.Configuration;
using CommonCleanArch.Infrastructure.Infrastructure.EventBus;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

//for retriving secrets from Azure Key Vault only
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Information);
    builder.AddConsole();
    builder.AddEventSourceLogger();
});
var _logger = loggerFactory.CreateLogger(nameof(ProgramApi));
_logger.LogInformation($"Created {nameof(ProgramApi)} _logger");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCarter(); //minimal API

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(AppApiVersions.Default);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .EnableApiVersionBinding()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";        
        options.SubstituteApiVersionInUrl = true;
    });

//Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var services = builder.Services;
var Configuration = builder.Configuration;

services.RegisterCommonCleanArchServices(Configuration, _logger);
//services.RegisterCommonCleanArchServices(Configuration, null);
services.RegisterSharedCommonServices(Configuration);

services.AddMvc(options => //Validation filter
{
    options.Filters.Add(new ValidationFilterMvc());
})
.AddFluentValidation(options =>
{    
    options.RegisterValidatorsFromAssemblyContaining<BaseAbstractValidator>();
});


builder.Services.AddRateLimiter(_ => 
    _.AddFixedWindowLimiter(policyName: "FixedWindowLimiter", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

DbTypeEnum DbTypeEnum = DbTypeEnum.Unknown;
var configuration = builder.Configuration;
string? databaseConnectionString;
try
{    
    DbTypeEnum = configuration.GetValue<DbTypeEnum>(nameof(DbTypeEnum));
}
catch { }
if (DbTypeEnum == DbTypeEnum.Unknown)
    throw new Exception("DbTypeEnum is unknown");

if (DbTypeEnum == DbTypeEnum.SqlLite)
    databaseConnectionString = configuration.GetConnectionStringOrThrow($"{InfrastructureConstants.ConnectionStrings.Database}Lite");
else if (DbTypeEnum == DbTypeEnum.InMemory)
    databaseConnectionString = configuration.GetConnectionStringOrThrow($"{InfrastructureConstants.ConnectionStrings.Database}InMemory");
else if (DbTypeEnum == DbTypeEnum.PostgreSQL)
    databaseConnectionString = configuration.GetConnectionStringOrThrow($"{InfrastructureConstants.ConnectionStrings.Database}PostgreSQL");    
else
    databaseConnectionString = configuration.GetConnectionStringOrThrow(InfrastructureConstants.ConnectionStrings.Database);


var rabbitMqSettings = new RabbitMqSettings(builder.Configuration.GetConnectionStringOrThrow("Queue"));
if (DbTypeEnum == DbTypeEnum.PostgreSQL)
{    
    builder.Services.AddHealthChecks()
        .AddNpgSql(databaseConnectionString)
        .AddRabbitMQ(rabbitConnectionString: rabbitMqSettings.Host)
        ;
}

/* app *********************************************************************************************************************/

var app = builder.Build();
app.UseApiExceptionHandler(options =>
{
    options.AddResponseDetails = ApiExceptionMiddlewareExtensions.UpdateApiErrorResponse;
    options.DetermineLogLevel = ApiExceptionMiddlewareExtensions.DetermineLogLevel;
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (ApiVersionDescription description in app.DescribeApiVersions())
    {
        string url = $"/swagger/{description.GroupName}/swagger.json";
        string name = description.GroupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }
});
var ApiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(AppApiVersions.V1))
    .HasApiVersion(new ApiVersion(AppApiVersions.V2))
    .HasApiVersion(new ApiVersion(AppApiVersions.V3))
    .ReportApiVersions()
    .Build();

var RouteGroupBuilder = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(ApiVersionSet);
RouteGroupBuilder.MapCarter();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContext();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseRateLimiter();
app.AddKeyVaultExtensions();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var sp = scope.ServiceProvider;
        var dbContext = sp.GetRequiredService<ApiDbContext>();
        if (app.Environment.IsDevelopment())
        {            
            //call migration
            if (DbTypeEnum == DbTypeEnum.PostgreSQL)
            {                
                //dbContext.Database.EnsureDeleted();                
                dbContext.Database.Migrate();
            }
            else
            {                
                dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            //seed data
            var SeedDb = configuration.GetValue<bool>("SeedDb");
            if (SeedDb)
            {
                //seed data
                var TicketRepository = sp.GetRequiredService<ITicketRepository>();
                var s4 = await TicketRepository.Seed(4, 4, "SEED Startup");

                var CrudExampleRepository = sp.GetRequiredService<ICrudExampleRepository>();
                var s10 = await CrudExampleRepository.Seed(10, 10, "SEED Startup");
            }
        }
        else        
            dbContext.Database.Migrate();
        
    }
    catch (Exception ex)
    {        
        Log.Error(ex, "An error occurred while migrating or initializing the database.");
    }
}

await app.RunAsync();

namespace NetCoreBE.Api
{
    public partial class ProgramApi { }
}
