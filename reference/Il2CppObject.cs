namespace xarsu.Reference;

public class Il2CppObject
{
    public ObjectPointer Pointer { get; }

    public Il2CppObject(ObjectPointer ptr)
    {
        if (ptr == ObjectPointer.Null)
            throw new ArgumentNullException(nameof(ptr), "Il2CppObject pointer must not be null.");
        Pointer = ptr;
    }

    protected static IntPtr AllocObject(string assemblyName, string namespaceName, string className)
    {
        var klass = IL2CPP.GetIl2CppClass(assemblyName, namespaceName, className);
        var ptr = IL2CPP.il2cpp_object_new(klass);
        if (ptr == IntPtr.Zero)
            throw new InvalidOperationException(
                $"il2cpp_object_new returned null for {namespaceName}.{className}");
        return ptr;
    }

    public static IntPtr Box<T>(string assemblyName, string namespaceName, string className, T value)
        where T : unmanaged
    {
        var klass = IL2CPP.GetIl2CppClass(assemblyName, namespaceName, className);
        unsafe
        {
            fixed (void* p = &System.Runtime.CompilerServices.Unsafe.As<T, byte>(ref value))
                return IL2CPP.il2cpp_value_box(klass, (IntPtr)p);
        }
    }

    public override string ToString()
        => $"[Il2CppObject 0x{Pointer:X}]";

    public override bool Equals(object? obj)
        => obj is Il2CppObject other && other.Pointer == Pointer;

    public override int GetHashCode()
        => Pointer.GetHashCode();
}
