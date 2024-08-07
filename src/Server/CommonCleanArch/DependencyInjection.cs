using Azure.Identity;
using CommonCleanArch.Infrastructure;
using CommonCleanArch.Infrastructure.Interceptors;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedCommon;

namespace CommonCleanArch;
public static class DependencyInjection
{
    /// <summary>
    /// Common Service registration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="Configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCommonCleanArchServices(this IServiceCollection services, ConfigurationManager Configuration, ILogger? logger)
    {
        if (services is null)        
            throw new ArgumentNullException(nameof(services));        

        if (Configuration is null)        
            throw new ArgumentNullException(nameof(Configuration));

        //services.AddHttpContextAccessor(); comes from SharedCommon
        services.AddScoped<IApiIdentity, ApiIdentity>();
        //services.AddScoped<IDateTimeService, DateTimeService>();        
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IEmailService, EmailService>();

        //AddSingleton
        //services.AddSingleton<ICacheProvider, CacheProvider>(); //can be configer in config setting in appsettings.json

        KeyVaultConfig? keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
        AzureAppConfigurationConfig? azureAppConfigurationConfig = Configuration.GetSection(nameof(azureAppConfigurationConfig)).Get<AzureAppConfigurationConfig>();

        if (keyVaultConfig is not null)
        {
            logger?.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig?.UseKeyVault}");
            logger?.LogInformation($"{nameof(KeyVaultConfig.UseKeyVaultList)} = {keyVaultConfig?.UseKeyVaultList}");
        }
        if (azureAppConfigurationConfig is not null)
        {
            logger?.LogInformation($"{nameof(azureAppConfigurationConfig.UseKeyVault)} = {azureAppConfigurationConfig?.UseKeyVault}");            
        }

        if (keyVaultConfig?.UseKeyVaultList is true && keyVaultConfig?.UseKeyVault is true)
            throw new Exception("only one of KeyVaultConfig or AzureAppConfigurationConfig can be true in appsettings.json");

        if (keyVaultConfig is null && azureAppConfigurationConfig is null)
            throw new Exception("KeyVaultConfig or AzureAppConfigurationConfig is required in appsettings.json");

        if (keyVaultConfig?.UseKeyVaultList is true && azureAppConfigurationConfig?.UseKeyVault is true)
            throw new Exception("KeyVaultConfig or AzureAppConfigurationConfig is required in appsettings.json");

        if (azureAppConfigurationConfig?.UseKeyVault is true)
        {
            //versions with refersh
            //https://medium.com/dotnet-hub/use-azure-app-configuration-with-asp-net-core-application-6bdf0cc851e2
            //https://github.com/a-patel/azure-app-configuration-labs

            //set key vault refresh
            Configuration.AddAzureAppConfiguration(options =>
            {
                string? ConnectionStrings__AppConfig = Configuration["ConnectionStrings:AppConfig"];
                if (ConnectionStrings__AppConfig is null)
                    throw new Exception("ConnectionStrings:AppConfig is null. Set refference in azure appconfigurationn to your key vault.");

                //works but no refresh
                if (azureAppConfigurationConfig.AppConfigurationList?.Count > 0)
                {
                    options.Connect(ConnectionStrings__AppConfig)
                            .ConfigureKeyVault(kv =>
                            {
                                foreach (var item in azureAppConfigurationConfig.AppConfigurationList)
                                    kv.SetSecretRefreshInterval(item, TimeSpan.FromMinutes(azureAppConfigurationConfig.KeyVaultRefreshIntervalMinutes));

                                kv.SetCredential(new DefaultAzureCredential());
                                //kv.SetCredential(new ManagedIdentityCredential("ee2f0320-29c3-432a-bf84-a5d4277ce052"));                                                            
                            });
                }
                else
                {
                    options.Connect(ConnectionStrings__AppConfig)
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(new DefaultAzureCredential());
                                //kv.SetCredential(new ManagedIdentityCredential("ee2f0320-29c3-432a-bf84-a5d4277ce052"));
                                kv.SetSecretRefreshInterval("Some:Setting:Value", TimeSpan.FromMinutes(azureAppConfigurationConfig.KeyVaultRefreshIntervalMinutes));
                            });
                }

                //filter by label
                //options.Connect(ConnectionStrings__AppConfig)
                //       // Load all keys that start with `TestApp:` and have no label
                //       .Select("Some:*", LabelFilter.Null)
                //       // Configure to reload configuration if the registered sentinel key is modified
                //       .ConfigureRefresh(refreshOptions =>
                //            refreshOptions.Register("Some:Setting:Value", refreshAll: true));

            });
        }

        return services;
    }
}
