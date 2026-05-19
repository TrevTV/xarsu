using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace xarsu.Reference;

public class Il2CppObject
{
    public ObjectPointer Pointer
    {
        get
        {
            if (_gcHandle == IntPtr.Zero)
                throw new InvalidOperationException("GC handle is not initialized.");
            var handleTarget = IL2CPP.il2cpp_gchandle_get_target(_gcHandle);
            if (handleTarget == IntPtr.Zero)
                throw new InvalidOperationException("GC handle target is null, object may have been collected.");
            return new ObjectPointer(handleTarget);
        }
    }

    public bool WasCollected
    {
        get
        {
            var handleTarget = IL2CPP.il2cpp_gchandle_get_target(_gcHandle);
            if (handleTarget == IntPtr.Zero) return true;
            return false;
        }
    }

    private IntPtr _gcHandle;

    public Il2CppObject(ObjectPointer ptr)
    {
        if (ptr == ObjectPointer.Null)
            throw new ArgumentNullException(nameof(ptr), "Il2CppObject pointer must not be null.");
        CreateGCHandle(ptr.Value);
    }

    public void InitializeIl2Cpp(IntPtr ptr)
    {
        CreateGCHandle(ptr);
    }

    ~Il2CppObject()
    {
        IL2CPP.il2cpp_gchandle_free(_gcHandle);
    }

    private void CreateGCHandle(IntPtr pointer)
    {
        if (Pointer == ObjectPointer.Null)
            throw new InvalidOperationException("Cannot create GC handle for null pointer.");

        if (_gcHandle != IntPtr.Zero)
            return;

        _gcHandle = IL2CPP.il2cpp_gchandle_new(pointer, false);
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

    public IntPtr Box()
    {
        var klass = IL2CPP.il2cpp_object_get_class(Pointer.Value);
        return IL2CPP.il2cpp_value_box(klass, Pointer.Value);
    }

    public static IntPtr Box<T>(string assemblyName, string namespaceName, string className, T value)
        where T : unmanaged
    {
        var klass = IL2CPP.GetIl2CppClass(assemblyName, namespaceName, className);
        unsafe
        {
            fixed (void* p = &Unsafe.As<T, byte>(ref value))
                return IL2CPP.il2cpp_value_box(klass, (IntPtr)p);
        }
    }

    public static T? Wrap<T>(IntPtr ptr) where T : Il2CppObject
    {
        if (ptr == IntPtr.Zero)
            return null;

        var obj = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        obj.InitializeIl2Cpp(ptr);
        return obj;
    }

    public static Il2CppObject? Wrap(Type type, IntPtr ptr)
    {
        if (!typeof(Il2CppObject).IsAssignableFrom(type))
            throw new ArgumentException($"Type {type.FullName} must derive from Il2CppObject.", nameof(type));

        if (ptr == IntPtr.Zero)
            return null;

        var obj = RuntimeHelpers.GetUninitializedObject(type) as Il2CppObject;
        obj?.InitializeIl2Cpp(ptr);
        return obj;
    }

    public override string ToString()
        => $"[Il2CppObject 0x{Pointer:X}]";

    public override bool Equals(object? obj)
        => obj is Il2CppObject other && other.Pointer == Pointer;

    public static bool operator ==(Il2CppObject? left, Il2CppObject? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Pointer == right.Pointer);

    public static bool operator !=(Il2CppObject? left, Il2CppObject? right)
        => !(left == right);

    public override int GetHashCode()
        => Pointer.GetHashCode();
}
