using System.Runtime.InteropServices;

namespace xarsu;

internal static class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "XarsuLog")]
    public static void Log(IntPtr messagePtr)
    {
        string message = Marshal.PtrToStringUni(messagePtr) ?? "null";
        Core.ProxyLogger?.Log(message);
    }

    [UnmanagedCallersOnly(EntryPoint = "XarsuLogWarning")]
    public static void LogWarning(IntPtr messagePtr)
    {
        string message = Marshal.PtrToStringUni(messagePtr) ?? "null";
        Core.ProxyLogger?.LogWarning(message);
    }

    [UnmanagedCallersOnly(EntryPoint = "XarsuLogError")]
    public static void LogError(IntPtr messagePtr)
    {
        string message = Marshal.PtrToStringUni(messagePtr) ?? "null";
        Core.ProxyLogger?.LogError(message);
    }

    [UnmanagedCallersOnly(EntryPoint = "XarsuLogVerbose")]
    public static void LogVerbose(IntPtr messagePtr)
    {
        string message = Marshal.PtrToStringUni(messagePtr) ?? "null";
        Core.ProxyLogger?.LogVerbose(message);
    }

    [UnmanagedCallersOnly(EntryPoint = "XarsuGetIl2CppLibraryName")]
    public static IntPtr GetIl2CppLibraryName()
    {
        if (Core.Bootstrap == null)
            return IntPtr.Zero;
        string name = Core.Bootstrap.Il2CppAssemblyName;
        return Marshal.StringToHGlobalUni(name);
    }
}