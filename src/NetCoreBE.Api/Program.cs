using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
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
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//services.AddMediatR(Assembly.GetExecutingAssembly());

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

var scopeFactory = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    try
    {
        var sp = scope.ServiceProvider;
        //var dbContext = sp.GetRequiredService<DatabaseContext>();
        //dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

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

app.Run();
