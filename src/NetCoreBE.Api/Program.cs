using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CommonBE.Infrastructure.Persistence;
using CommonBE.Infrastructure.Enums;
using CSRO.Server.Services.Base;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using NetCoreBE.Api.Infrastructure.Persistence;
using System.Reflection;
using System.Security.Claims;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
var configuration = builder.Configuration;
var myType = typeof(Program);
var _namespace = myType.Namespace;

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

services.AddHttpContextAccessor();
services.AddScoped<IApiIdentity, ApiIdentity>();
services.AddScoped<IDateTimeService, DateTimeService>();

services.AddScoped(typeof(IRepository<>), typeof(ApiBaseRepository<>));
services.AddScoped<ITicketRepository, TicketRepository>();
//services.AddScoped<IDbContextFactory<ApiDbContext>>();
//services.AddTransient(provider =>
//    provider.GetRequiredService<IDbContextFactory<ApiDbContext>>().CreateDbContext());
services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
DbTypeEnum DbTypeEnum = DbTypeEnum.Unknown;
try
{
    DbTypeEnum = configuration.GetValue<DbTypeEnum>(nameof(DbTypeEnum));
}
catch { }

if (DbTypeEnum == DbTypeEnum.Unknown)
    throw new Exception($"Unable to read {nameof(DbTypeEnum)} from config. Please set value to SqlServer, InMemory for testing or.....");

services.AddDbContext<ApiDbContext>((p, m) =>
{
    //var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
    //m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
    
    if (DbTypeEnum == DbTypeEnum.SqlLite)
        m.UseSqlite(configuration.GetConnectionString("ApiDbCsLite"), x => x.MigrationsAssembly(_namespace));
    else if (DbTypeEnum == DbTypeEnum.InMemory)
        m.UseInMemoryDatabase(databaseName: configuration.GetConnectionString("ApiDbCs"));
    else
        m.UseSqlServer(configuration.GetConnectionString("ApiDbCs"), x => x.MigrationsAssembly(_namespace));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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
            var s = await ticketRepo.Seed(2, 2, "SEED Startup");
            //seed data
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

await app.RunAsync();
//app.Run();
