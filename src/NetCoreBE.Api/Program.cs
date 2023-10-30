using CommonBE.Infrastructure.Enums;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Identity.Web;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CommonBE.Infrastructure.Interceptors;
using FluentValidation.AspNetCore;
using CommonBE.Infrastructure;
using Carter;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

//Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var services = builder.Services;
var Configuration = builder.Configuration;
var myType = typeof(Program);
var _namespace = myType.Namespace;

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddMvc(options => //Validation filter
{
    options.Filters.Add(new ValidationFilter());
})
.AddFluentValidation(options =>
{    
    options.RegisterValidatorsFromAssemblyContaining<NetCoreBE.Api.Application.BaseAbstractValidator>();
});

services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    //Will automatical sign in user
    //options.FallbackPolicy = options.DefaultPolicy;
    //options.AddPolicy(PoliciesCsro.IsAdminPolicy, policy => policy.RequireClaim(ClaimTypes.Role, RolesCsro.Admin));
});

services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

services.RegisterCommonBEServices(Configuration);

// register PropertyMappingService for Search functionality
services.AddTransient<IPropertyMappingService, PropertyMappingService>();

services.AddScoped(typeof(IRepository<>), typeof(ApiRepositoryBase<>));
services.AddScoped(typeof(IDomainLogicBase<,>), typeof(ApiDomainLogicBase<,>));

services.AddScoped<ITicketRepository, TicketRepository>();
services.AddScoped<ITicketLogic, TicketLogic>();

services.AddScoped<IRequestRepository, RequestRepository>();
services.AddScoped<IRequestLogic, RequestLogic>();

DbTypeEnum DbTypeEnum = DbTypeEnum.Unknown;
try
{
    DbTypeEnum = Configuration.GetValue<DbTypeEnum>(nameof(DbTypeEnum));
}
catch { }

if (DbTypeEnum == DbTypeEnum.Unknown)
    throw new Exception($"Unable to read {nameof(DbTypeEnum)} from config. Please set value to SqlServer, InMemory for testing or.....");

////factory methods, not used yet
//services.AddScoped<IDbContextFactory<ApiDbContext>, DbContextFactory<ApiDbContext>>();
//services.AddTransient<IApiDbContext>(provider =>    
//    provider.GetRequiredService<IDbContextFactory<ApiDbContext>>().CreateDbContext());

services.AddDbContext<ApiDbContext>((p, m) =>
{
    //var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
    //m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
    
    if (DbTypeEnum == DbTypeEnum.SqlLite)
        m.UseSqlite(Configuration.GetConnectionString("ApiDbCsLite"), x => x.MigrationsAssembly(_namespace));
    else if (DbTypeEnum == DbTypeEnum.InMemory)
        m.UseInMemoryDatabase(databaseName: Configuration.GetConnectionString("ApiDbCs"));
    else
        m.UseSqlServer(Configuration.GetConnectionString("ApiDbCs"), x => x.MigrationsAssembly(_namespace));
});

services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.NotificationPublisher = new ParallelNoWaitPublisher();
    //config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
    //config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
    config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
    //config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    //config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>));
    //config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
    //config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
});

var app = builder.Build();
app.UseApiExceptionHandler(options =>
{
    options.AddResponseDetails = ApiExceptionMiddlewareExtensions.UpdateApiErrorResponse;
    options.DetermineLogLevel = ApiExceptionMiddlewareExtensions.DetermineLogLevel;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapCarter();
app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var sp = scope.ServiceProvider;

        var dbContext = sp.GetRequiredService<ApiDbContext>();
        dbContext.Database.Migrate();

        if (app.Environment.IsDevelopment())
        {
            var ticketRepo = sp.GetRequiredService<ITicketRepository>();
            var s2 = await ticketRepo.Seed(2, 2, "SEED Startup");
            //seed data
            var requestRepository = sp.GetRequiredService<IRequestRepository>();
            var s4 = await requestRepository.Seed(4, 4, "SEED Startup");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

await app.RunAsync();
