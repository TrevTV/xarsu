using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace xarsu.Reference;

public static unsafe partial class IL2CPP
{
    [GeneratedRegex("\\`\\d+")]
    private static partial Regex GenericMatch();

    public static string? Il2CppStringToManaged(nint il2CppString)
    {
        if (il2CppString == nint.Zero) return null;

        var length = il2cpp_string_length(il2CppString);
        var chars = il2cpp_string_chars(il2CppString);

        return new string(chars, 0, length);
    }

    public static nint ManagedStringToIl2Cpp(string? str)
    {
        if (str == null) return nint.Zero;

        fixed (char* chars = str)
        {
            return il2cpp_string_new_utf16(chars, str.Length);
        }
    }

    public static nint Il2CppObjectToPtr(Il2CppObject obj)
    {
        return obj?.Pointer.Value ?? nint.Zero;
    }

    public static nint Il2CppObjectToPtrNotNull(Il2CppObject obj)
    {
        return obj?.Pointer.Value ?? throw new NullReferenceException();
    }

    public static IntPtr GetIl2CppClass(string assemblyName, string namespaze, string className)
    {
        if (!_imageMap.TryGetValue(assemblyName, out var image))
            throw new KeyNotFoundException($"Assembly '{assemblyName}' not found");
        var klass = il2cpp_class_from_name(image, namespaze, className);
        if (klass == IntPtr.Zero)
            throw new KeyNotFoundException($"Class '{namespaze}.{className}' not found in assembly '{assemblyName}'");
        return klass;
    }

