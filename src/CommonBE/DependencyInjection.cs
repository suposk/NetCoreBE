using CommonBE.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonBE;
public static class DependencyInjection
{
    /// <summary>
    /// Common Service registration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCommonBEServices(this IServiceCollection services, ConfigurationManager? configuration = null)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IApiIdentity, ApiIdentity>();
        services.AddScoped<IDateTimeService, DateTimeService>();        
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        //AddSingleton
        services.AddSingleton<ICacheProvider, CacheProvider>(); //can be configer in config setting in appsettings.json
        return services;
    }
}
