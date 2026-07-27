using xarsu.Hooks;
using xarsu.Proxy;

namespace xarsu;

internal static class Core
{
    public static ProxyLogger? ProxyLogger { get; set; }
    public static IProxyBootstrap? Bootstrap { get; private set; }

    private readonly static List<Library> _loadedLibraries = [];

    public static void Init(IProxyBootstrap bootstrap)
    {
        Bootstrap = bootstrap;
        ProxyLogger?.Log("Initializing core...");

        InitHook.DoHook();

        foreach (var library in bootstrap.LoadLibraries())
        {
            ProxyLogger?.Log($"Loaded library: {library.Name}");
            _loadedLibraries.Add(library);
            library.InvokeLoad();
        }
    }

    public static void NotifyIl2CppReady()
    {
        foreach (var library in _loadedLibraries)
        {
            library.InvokeIl2CppReady();
        }
    }

    public static void NotifySceneChanged(string? oldScene, string? newScene)
    {
        foreach (var library in _loadedLibraries)
        {
            library.InvokeSceneChanged(oldScene, newScene);
        }
    }

    public static void NotifyUpdate()
    {
        foreach (var library in _loadedLibraries)
        {
            library.InvokeUpdate();
        }
    }
}