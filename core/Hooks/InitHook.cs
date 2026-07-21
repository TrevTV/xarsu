using System.Runtime.InteropServices;
using xarsu.Reference;

namespace xarsu.Hooks;

internal static class InitHook
{
    private delegate IntPtr il2cpp_init_func([MarshalAs(UnmanagedType.LPUTF8Str)] string domain_name);

    private static Dobby.NativeHook<il2cpp_init_func>? _il2cppInitHook;

    public static void DoHook()
    {
        string libraryPath = Core.Bootstrap!.Il2CppAssemblyName;
        if (!NativeLibrary.TryLoad(libraryPath, out var il2cpp))
        {
            Core.ProxyLogger?.LogError($"Failed to load {libraryPath}");
            return;
        }

        if (!NativeLibrary.TryGetExport(il2cpp, "il2cpp_init", out var il2cppInitPtr))
        {
            Core.ProxyLogger?.LogError("Failed to find il2cpp_init export");
            return;
        }

        _il2cppInitHook = new Dobby.NativeHook<il2cpp_init_func>(il2cppInitPtr, Il2CppInitDetour);
        if (_il2cppInitHook.Hook())
        {
            Core.ProxyLogger?.Log("Successfully hooked il2cpp_init");
        }
        else
        {
            Core.ProxyLogger?.LogError("Failed to hook il2cpp_init");
        }
    }

    private static IntPtr Il2CppInitDetour(string domain_name)
    {
        Core.ProxyLogger?.Log($"il2cpp_init called with domain name: {domain_name}");
        IntPtr domain = _il2cppInitHook!.Trampoline!.Invoke(domain_name);
        _il2cppInitHook.Unhook();

        Core.NotifyIl2CppReady();

        InvokeHook.DoHook();

        return domain;
    }
}