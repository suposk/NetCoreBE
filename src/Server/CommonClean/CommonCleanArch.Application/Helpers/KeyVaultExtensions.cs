using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommonCleanArch.Application.Helpers;

public static class KeyVaultExtensions
{
    //todo Microsoft.Extensions.Configuration.AzureKeyVault replace Microsoft.Azure.KeyVault

    /// <summary>
    /// Read from keyvault Configuration setting and ovveride them. 
    /// KeyVaultConfig must be set in appsettings.json
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder AddKeyVaultExtensions(this IApplicationBuilder builder)
    {
        ILogger? logger = null;
        try
        {           
            IConfiguration Configuration = builder.ApplicationServices.GetRequiredService<IConfiguration>();
            logger = builder.ApplicationServices.GetService<ILogger<IApplicationBuilder>>();

            KeyVaultConfig? keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
            if (keyVaultConfig is null)
                throw new Exception($"{nameof(KeyVaultConfig)} is required in appsettings.json");

            logger?.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig?.UseKeyVault}");
            logger?.LogInformation($"{nameof(KeyVaultConfig.UseKeyVaultList)} = {keyVaultConfig?.UseKeyVaultList}");

            if (keyVaultConfig?.UseKeyVaultList is true && keyVaultConfig?.UseKeyVault is true)
                throw new Exception("only one of KeyVaultConfig or AzureAppConfigurationConfig can be true in appsettings.json");

            KeyVaultClient? keyVaultClient = null;
            if (keyVaultConfig?.UseKeyVaultList is true || keyVaultConfig?.UseKeyVault is true)
            {
                logger?.LogInformation($"{nameof(KeyVaultConfig.KeyVaultName)} = {keyVaultConfig.KeyVaultName}");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));                
            }

            //https://code-maze.com/aspnetcore-get-json-array-using-iconfiguration/

            if (keyVaultConfig?.UseKeyVaultList is true && keyVaultConfig?.SettingToVaultNameList?.Count > 0)
            {
                foreach (var item in keyVaultConfig.SettingToVaultNameList)
                {
                    if (item.Load && item.VaultSecretName != null && item.Setting is not null)
                    {
                        try
                        {
                            var value = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, item.VaultSecretName).Result.Value;
                            //logger?.LogInformation($"Read from vault Setting = {item.Setting}");
                            logger?.LogWarning($"Read from vault Setting = {item.Setting}");
                            Configuration[item.Setting] = value;
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Error reading from vault VaultSecretName = {vsn} Setting = {set}", item.VaultSecretName, item.Setting);                            
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError("Error reading KeyVaultClient = {e}", ex);
            //throw; // stop app from starting
        }
        return builder;
    }
}
