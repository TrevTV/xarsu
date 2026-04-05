using System.Text.Json.Serialization;
using Tomlyn.Serialization;

namespace xarsu;

internal static class Configuration
{
    public static ConfigurationModel? Current { get; private set; }

    public static void Load(string data)
    {
        Core.ProxyLogger?.Log("Loading configuration from data:");
        Core.ProxyLogger?.Log(data);
        Current = Tomlyn.TomlSerializer.Deserialize(data, ConfigurationContext.Default.ConfigurationModel);
    }
}


internal class ConfigurationModel
{
    [JsonPropertyName("mods")]
    public required string[] ModLibraryNames { get; set; }
}

[TomlSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace)]
[TomlSerializable(typeof(ConfigurationModel))]
internal partial class ConfigurationContext : TomlSerializerContext
{
}