using System.Reflection;

namespace xarsu.Reference;

public class Il2CppHook<TDelegate> where TDelegate : Delegate
{
    private Dobby.NativeHook<TDelegate>? _hook;
    private readonly TDelegate _detour;
    private IntPtr _methodPtr;

    public TDelegate? Original => _hook?.Trampoline;

    public Il2CppHook(TDelegate detour) => _detour = detour;

    public bool Install(IntPtr methodInfo)
    {
        _methodPtr = IL2CPP.GetIl2CppMethodPointer(methodInfo);
        _hook = new Dobby.NativeHook<TDelegate>(_methodPtr, _detour);
        return _hook.Hook();
    }

    public bool Install(MethodInfo? managedMethod)
        => Install(IL2CPP.GetIl2CppMethodByMethodInfo(managedMethod));

    public bool Uninstall() => _hook?.Unhook() ?? false;
}

public static class Il2CppHook
{
    public static Il2CppHook<TDelegate> Install<TDelegate>(IntPtr methodInfo, TDelegate detour)
        where TDelegate : Delegate
    {
        var hook = new Il2CppHook<TDelegate>(detour);
        bool success = hook.Install(methodInfo);
        XarsuExports.Log(success
            ? $"Hooked 0x{methodInfo:X}"
            : $"Failed to hook 0x{methodInfo:X}");
        return hook;
    }

    public static Il2CppHook<TDelegate> Install<TDelegate>(
        string assemblyName, string namespaceName, string className,
        string methodName, string returnTypeName, string[] paramTypeNames,
        TDelegate detour)
        where TDelegate : Delegate
    {
        var klass = IL2CPP.GetIl2CppClass(assemblyName, namespaceName, className);
        var method = IL2CPP.GetIl2CppMethod(klass, false, methodName, returnTypeName, paramTypeNames);
        return Install(method, detour);
    }

    public static Il2CppHook<TDelegate> Install<TDelegate>(MethodInfo? managedMethod, TDelegate detour)
        where TDelegate : Delegate
    {
        var hook = new Il2CppHook<TDelegate>(detour);
        bool success = hook.Install(managedMethod);
        XarsuExports.Log(success
            ? $"Hooked {managedMethod?.Name}"
            : $"Failed to hook {managedMethod?.Name}");
        return hook;
    }
}

public static class HookExtensions
{
    public static T AsIl2Cpp<T>(this IntPtr ptr) where T : Il2CppObject
        => Il2CppObject.Wrap<T>(ptr)!;

    public static T? AsIl2CppOrNull<T>(this IntPtr ptr) where T : Il2CppObject
        => ptr == IntPtr.Zero ? null : Il2CppObject.Wrap<T>(ptr);
}