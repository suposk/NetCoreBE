using FluentValidation.AspNetCore;
using NetCoreBE.Api;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using NetCoreBE.Api.OpenApi;
using SharedCommon;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var ApiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(AppApiVersions.V1))
    .HasApiVersion(new ApiVersion(AppApiVersions.V2))
    .HasApiVersion(new ApiVersion(AppApiVersions.V3))
    .ReportApiVersions()
    .Build();

var RouteGroupBuilder = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(ApiVersionSet);
RouteGroupBuilder.MapCarter(); 
//app.MapCarter(); 

//app.UseSerilogRequestLogging();

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
            var configuration = sp.GetRequiredService<IConfiguration>();
            DbTypeEnum DbTypeEnum = DbTypeEnum.Unknown;
            try
            {
                DbTypeEnum = configuration.GetValue<DbTypeEnum>(nameof(DbTypeEnum));
            }
            catch { }
            if (DbTypeEnum == DbTypeEnum.PostgreSQL)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
            else
                dbContext.Database.Migrate();            

            var OldTicketRepo = sp.GetRequiredService<IOldTicketRepository>();
            var s2 = await OldTicketRepo.Seed(2, 2, "SEED Startup");
            //seed data
            var TicketRepository = sp.GetRequiredService<ITicketRepository>();
            var s4 = await TicketRepository.Seed(4, 4, "SEED Startup");
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
