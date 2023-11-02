using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedCommon;
public static class DependencyInjection
{
    /// <summary>
    /// Common Service registration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSharedCommonServices(this IServiceCollection services, ConfigurationManager? configuration = null)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IDateTimeService, DateTimeService>();

        //AddSingleton
        services.AddSingleton<ICacheProvider, CacheProvider>(); //can be configer in config setting in appsettings.json
        return services;
    }
}
