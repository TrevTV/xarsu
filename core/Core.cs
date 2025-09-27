using xarsu.Hooks;
using xarsu.Proxy;

namespace xarsu;

internal static class Core
{
    public static IProxyLogger? ProxyLogger { get; set; }

    public static void Init()
    {
        InitHook.DoHook();
    }
}