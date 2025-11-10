using System.Runtime.InteropServices;

namespace xarsu.Reference;

public static class XarsuExports
{
    private static readonly IntPtr _handle;
    private static readonly Delegates _exports;

    private static readonly string[] _possibleLibraries = [
        "libmain.so"
    ];

    static XarsuExports()
    {
        // TODO: kinda jank
        foreach (var libraryName in _possibleLibraries)
            if (IsXarsu(libraryName, out _handle))
                break;

        if (_handle == IntPtr.Zero)
            throw new DllNotFoundException("Failed to find the xarsu library.");

        _exports = new Delegates(_handle);
    }

    private static bool IsXarsu(string libraryName, out IntPtr handle)
    {
        handle = IntPtr.Zero;

        if (NativeLibrary.TryLoad(libraryName, out IntPtr tempHandle))
        {
            if (NativeLibrary.TryGetExport(tempHandle, "XarsuGetIl2CppLibraryName", out var addr) && addr != IntPtr.Zero)
            {
                handle = tempHandle;
                return true;
            }
        }

        return false;
    }

    public static void Log(string message) => _exports.Log(message);
    public static void LogWarning(string message) => _exports.LogWarning(message);
    public static void LogError(string message) => _exports.LogError(message);
    public static void LogVerbose(string message) => _exports.LogVerbose(message);

    public static string GetIl2CppLibraryName() => _exports.GetIl2CppLibraryName();

    private class Delegates
    {
        public Delegates(IntPtr lib)
        {
            Log = NativeLibraryUtil.LoadFunction<LogDelegate>(lib, "XarsuLog", true);
            LogWarning = NativeLibraryUtil.LoadFunction<LogWarningDelegate>(lib, "XarsuLogWarning", true);
            LogError = NativeLibraryUtil.LoadFunction<LogErrorDelegate>(lib, "XarsuLogError", true);
            LogVerbose = NativeLibraryUtil.LoadFunction<LogVerboseDelegate>(lib, "XarsuLogVerbose", true);
            GetIl2CppLibraryName = NativeLibraryUtil.LoadFunction<GetIl2CppLibraryNameDelegate>(lib, "XarsuGetIl2CppLibraryName", true);
        }

        #region Delegate Definitions

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void LogDelegate([MarshalAs(UnmanagedType.LPWStr)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void LogWarningDelegate([MarshalAs(UnmanagedType.LPWStr)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void LogErrorDelegate([MarshalAs(UnmanagedType.LPWStr)] string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void LogVerboseDelegate([MarshalAs(UnmanagedType.LPWStr)] string message);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate string GetIl2CppLibraryNameDelegate();

        #endregion

        #region Delegate Instances

        public LogDelegate Log { get; }
        public LogWarningDelegate LogWarning { get; }
        public LogErrorDelegate LogError { get; }
        public LogVerboseDelegate LogVerbose { get; }
        public GetIl2CppLibraryNameDelegate GetIl2CppLibraryName { get; }

        #endregion
    }
}