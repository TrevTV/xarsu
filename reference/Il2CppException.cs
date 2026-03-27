using System.Runtime.InteropServices;

namespace xarsu.Reference;

public sealed unsafe class Il2CppException(string message) : Exception(message)
{
    public static void ThrowPointer(IntPtr exc)
    {
        const int bufSize = 1024;
        var buf = stackalloc byte[bufSize];
        IL2CPP.il2cpp_format_exception(exc, (IntPtr)buf, bufSize);
        var msg = Marshal.PtrToStringUTF8((IntPtr)buf) ?? "Unknown il2cpp exception";
        throw new Il2CppException(msg);
    }
}