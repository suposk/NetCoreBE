namespace SharedCommon;

public class SettingToVaultName
{
    public string? Setting { get; set; }
    public string? VaultSecretName { get; set; }
    public bool Load { get; set; } = true;
}

/// <summary>
/// KeyVaultClient and AzureServiceTokenProvider version. Load setting from keyvault from SettingToVaultKeyList pairs usiung 
/// </summary>
public class KeyVaultConfig
{
    public string? KeyVaultName { get; set; }

    //old version
    public bool UseKeyVault { get; set; }

    /// <summary>
    /// New version. Load setting from keyvault from SettingToVaultKeyList pairs
    /// </summary>
    public bool UseKeyVaultList { get; set; }

    public List<SettingToVaultName>? SettingToVaultNameList { get; set; }

    public static class Constants
    {
        public const string AzureAdClientSecret = "AzureAd:ClientSecret";
        public const string SpnClientSecret = "SpnAd:ClientSecret";
    }

    public static class ConnectionStrings
    {
        public const string TokenCacheDb = "TokenCacheDbCs";
        public const string AuthDb = "AuthDbCs";
        public const string ApiDb = "Database";
        public const string AdoDb = "AdoDbCs";
        public const string ReportDb = "ReportDbCs";
        public const string CustomerDb = "CustomerDbCs";
        public const string AzureServiceBus = "AzureServiceBus";
    }
}
