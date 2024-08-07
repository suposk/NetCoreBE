namespace SharedCommon;

/// <summary>
/// Azure App Configuration version. Load setting from keyvault from AppConfigurationList using Azure AppConfiguration
/// </summary>
public class AzureAppConfigurationConfig
{
    public bool? UseKeyVault { get; set; }
    public int KeyVaultRefreshIntervalMinutes { get; set; } = 5;
    public List<string>? AppConfigurationList { get; set; }
    public string? Endpoint { get; set; }
}
