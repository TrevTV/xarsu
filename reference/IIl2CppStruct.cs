using System.Runtime.InteropServices;

namespace xarsu.Reference;

public interface IIl2CppStruct
{
    int GetSize();
    IntPtr WriteToNative();
    IIl2CppStruct ReadFromNative(IntPtr ptr);
}

public interface IIl2CppStruct<T> : IIl2CppStruct where T : unmanaged, IIl2CppStruct<T>
{
    static abstract int Size { get; }
    static abstract T Read(IntPtr ptr);
    static abstract void Write(T instance, IntPtr ptr);

    int IIl2CppStruct.GetSize() => T.Size;

    IntPtr IIl2CppStruct.WriteToNative()
    {
        var ptr = Marshal.AllocHGlobal(T.Size);
        T.Write((T)(object)this, ptr);
        return ptr;
    }

    IIl2CppStruct IIl2CppStruct.ReadFromNative(nint ptr)
    {
        return T.Read(ptr);
    }
}