using System.Runtime.InteropServices;

namespace xarsu.Reference;

public static unsafe class NativeUtilities
{
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

        if (typeof(T).IsAssignableFrom(typeof(IIl2CppStruct)))
        {
            // TODO: test, not sure if this will work as intended
            IntPtr structPtr = *(IntPtr*)(IntPtr.Add(ptr, offset));
            return (T)((IIl2CppStruct)default(T)!).ReadFromNative(structPtr);
        }

#pragma warning disable CS8500 // should be safe as the other cases should catch anything else that isn't blittable
        return *(T*)(IntPtr.Add(ptr, offset));
#pragma warning restore CS8500
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
            IntPtr structPtr = structValue.WriteToNative();
            *(IntPtr*)(IntPtr.Add(ptr, offset)) = structPtr;
        }
        else
        {
#pragma warning disable CS8500  // should be safe as the other cases should catch anything else that isn't blittable
            *(T*)IntPtr.Add(ptr, offset) = value;
#pragma warning restore CS8500
        }
    }

    public static void* CopyToUnmanaged<T>(T value) where T : unmanaged
    {
        void* mem = NativeMemory.Alloc((nuint)sizeof(T));
        *(T*)mem = value;
        return mem;
    }
}