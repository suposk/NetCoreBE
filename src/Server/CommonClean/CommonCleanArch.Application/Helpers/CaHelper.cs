using Newtonsoft.Json;

namespace CommonCleanArch.Application.Helpers;

public static class CaHelper
{    
    public static JsonSerializerSettings JsonSerializerSettingsNone = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
}
