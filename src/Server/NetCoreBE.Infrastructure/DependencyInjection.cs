﻿using CommonCleanArch.Infrastructure.Infrastructure.EventBus;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using NetCoreBE.Infrastructure.BackroundJobs;
using Quartz;
using System.Configuration;
using System.Xml.Linq;
using CommonCleanArch.Infrastructure.Infrastructure.Configuration;
using NetCoreBE.Application.Tickets.IntegrationEvents;
using CommonCleanArch.Application.EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Trace;
using Npgsql;
using OpenTelemetry.Resources;
using NetCoreBE.Infrastructure.EventBus.Tickets;

namespace NetCoreBE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // register PropertyMappingService for Search functionality
        services.AddTransient<IPropertyMappingService, PropertyMappingService>();

        DbTypeEnum DbTypeEnum = AddPersistence(services, configuration);

        AddQueries(services);

        AddCaching(services, configuration);

        AddAuthentication(services, configuration);

        AddAuthorization(services);

        //AddHealthChecks(services, configuration);

        AddBackgroundJobs(services, configuration);


        services.TryAddSingleton<IEventBus, CommonCleanArch.Infrastructure.Infrastructure.EventBus.EventBus>();
        var rabbitMqSettings = new RabbitMqSettings(configuration.GetConnectionStringOrThrow("Queue"));
        services.AddMassTransit(configure =>
        {
            configure.AddConsumer<TicketCanceledIntegrationEventConsumer>();

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqSettings.Host), h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        //todo not working properly. Need to investigate
        var serviceName = "NetCoreBE";
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    //.AddRedisInstrumentation()
                    //.AddNpgsql()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
                //if (DbTypeEnum == DbTypeEnum.PostgreSQL)
                //    tracing.AddNpgsql();

                tracing.AddOtlpExporter();
            });

        return services;
    }

    private static DbTypeEnum AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        DbTypeEnum DbTypeEnum = DbTypeEnum.Unknown;
        try
        {
            DbTypeEnum = configuration.GetValue<DbTypeEnum>(nameof(DbTypeEnum));
        }
        catch { }
        if (DbTypeEnum == DbTypeEnum.Unknown)
            throw new Exception($"Unable to read {nameof(DbTypeEnum)} from config. Please set value to SqlServer, InMemory for testing or.....");

        var _namespace = typeof(DependencyInjection).Namespace;

        services.AddDbContext<ApiDbContext>((p, m) =>
        {
            //var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
            //m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);

            if (DbTypeEnum == DbTypeEnum.SqlLite)
                m.UseSqlite(configuration.GetConnectionString($"{InfrastructureConstants.ConnectionStrings.Database}Lite"), x => x.MigrationsAssembly(_namespace));
            else if (DbTypeEnum == DbTypeEnum.InMemory)
                m.UseInMemoryDatabase(configuration.GetConnectionString($"{InfrastructureConstants.ConnectionStrings.Database}InMemory"));
            else if (DbTypeEnum == DbTypeEnum.PostgreSQL)
            //m.UseNpgsql(configuration.GetConnectionString($"{InfrastructureConstants.ConnectionStrings.Database}PostgreSQL"), x => x.MigrationsAssembly(_namespace));
            //m.UseNpgsql(configuration.GetConnectionString($"{InfrastructureConstants.ConnectionStrings.Database}PostgreSQL")).UseSnakeCaseNamingConvention();
            {
                var connectionString = configuration.GetConnectionString($"{InfrastructureConstants.ConnectionStrings.Database}PostgreSQL");
                m.UseNpgsql(connectionString);
            }
            else
                m.UseSqlServer(configuration.GetConnectionString(InfrastructureConstants.ConnectionStrings.Database), x => x.MigrationsAssembly(_namespace));
        });
        // Register the service and implementation for the database context
        services.AddScoped<IApiDbContext>(provider => provider.GetService<ApiDbContext>());

        //generic types
        services.AddScoped(typeof(IRepository<>), typeof(ApiRepositoryBase<>));        
        services.AddScoped(typeof(IRepositoryDecoratorBase<,>), typeof(ApiRepositoryDecoratorBase<,>));


        services.AddScoped<IOutboxDomaintEventRepository, OutboxDomaintEventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ICrudExampleRepository, CrudExampleRepository>();        

        services.AddScoped<ITicketRepositoryDecorator, TicketRepositoryDecorator>();
        services.AddScoped<ICrudExampleRepositoryDecorator, CrudExampleRepositoryDecorator>();

        ////factory methods, not used yet
        //services.AddScoped<IDbContextFactory<ApiDbContext>, DbContextFactory<ApiDbContext>>();
        //services.AddTransient<IApiDbContext>(provider =>    
        //    provider.GetRequiredService<IDbContextFactory<ApiDbContext>>().CreateDbContext());
        return DbTypeEnum;
    }

    private static void AddQueries(IServiceCollection services)
    {
        //generic version 
        //services.AddScoped(typeof(IRequest<>), typeof(GetByIdQueryBaseApi<,>));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(GetByIdQueryHandlerBaseApi<,>));

        //manual registration
        services.AddScoped(typeof(IRequestHandler<GetByIdQuery<TicketDto>, ResultCom<TicketDto>>),
            typeof(GetByIdQueryHandlerRequest));
        services.AddScoped(typeof(IRequestHandler<GetListQuery<TicketDto>, ResultCom<List<TicketDto>>>),
            typeof(GetListQueryHandler<Ticket, TicketDto>));

        services.AddScoped(typeof(IRequestHandler<GetByIdQuery<TicketHistoryDto>, ResultCom<TicketHistoryDto>>),
            typeof(GetByIdQueryHandler<TicketHistory, TicketHistoryDto>));
        services.AddScoped(typeof(IRequestHandler<GetListQuery<TicketHistoryDto>, ResultCom<List<TicketHistoryDto>>>),
            typeof(GetListQueryHandler<TicketHistory, TicketHistoryDto>));
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        #region JWT
        //services
        //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer();

        //services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
        //services.ConfigureOptions<JwtBearerOptionsSetup>();
        //services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));
        //services.AddTransient<AdminAuthorizationDelegatingHandler>();
        //services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        //{
        //    KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

        //    httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        //})
        //.AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        //services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        //{
        //    KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

        //    httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        //});
        #endregion

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration, "AzureAd")
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        //services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy
            //Will automatical sign in user
            //options.FallbackPolicy = options.DefaultPolicy;
            //options.AddPolicy(PoliciesCsro.IsAdminPolicy, policy => policy.RequireClaim(ClaimTypes.Role, RolesCsro.Admin));
        });

        //services.AddScoped<AuthorizationService>();
        //services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        //string connectionString = configuration.GetConnectionString("Cache") ??
        //                          throw new ArgumentNullException(nameof(configuration));

        //services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
        //services.AddSingleton<ICacheService, CacheService>();
    }

    //private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    //{
    //    //services.AddHealthChecks()
    //    //    .AddNpgSql(configuration.GetConnectionString("Database")!)
    //    //    .AddRedis(configuration.GetConnectionString("Cache")!)
    //    //    .AddUrlGroup(new Uri(configuration["KeyCloak:BaseUrl"]!), HttpMethod.Get, "keycloak");
    //}

    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
        //services.AddQuartz();
        //services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        //services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();

        services.AddQuartz(config =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxDomaintEventsJob));
            config
            .AddJob<ProcessOutboxDomaintEventsJob>(jobKey)
            .AddTrigger(options =>
            {
                options.ForJob(jobKey);
                //options.StartNow();        
                options.StartAt(DateTimeOffset.UtcNow.AddSeconds(30));
                options.WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever());
            });
            config.UseMicrosoftDependencyInjectionJobFactory();//Important for DI
        });
        services.AddQuartzHostedService();
    }
}
