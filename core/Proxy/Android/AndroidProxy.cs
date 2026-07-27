#if ANDROID
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using xarsu.Reference.Java;

namespace xarsu.Proxy.Android;

internal static class AndroidProxy
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint JNI_OnLoadFunc(IntPtr vm, IntPtr reserved);

    [UnmanagedCallersOnly(EntryPoint = "JNI_OnLoad")]
    public static unsafe JNI.Version JNI_OnLoad(IntPtr vm, void* reserved)
    {
        JNI.Initialize(vm);
        JClass nativeLoader = JNI.FindClass("com/unity3d/player/NativeLoader");
        if (!nativeLoader.Valid())
        {
            AndroidLogger.LogInternal("Cannot find NativeLoader class", AndroidLogger.LogPriority.ERROR);
            return JNI.Version.V1_6;
        }

        var methods = (JNINativeMethod*)NativeMemory.Alloc((nuint)(sizeof(JNINativeMethod) * 2));

        methods[0] = new JNINativeMethod { Name = Utf8StringMarshaller.ConvertToUnmanaged("load"), Signature = Utf8StringMarshaller.ConvertToUnmanaged("(Ljava/lang/String;)Z"), FnPtr = (delegate* unmanaged[Cdecl]<void*, void*, void*, byte>)&Load };
        methods[1] = new JNINativeMethod { Name = Utf8StringMarshaller.ConvertToUnmanaged("unload"), Signature = Utf8StringMarshaller.ConvertToUnmanaged("()Z"), FnPtr = (delegate* unmanaged[Cdecl]<void*, void*, byte>)&Unload };

        var registerNatives = JNI.Env->Functions->RegisterNatives(JNI.Env, nativeLoader.Handle, (IntPtr)methods, 2);
        if (registerNatives != 0)
        {
            AndroidLogger.LogInternal("Failed to register native methods", AndroidLogger.LogPriority.ERROR);
        }
        return JNI.Version.V1_6;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe byte Load(void* env, void* jobject, void* str)
    {
        LoadUnity();
        AndroidBootstrap.TryInitCore();
        return 1;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe byte Unload(void* vm, void* reserved)
    {
        Core.ProxyLogger?.Log("Unload called");
        return 1;
    }

    public static unsafe void LoadUnity()
    {
        if (!NativeLibrary.TryLoad("libunity.so", out var libUnity))
        {
            AndroidLogger.LogInternal("Failed to load libunity.so", AndroidLogger.LogPriority.ERROR);
            return;
        }

        if (!NativeLibrary.TryGetExport(libUnity, "JNI_OnLoad", out var jniOnLoadPtr))
        {
            AndroidLogger.LogInternal("Can't load export via JNI_OnLoad", AndroidLogger.LogPriority.ERROR);
            return;
        }

        var jniOnLoad = Marshal.GetDelegateForFunctionPointer<JNI_OnLoadFunc>(jniOnLoadPtr);
        jniOnLoad((IntPtr)JNI.VM, IntPtr.Zero);
    }

    private unsafe struct JNINativeMethod
    {
        public byte* Name;
        public byte* Signature;
        public void* FnPtr;
    }
}
#endif