    public static IntPtr GetIl2CppMethod(IntPtr clazz, bool isGeneric, string methodName, string returnTypeName, params string[] argTypes)
    {
        if (clazz == IntPtr.Zero)
            throw new ArgumentNullException(nameof(clazz));

        // TODO: cache methods

        returnTypeName = GenericMatch().Replace(returnTypeName, "").Replace('/', '.').Replace('+', '.');
        for (var index = 0; index < argTypes.Length; index++)
        {
            var argType = argTypes[index];
            argTypes[index] = GenericMatch().Replace(argType, "").Replace('/', '.').Replace('+', '.');
        }

        var methodsSeen = 0;
        var lastMethod = IntPtr.Zero;
        var iter = IntPtr.Zero;
        IntPtr method;
        while ((method = il2cpp_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
        {
            if (il2cpp_method_get_name(method) != methodName)
                continue;

            if (il2cpp_method_get_param_count(method) != argTypes.Length)
                continue;

            if (il2cpp_method_is_generic(method) != isGeneric)
                continue;

            var returnType = il2cpp_method_get_return_type(method);
            var returnTypeNameActual = il2cpp_type_get_name(returnType);
            if (returnTypeNameActual != returnTypeName)
                continue;

            methodsSeen++;
            lastMethod = method;

            var badType = false;
            for (var i = 0; i < argTypes.Length; i++)
            {
                var paramType = il2cpp_method_get_param(method, (uint)i);
                var typeName = il2cpp_type_get_name(paramType);
                if (typeName != argTypes[i])
                {
                    badType = true;
                    break;
                }
            }

            if (badType) continue;

            return method;
        }

        var className = il2cpp_class_get_name(clazz);

        if (methodsSeen == 1)
        {
            TraceLog(
                "Method {0}::{1} was stubbed with a random matching method of the same name", className, methodName);
            TraceLog(
                "Stubby return type/target: {0} / {1}", il2cpp_type_get_name(il2cpp_method_get_return_type(lastMethod)), returnTypeName);
            TraceLog("Stubby parameter types/targets follow:");
            for (var i = 0; i < argTypes.Length; i++)
            {
                var paramType = il2cpp_method_get_param(lastMethod, (uint)i);
                var typeName = il2cpp_type_get_name(paramType);
                TraceLog("    {0} / {1}", typeName, argTypes[i]);
            }

            return lastMethod;
        }

        TraceLog("Unable to find method {0}::{1}; signature follows", className, methodName);
        TraceLog("    return {0}", returnTypeName);
        foreach (var argType in argTypes)
            TraceLog("    {0}", argType);
        TraceLog("Available methods of this name follow:");
        iter = IntPtr.Zero;
        while ((method = il2cpp_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
        {
            if (il2cpp_method_get_name(method) != methodName)
                continue;

            var nParams = il2cpp_method_get_param_count(method);
            TraceLog("Method starts");
            TraceLog(
                "     return {0}", il2cpp_type_get_name(il2cpp_method_get_return_type(method)));
            for (var i = 0; i < nParams; i++)
            {
                var paramType = il2cpp_method_get_param(method, (uint)i);
                var typeName = il2cpp_type_get_name(paramType);
                TraceLog("    {0}", typeName);
            }

            return method;
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// Invokes an il2cpp method, boxes arguments from a managed object[] array, and returns
    /// the result as a boxed object (or null for void). Exceptions from il2cpp are rethrown.
    ///
    /// Signature used by generated method bodies:
    ///   object? IL2CPP.InvokeMethod(IntPtr method, IntPtr instance, object?[] args)
    ///
    /// - method:   IntPtr from GetIl2CppMethod
    /// - instance: Il2CppObject.Pointer for instance methods; IntPtr.Zero for static
    /// - args:     managed object[] — value types must already be boxed by the generated body
    /// </summary>
    public static object? InvokeMethod(IntPtr method, IntPtr instance, object?[] args)
    {
        if (args == null || args.Length == 0)
        {
            IntPtr exc = IntPtr.Zero;
            IntPtr result = il2cpp_runtime_invoke(method, instance, null, ref exc);
            return UnboxResult(method, result);
        }

        var ptrs = new void*[args.Length];
        var handles = new GCHandle[args.Length];

        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string str)
                {
                    // Strings need to be converted to Il2Cpp strings
                    ptrs[i] = (void*)ManagedStringToIl2Cpp(str);
                }
                else if (args[i] is Il2CppObject il2cppObj)
                {
                    // Il2CppObjects need to be boxed into a managed IL2CPP pointer
                    IntPtr boxed = il2cppObj.Box();
                    ptrs[i] = (void*)boxed;
                }
                else
                {
                    // Value types: pin and pass address directly
                    handles[i] = GCHandle.Alloc(args[i], GCHandleType.Pinned);
                    ptrs[i] = (void*)handles[i].AddrOfPinnedObject();
                }
            }

            fixed (void** pArgs = ptrs)
            {
                IntPtr exc = IntPtr.Zero;
                IntPtr result = il2cpp_runtime_invoke(method, instance, pArgs, ref exc);
                return UnboxResult(method, result);
            }
        }
        finally
        {
            for (int i = 0; i < handles.Length; i++)
                if (handles[i].IsAllocated)
                    handles[i].Free();
        }
    }

    // =========================================================================
    // Internal helpers
    // =========================================================================

    /// <summary>
    /// After il2cpp_runtime_invoke returns, unbox the result to a managed object
    /// so that generated bodies can use Unbox_Any / Castclass on the result of InvokeMethod.
    /// </summary>
    public static object? UnboxResult(IntPtr method, IntPtr result)
    {
        if (result == IntPtr.Zero) return null;

        // Determine if return type is a value type.
        var returnType = il2cpp_method_get_return_type(method);
        int typeEnum = il2cpp_type_get_type(returnType);

        // Il2CppTypeEnum value types are in the range 2..13 (BOOLEAN through R8, etc.)
        // Reference types (CLASS, OBJECT, STRING, SZARRAY, etc.) are above that.
        // See: https://github.com/Perfare/Il2CppDumper/blob/master/Il2CppDumper/Il2Cpp/Il2CppClass.cs#L96
        bool isValueType = typeEnum is >= 2 and <= 13;
        bool isString = typeEnum == 14; // special case: strings are reference types but need to be converted back to managed strings

        XarsuExports.Log($"Unboxing result of type enum {typeEnum} (isValueType={isValueType})");

        if (isValueType)
        {
            // il2cpp_runtime_invoke boxes value type results automatically.
            // We need to unbox to get the raw bytes, then re-box as a managed type.
            void* data = il2cpp_object_unbox(result);

            // Map common type enums to managed types.
            return typeEnum switch
            {
                2 => *(bool*)data,
                3 => *(char*)data,
                4 => *(sbyte*)data,
                5 => *(byte*)data,
                6 => *(short*)data,
                7 => *(ushort*)data,
                8 => *(int*)data,
                9 => *(uint*)data,
                10 => *(long*)data,
                11 => *(ulong*)data,
                12 => *(float*)data,
                13 => *(double*)data,
                // VALUETYPE / GENERICINST / etc.: return a raw IntPtr that
                // generated Unbox_Any will handle if the generated return type is a struct.
                _ => (IntPtr)data,
            };
        }

        if (isString)
        {
            return Il2CppStringToManaged(result);
        }

        // Reference type: return the pointer so the generated func can wrap it itself
        return result;
    }

    private static void TraceLog(string message, params object?[] args)
    {
        string formatted = string.Format(message, args);
        XarsuExports.LogVerbose("[IL2CPP] " + formatted);
    }
}