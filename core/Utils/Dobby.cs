using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace xarsu.Utils;

internal static partial class Dobby
{
    [LibraryImport("*", EntryPoint = "DobbyHook")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int HookNative(nint target, nint detour, ref nint original);

    [LibraryImport("*", EntryPoint = "DobbyDestroy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int DestroyNative(nint target);

    public class NativeHook<T>(nint target, T detour) where T : Delegate
    {
        private nint _target = target;
        private nint _detour = Marshal.GetFunctionPointerForDelegate(detour);
        private nint _trampoline;
        private T? _trampolineDelegate;

        public T? Trampoline => _trampolineDelegate;
        public bool IsHooked => _target != 0 && _trampoline != 0;

        public bool Hook()
        {
            if (IsHooked)
                return true;

            int result = HookNative(_target, _detour, ref _trampoline);
            if (_trampoline != 0 && result == 0)
            {
                _trampolineDelegate = Marshal.GetDelegateForFunctionPointer<T>(_trampoline);
                return true;
            }

            return false;
        }

        public bool Unhook()
        {
            if (!IsHooked)
                return true;

            int result = DestroyNative(_target);
            if (result == 0)
            {
                _target = 0;
                _detour = 0;
                _trampoline = 0;
                _trampolineDelegate = null;
                return true;
            }

            return false;
        }
    }
}