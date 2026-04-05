using xarsu.Hooks;
using xarsu.Proxy;

namespace xarsu;

internal static class Core
{
    public static IProxyLogger? ProxyLogger { get; set; }
    public static IProxyBootstrap? Bootstrap { get; private set; }

    public static List<Library> LoadedLibraries = [];

    public static void Init(IProxyBootstrap bootstrap)
    {
        Bootstrap = bootstrap;
        ProxyLogger?.Log("Initializing core...");

        if (!bootstrap.TryLoadConfiguration())
        {
            ProxyLogger?.Log("Failed to load configuration, aborting initialization.");
            return;
        }

        InitHook.DoHook();

        foreach (var library in bootstrap.LoadLibraries())
        {
            ProxyLogger?.Log($"Loaded library: {library.Name}");
            LoadedLibraries.Add(library);
            library.InvokeLoad();
        }
    }

    public static void NotifyIl2CppReady()
    {
        foreach (var library in LoadedLibraries)
        {
            library.InvokeIl2CppReady();
        }
    }
}