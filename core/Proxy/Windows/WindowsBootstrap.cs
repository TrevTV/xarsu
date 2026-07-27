#if WINDOWS
using System.Runtime.InteropServices;
using System.Text;

namespace xarsu.Proxy.Windows;

internal partial class WindowsBootstrap : IProxyBootstrap
{
    public string? DataDirectory { get; }

    public string Il2CppAssemblyName => "GameAssembly.dll";

    public static void TryInitCore()
    {
        WindowsBootstrap bootstrap = new();
        Core.Init(bootstrap);
    }

    public WindowsBootstrap()
    {
        DataDirectory = Path.Combine(AppContext.BaseDirectory, "xarsu");

        if (!TryLoadConfiguration())
        {
            Core.ProxyLogger?.Log("Failed to load configuration, aborting initialization.");
            return;
        }

        if (Configuration.Current?.Windows?.OpenConsole ?? true)
        {
            AllocConsole();

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));

            Console.OutputEncoding = Encoding.UTF8;
        }

        Core.ProxyLogger = new(new WindowsLogger());

        if (!Directory.Exists(DataDirectory))
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
            }
            catch (Exception ex)
            {
                Core.ProxyLogger?.LogError($"Failed to create data directory at {DataDirectory}: {ex.Message}");
                Core.ProxyLogger?.LogError(ex.StackTrace);
            }
        }
    }

    private bool TryLoadConfiguration()
    {
        string configPath = Path.Combine(DataDirectory!, Configuration.CONFIGURATION_FILE_NAME);
        if (!File.Exists(configPath))
        {
            Core.ProxyLogger?.Log($"Configuration file not found at {configPath}, creating default configuration.");
            try
            {
                Configuration.CreateAndLoadDefault(configPath);
                return true;
            }
            catch (Exception ex)
            {
                Core.ProxyLogger?.LogError("Failed to create default configuration file: " + ex.Message);
                Core.ProxyLogger?.LogError(ex.StackTrace);
                return false;
            }
        }

        Core.ProxyLogger?.Log($"Loading configuration from {configPath}");

        try
        {
            Configuration.Load(File.ReadAllText(configPath));
            return true;
        }
        catch (Exception ex)
        {
            Core.ProxyLogger?.LogError("Failed to load configuration file: " + ex.Message);
            Core.ProxyLogger?.LogError(ex.StackTrace);
        }

        return false;
    }

    public IEnumerable<Library> LoadLibraries()
    {
        string[] libraries = Directory.GetFiles(DataDirectory!, "*.dll", SearchOption.TopDirectoryOnly);
        foreach (string libraryPath in libraries)
        {
            Core.ProxyLogger?.Log($"Found library at {libraryPath}, attempting to load...");

            if (Library.TryLoad(libraryPath, out Library? library) && library != null)
            {
                yield return library;
            }
            else
            {
                Core.ProxyLogger?.LogError($"Failed to load library at {libraryPath}");
            }
        }
    }

    public bool TryLoadRawLibrary(string path, out nint handle)
    {
        handle = 0;
        try
        {
            handle = NativeLibrary.Load(path);
            return true;
        }
        catch (Exception ex)
        {
            Core.ProxyLogger?.LogError($"Failed to load library at {path}: {ex.Message}");
            Core.ProxyLogger?.LogError(ex.StackTrace);
            return false;
        }
    }

    [LibraryImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();
}
#endif