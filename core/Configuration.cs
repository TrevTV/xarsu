using System.Text.Json.Serialization;
using Tomlyn.Serialization;
using xarsu.Proxy;

namespace xarsu;

internal static class Configuration
{
    public const string CONFIGURATION_FILE_NAME = "xarsu.toml";

    public static ConfigurationModel? Current { get; private set; }

    public static void Load(string data)
    {
        Current = Tomlyn.TomlSerializer.Deserialize(data, ConfigurationContext.Default.ConfigurationModel);
    }

    public static void CreateAndLoadDefault(string path)
    {
        var defaultConfig = new ConfigurationModel
        {
            ModLibraryNames = [],
            Logging = new LoggingConfigurationModel()
            {
                LogLevel = ProxyLogger.LogLevel.Info,
                LogToFile = false
            },
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

    [JsonPropertyName("logging")]
    public required LoggingConfigurationModel? Logging { get; set; } = null;

    [JsonPropertyName("windows_only")]
    public WindowsConfigurationModel? Windows { get; set; } = null;
}

internal class LoggingConfigurationModel
{
    [JsonPropertyName("log_level")]
    public required ProxyLogger.LogLevel LogLevel { get; set; } = ProxyLogger.LogLevel.Info;
    [JsonPropertyName("log_to_file")]
    public required bool LogToFile { get; set; } = false;
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