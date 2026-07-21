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
    private delegate void scene_changed_func(string? oldScene, string? newScene);
    private delegate void update_func();

    public string Name { get; }
    public IntPtr Handle { get; }
    public bool IsValid => Handle != IntPtr.Zero && _loadFuncPtr != IntPtr.Zero && _il2cppReadyFuncPtr != IntPtr.Zero;
    public IntPtr ExtraData { get; }

    private readonly IntPtr _loadFuncPtr;
    private readonly IntPtr _il2cppReadyFuncPtr;
    private readonly IntPtr _sceneChangedFuncPtr;
    private readonly IntPtr _updateFuncPtr;

    // these will be ran more frequently, so we cache them to avoid repeated Marshal.GetDelegateForFunctionPointer calls
    private readonly scene_changed_func? _sceneChangedFunc;
    private readonly update_func? _updateFunc;

    public Library(string name, IntPtr handle, IntPtr? extraData = null)
    {
        if (handle == IntPtr.Zero)
            throw new ArgumentException("Handle cannot be zero", nameof(handle));

        Name = name;
        Handle = handle;
        ExtraData = extraData ?? IntPtr.Zero;

        NativeLibrary.TryGetExport(handle, "load", out _loadFuncPtr);
        NativeLibrary.TryGetExport(handle, "il2cpp_ready", out _il2cppReadyFuncPtr);
        NativeLibrary.TryGetExport(handle, "scene_changed", out _sceneChangedFuncPtr);
        NativeLibrary.TryGetExport(handle, "update", out _updateFuncPtr);

        if (_sceneChangedFuncPtr != IntPtr.Zero)
            _sceneChangedFunc = Marshal.GetDelegateForFunctionPointer<scene_changed_func>(_sceneChangedFuncPtr);
        if (_updateFuncPtr != IntPtr.Zero)
            _updateFunc = Marshal.GetDelegateForFunctionPointer<update_func>(_updateFuncPtr);
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

    public void InvokeSceneChanged(string? oldScene, string? newScene)
    {
        if (!IsValid)
            throw new InvalidOperationException("Library is not valid");
        _sceneChangedFunc?.Invoke(oldScene, newScene);
    }

    public void InvokeUpdate()
    {
        if (!IsValid)
            throw new InvalidOperationException("Library is not valid");
        _updateFunc?.Invoke();
    }
}