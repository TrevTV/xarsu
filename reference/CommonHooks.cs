using System.Runtime.InteropServices;

namespace xarsu.Reference;

public static class CommonHooks
{
    /// <summary>
    /// Prevents the original method from executing by doing nothing in the detour. Useful for void returning methods.
    /// </summary>
    public static readonly Action NoOp = () => { };

    /// <summary>
    /// Forces the original method to return true.
    /// </summary>
    public static readonly BoolInstanceDelegate ForceTrue = () => true;

    /// <summary>
    /// Forces the original method to return false.
    /// </summary>
    public static readonly BoolInstanceDelegate ForceFalse = () => false;

    /// <summary>
    /// Provides a delegate that returns a large integer value (999999). Useful for overriding currency getters.
    /// </summary>
    public static readonly IntInstanceDelegate ForceLargeInteger = () => 999999;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool BoolInstanceDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int IntInstanceDelegate();
}