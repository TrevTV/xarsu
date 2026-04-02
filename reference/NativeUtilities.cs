using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace xarsu.Reference;

public static unsafe class NativeUtilities
{
    public static T? ReadValueAtIndex<T>(IntPtr ptr, int idx)
    {
        int size = GetSizeOfReference<T>();
        int offset = idx * size;
        return ReadValueAtOffset<T>(ptr, offset);
    }

    public static T? ReadValueAtOffset<T>(IntPtr ptr, int offset)
    {
        if (typeof(T) == typeof(string))
        {
            IntPtr stringPtr = *(IntPtr*)(IntPtr.Add(ptr, offset));
            return (T?)(object?)IL2CPP.Il2CppStringToManaged(stringPtr);
        }

        if (typeof(Il2CppObject).IsAssignableFrom(typeof(T)))
        {
            IntPtr objectPtr = *(IntPtr*)(IntPtr.Add(ptr, offset));
            return (T)(object)Il2CppObject.Wrap(typeof(T), objectPtr)!;
        }

        if (typeof(IIl2CppStruct).IsAssignableFrom(typeof(T)))
        {
            var instance = (IIl2CppStruct)RuntimeHelpers.GetUninitializedObject(typeof(T));
            return (T)instance.ReadFromNative(IntPtr.Add(ptr, offset));
        }

#pragma warning disable CS8500 // should be safe as the other cases should catch anything else that isn't blittable
        return *(T*)(IntPtr.Add(ptr, offset));
#pragma warning restore CS8500
    }

    public static void WriteValueAtIndex<T>(IntPtr ptr, int idx, T value)
    {
        int size = GetSizeOfReference<T>();
        int offset = idx * size;
        WriteValueAtOffset(ptr, offset, value);
    }

    public static void WriteValueAtOffset<T>(IntPtr ptr, int offset, T value)
    {
        if (value is string str)
        {
            IntPtr stringPtr = IL2CPP.ManagedStringToIl2Cpp(str);
            *(IntPtr*)(IntPtr.Add(ptr, offset)) = stringPtr;

        }
        else if (value is Il2CppObject obj)
        {
            *(IntPtr*)(IntPtr.Add(ptr, offset)) = obj.Pointer.Value;
        }
        else if (value is IIl2CppStruct structValue)
        {
            structValue.WriteToNativePointer(IntPtr.Add(ptr, offset));
        }
        else
        {
#pragma warning disable CS8500  // should be safe as the other cases should catch anything else that isn't blittable
            *(T*)IntPtr.Add(ptr, offset) = value;
#pragma warning restore CS8500
        }
    }

    public static int GetSizeOfReference<T>()
    {
        if (typeof(T) == typeof(string) || typeof(Il2CppObject).IsAssignableFrom(typeof(T)) || typeof(IIl2CppStruct).IsAssignableFrom(typeof(T)))
            return sizeof(IntPtr);
        return Marshal.SizeOf<T>();
    }

    public static int GetSizeOf<T>()
    {
        if (typeof(T) == typeof(string) || typeof(Il2CppObject).IsAssignableFrom(typeof(T)))
            return sizeof(IntPtr);
        if (typeof(IIl2CppStruct).IsAssignableFrom(typeof(T)))
            return ((IIl2CppStruct)default(T)!).GetSize();
        return Marshal.SizeOf<T>();
    }

    public static void* CopyToUnmanaged<T>(T value) where T : unmanaged
    {
        void* mem = NativeMemory.Alloc((nuint)sizeof(T));
        *(T*)mem = value;
        return mem;
    }
}