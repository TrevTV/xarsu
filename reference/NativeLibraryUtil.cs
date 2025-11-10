using System.Runtime.InteropServices;

namespace xarsu.Reference;

public static class NativeLibraryUtil
{
    public static T LoadFunction<T>(IntPtr lib, string name, bool throwOnMissing = false) where T : Delegate
    {
        if (!NativeLibrary.TryGetExport(lib, name, out var addr) || addr == IntPtr.Zero)
            if (throwOnMissing)
                throw new EntryPointNotFoundException($"Failed to find export '{name}' in library {lib}");
            else
                return null!;

        return Marshal.GetDelegateForFunctionPointer<T>(addr);
    }
}