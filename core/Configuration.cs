using System.Text.Json.Serialization;
using Tomlyn.Serialization;

namespace xarsu;

internal static class Configuration
{
    public const string CONFIGURATION_FILE_NAME = "xarsu.toml";

    public static ConfigurationModel? Current { get; private set; }

    public static void Load(string data)
    {
        Core.ProxyLogger?.Log("Loading configuration from data:");
        Core.ProxyLogger?.Log(data);
        Current = Tomlyn.TomlSerializer.Deserialize(data, ConfigurationContext.Default.ConfigurationModel);
    }

    public static void CreateAndLoadDefault(string path)
    {
        var defaultConfig = new ConfigurationModel
        {
            ModLibraryNames = [],
            Windows = new WindowsConfigurationModel()
            {
                OpenConsole = true
            }
        };
        string tomlData = Tomlyn.TomlSerializer.Serialize(defaultConfig, ConfigurationContext.Default.ConfigurationModel);
        File.WriteAllText(path, tomlData);
        Current = defaultConfig;
    }
}

internal class ConfigurationModel
{
    [JsonPropertyName("mods")]
    public required string[] ModLibraryNames { get; set; }

    [JsonPropertyName("windows_only")]
    public required WindowsConfigurationModel Windows { get; set; }
}

internal class WindowsConfigurationModel
{
    [JsonPropertyName("open_console")]
    public required bool OpenConsole { get; set; } = true;
}

[TomlSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace)]
[TomlSerializable(typeof(ConfigurationModel))]
internal partial class ConfigurationContext : TomlSerializerContext
{
}