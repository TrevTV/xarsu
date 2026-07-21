using System.Runtime.InteropServices;
using xarsu.Reference;

namespace xarsu.Hooks;

internal static unsafe class InvokeHook
{
    private delegate IntPtr il2cpp_runtime_invoke_func(nint method, nint obj, void** args, nint exc);
    private delegate nint SceneGetNameInternal(int sceneHandle);

    private static Dobby.NativeHook<il2cpp_runtime_invoke_func>? _il2cppInvokeHook;
    private static SceneGetNameInternal? _sceneGetNameInternal;

    public static void DoHook()
    {
        string libraryPath = Core.Bootstrap!.Il2CppAssemblyName;
        if (!NativeLibrary.TryLoad(libraryPath, out var il2cpp))
        {
            Core.ProxyLogger?.LogError($"Failed to load {libraryPath}");
            return;
        }

        // get utility method first
        IntPtr getNameInternalMethod = IL2CPP.il2cpp_resolve_icall("UnityEngine.SceneManagement.Scene::GetNameInternal");
        if (getNameInternalMethod == IntPtr.Zero)
            Core.ProxyLogger?.LogError("Failed to resolve icall for Scene::GetNameInternal");
        else
            _sceneGetNameInternal = Marshal.GetDelegateForFunctionPointer<SceneGetNameInternal>(getNameInternalMethod);

        // now find the target
        if (!NativeLibrary.TryGetExport(il2cpp, "il2cpp_runtime_invoke", out var il2cppInvokePtr))
        {
            Core.ProxyLogger?.LogError("Failed to find il2cpp_runtime_invoke export");
            return;
        }

#if ANDROID
        il2cppInvokePtr = ResolveInnerBranch(il2cppInvokePtr, 2);
#endif

        _il2cppInvokeHook = new Dobby.NativeHook<il2cpp_runtime_invoke_func>(il2cppInvokePtr, Il2CppInvokeDetour);
        if (_il2cppInvokeHook.Hook())
        {
            Core.ProxyLogger?.Log("Successfully hooked il2cpp_runtime_invoke");
        }
        else
        {
            Core.ProxyLogger?.LogError("Failed to hook il2cpp_runtime_invoke");
        }
    }

    private static IntPtr Il2CppInvokeDetour(nint method, nint obj, void** args, nint exc)
    {
        IntPtr result = _il2cppInvokeHook!.Trampoline!.Invoke(method, obj, args, exc);

        string? methodName = IL2CPP.il2cpp_method_get_name(method);
        if (methodName == null) return result;

        IntPtr type = IL2CPP.il2cpp_method_get_declaring_type(method);
        string? typeName = IL2CPP.il2cpp_class_get_name(type);
        if (typeName == null) return result;

        if (methodName == "Internal_ActiveSceneChanged")
        {
            if (_sceneGetNameInternal == null)
                return result;

            // args is 2 Scene structs (just a handle), first is old scene, second is new scene
            int oldSceneHandle = *(int*)args[0];
            int newSceneHandle = *(int*)args[1];

            nint oldSceneNameRaw = _sceneGetNameInternal(oldSceneHandle);
            nint newSceneNameRaw = _sceneGetNameInternal(newSceneHandle);   

            string? oldSceneName = IL2CPP.Il2CppStringToManaged(oldSceneNameRaw);
            string? newSceneName = IL2CPP.Il2CppStringToManaged(newSceneNameRaw);

            if ((oldSceneName == null && newSceneName == null) || newSceneName?.Length == 0)
                return result; // likely an early init call, not important

            Core.NotifySceneChanged(oldSceneName, newSceneName);
        }

        // HACK: very specific, but its a solid bet for most games and is known to only run once per frame
        if (methodName == "Update" && typeName == "EventSystem")
            Core.NotifyUpdate();

        return result;
    }

    private static unsafe nint ResolveInnerBranch(nint funcAddr, int maxDepth)
    {
        if (funcAddr == 0 || maxDepth <= 0) return funcAddr;

        nuint addr = (nuint)funcAddr;
        uint instruction = *(uint*)addr;

        // Check for unconditional B (0b000101)
        if ((instruction >> 26) == 0b000101)
        {
            uint imm26U = instruction & 0x03FFFFFF;
            long imm26 = (int)(imm26U << 6) >> 6; // sign-extend 26-bit
            nuint targetAddr = addr + (nuint)(imm26 << 2);

            // Recursively resolve the target address
            return ResolveInnerBranch((nint)targetAddr, maxDepth - 1);
        }

        // return the original address if no branch instruction is found
        return funcAddr;
    }
}