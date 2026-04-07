using System.Runtime.InteropServices;

namespace xarsu;

internal class Library
{
    public static bool TryLoad(string path, out Library? library, IntPtr? extraData = null)
    {
        if (Core.Bootstrap == null)
            throw new InvalidOperationException("Bootstrap is not initialized");

        if (!Core.Bootstrap!.TryLoadRawLibrary(path, out IntPtr handle))
        {
            library = null;
            return false;
        }

        var lib = new Library(Path.GetFileName(path), handle, extraData);
        if (!lib.IsValid)
        {
            NativeLibrary.Free(handle);
            library = null;
            return false;
        }

        library = lib;
        return true;
    }

    private delegate void load_func(IntPtr extraData);
    private delegate void il2cpp_ready_func();

    public string Name { get; }
    public IntPtr Handle { get; }
    public bool IsValid => Handle != IntPtr.Zero && _loadFuncPtr != IntPtr.Zero && _il2cppReadyFuncPtr != IntPtr.Zero;
    public IntPtr ExtraData { get; }

    private readonly IntPtr _loadFuncPtr;
    private readonly IntPtr _il2cppReadyFuncPtr;

    public Library(string name, IntPtr handle, IntPtr? extraData = null)
    {
        if (handle == IntPtr.Zero)
            throw new ArgumentException("Handle cannot be zero", nameof(handle));

        Name = name;
        Handle = handle;
        ExtraData = extraData ?? IntPtr.Zero;

        NativeLibrary.TryGetExport(handle, "load", out _loadFuncPtr);
        NativeLibrary.TryGetExport(handle, "il2cpp_ready", out _il2cppReadyFuncPtr);
    }

    ~Library()
    {
        if (Handle != IntPtr.Zero)
        {
            NativeLibrary.Free(Handle);
        }
    }

    public void InvokeLoad()
    {
        if (!IsValid)
            throw new InvalidOperationException("Library is not valid");
        var loadFunc = Marshal.GetDelegateForFunctionPointer<load_func>(_loadFuncPtr);
        loadFunc(ExtraData);
    }

    public void InvokeIl2CppReady()
    {
        if (!IsValid)
            throw new InvalidOperationException("Library is not valid");
        var il2cppReadyFunc = Marshal.GetDelegateForFunctionPointer<il2cpp_ready_func>(_il2cppReadyFuncPtr);
        il2cppReadyFunc();
    }
}