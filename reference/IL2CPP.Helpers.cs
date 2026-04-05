using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace xarsu.Reference;

public static unsafe partial class IL2CPP
{
    private static readonly ConcurrentDictionary<string, IntPtr> _classCache = new();
    private static readonly ConcurrentDictionary<string, IntPtr> _genericClassCache = new();
    private static readonly ConcurrentDictionary<string, IntPtr> _methodCache = new();
    private static readonly ConcurrentDictionary<string, IntPtr> _fieldCache = new();

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
        var key = $"{assemblyName}|{namespaze}|{className}";
        if (_classCache.TryGetValue(key, out var cached))
            return cached;

        if (!_imageMap.TryGetValue(assemblyName, out var image))
            throw new KeyNotFoundException($"Assembly '{assemblyName}' not found");
        var klass = il2cpp_class_from_name(image, namespaze, className);
        if (klass == IntPtr.Zero)
            throw new KeyNotFoundException($"Class '{namespaze}.{className}' not found in assembly '{assemblyName}'");

        _classCache[key] = klass;
        return klass;
    }

    public static IntPtr GetIl2CppGenericClass(string assemblyName, string namespaze, string className, params Type[] genericArgumentTypes)
    {
        string key = BuildGenericClassKey(assemblyName, namespaze, className, genericArgumentTypes);
        if (_genericClassCache.TryGetValue(key, out IntPtr cached))
            return cached;

        IntPtr clazz = GetIl2CppClass(assemblyName, namespaze, className);

        Debug.Assert(il2cpp_class_is_generic(clazz));

        IntPtr type = ResolveIl2CppType(clazz);

        IntPtr systemTypeClass = GetIl2CppClass("mscorlib.dll", "System", "Type");

        // build the System.Type[] argument array
        IntPtr systemTypeArrayClass = il2cpp_array_class_get(systemTypeClass, 1);
        Il2CppArray typeArray = Il2CppArray.New(systemTypeArrayClass, genericArgumentTypes.Length);
        Debug.Assert(typeArray.Pointer != ObjectPointer.Null);

        for (int i = 0; i < genericArgumentTypes.Length; i++)
            typeArray[i] = ResolveIl2CppType(genericArgumentTypes[i]);

        IntPtr runtimeTypeClass = il2cpp_object_get_class(type);
        IntPtr makeGenericType = GetIl2CppMethod(runtimeTypeClass, false, "MakeGenericType", "Type", ["Type[]"]);
        IntPtr result = InvokeWithArray(makeGenericType, type, typeArray.Pointer.Value);

        // call get_TypeHandle on the returned System.Type object
        IntPtr getTypeHandle = GetIl2CppMethod(runtimeTypeClass, false, "get_TypeHandle", "RuntimeTypeHandle", []);
        IntPtr typeHandleObj = Il2CppInvoke(getTypeHandle, result, null);
        Debug.Assert(typeHandleObj != IntPtr.Zero);

        // RuntimeTypeHandle is a value type, so unbox it to get the struct data
        // it contains a single IntPtr 'value' field at offset 0
        void* typeHandleData = il2cpp_object_unbox(typeHandleObj);
        IntPtr typePtr = *(IntPtr*)typeHandleData;
        Debug.Assert(typePtr != IntPtr.Zero);

        IntPtr resultClass = il2cpp_class_from_type(typePtr);
        Debug.Assert(resultClass != IntPtr.Zero);

        _genericClassCache[key] = resultClass;

        return resultClass;
    }

    private static string BuildGenericClassKey(string assemblyName, string namespaceName, string className, Type[] genericArgs)
    => $"{assemblyName}|{namespaceName}|{className}|{string.Join(",", genericArgs.Select(t => t.FullName))}";

    public static IntPtr GetIl2CppMethod(IntPtr clazz, bool isGeneric, string methodName, string returnTypeName, params string[] argTypes)
    {
        if (clazz == IntPtr.Zero)
            throw new ArgumentNullException(nameof(clazz));

        returnTypeName = GenericMatch().Replace(returnTypeName, "").Replace('/', '.').Replace('+', '.');
        for (var index = 0; index < argTypes.Length; index++)
        {
            var argType = argTypes[index];
            argTypes[index] = GenericMatch().Replace(argType, "").Replace('/', '.').Replace('+', '.');
        }

        var key = $"{clazz}|{isGeneric}|{methodName}|{returnTypeName}|{string.Join(",", argTypes)}";
        if (_methodCache.TryGetValue(key, out var cached))
            return cached;

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

            _methodCache[key] = method; // only exact matches
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

    public static IntPtr GetIl2CppMethodByToken(IntPtr clazz, int token)
    {
        var key = $"{clazz}|{token}";
        if (_methodCache.TryGetValue(key, out var cached))
            return cached;

        var iter = IntPtr.Zero;
        IntPtr method;
        while ((method = il2cpp_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
        {
            if (il2cpp_method_get_token(method) != token)
                continue;

            _methodCache[key] = method;
            return method;
        }

        return IntPtr.Zero;
    }

    public static IntPtr GetIl2CppMethodByMethodInfo(MethodInfo? info)
    {
        if (info == null)
            return IntPtr.Zero;

        IntPtr clazz = GetIl2CppClassFromType(info.DeclaringType!);
        MethodTokenAttribute? tokenAttrib = info.GetCustomAttribute<MethodTokenAttribute>();
        if (tokenAttrib != null)
        {
            IntPtr method = GetIl2CppMethodByToken(clazz, (int)tokenAttrib.Token);
            if (method != IntPtr.Zero)
                return method;
        }

        throw new InvalidOperationException("The provided MethodInfo does not exist in IL2CPP.");
    }

    public static IntPtr GetIl2CppMethodPointer(IntPtr methodInfo) => *(IntPtr*)methodInfo;

    public static IntPtr GetIl2CppMethodPointer(MethodInfo? info) => GetIl2CppMethodPointer(GetIl2CppMethodByMethodInfo(info));

    public static IntPtr GetIl2CppField(IntPtr clazz, string fieldName)
    {
        var key = $"{clazz}|{fieldName}";
        if (_fieldCache.TryGetValue(key, out var cached))
            return cached;

        IntPtr iter = IntPtr.Zero;
        IntPtr field;
        while ((field = il2cpp_class_get_fields(clazz, ref iter)) != IntPtr.Zero)
        {
            if (il2cpp_field_get_name(field) == fieldName)
            {
                _fieldCache[key] = field;
                return field;
            }
        }
        throw new KeyNotFoundException($"Field '{fieldName}' not found in class '{il2cpp_class_get_name(clazz)}'");
    }

    public static IntPtr MakeGenericMethod(IntPtr methodInfo, params Type[] genericParamTypes)
    {
        Debug.Assert(il2cpp_method_is_generic(methodInfo));

        IntPtr systemTypeClass = GetIl2CppClass("mscorlib.dll", "System", "Type");

        // build the System.Type[] argument array
        IntPtr systemTypeArrayClass = il2cpp_array_class_get(systemTypeClass, 1);
        Il2CppArray typeArray = Il2CppArray.New(systemTypeArrayClass, genericParamTypes.Length);
        Debug.Assert(typeArray.Pointer != ObjectPointer.Null);

        for (int i = 0; i < genericParamTypes.Length; i++)
            typeArray[i] = ResolveIl2CppType(genericParamTypes[i]);

        // get MakeGenericMethod
        IntPtr methodClass = il2cpp_method_get_class(methodInfo);
        IntPtr methodObj = il2cpp_method_get_object(methodInfo, methodClass);
        IntPtr methodObjClass = il2cpp_object_get_class(methodObj);
        IntPtr makeGenericMethod = GetIl2CppMethod(methodObjClass, false, "MakeGenericMethod", "MethodInfo", ["Type[]"]);

        // invoke RuntimeMethodInfo.MakeGenericMethod(typeArray)
        IntPtr result = InvokeWithArray(makeGenericMethod, methodObj, typeArray.Pointer.Value);

        return result == IntPtr.Zero ? IntPtr.Zero : il2cpp_method_get_from_reflection(result);
    }

    /// <summary>Resolves a managed Type to its IL2CPP System.Type object</summary>
    public static IntPtr ResolveIl2CppType(Type type)
    {
        IntPtr internalFromHandle = GetIl2CppMethod(GetIl2CppClass("mscorlib.dll", "System", "Type"), false, "internal_from_handle", "System.Type", ["System.IntPtr"]);

        IntPtr il2cppClass = GetIl2CppClassFromType(type);

        IntPtr typeHandle = il2cpp_class_get_type(il2cppClass);
        void* typeHandlePtr = &typeHandle;
        IntPtr systemType = Il2CppInvoke(internalFromHandle, IntPtr.Zero, &typeHandlePtr);
        return systemType;
    }

    /// <summary>Resolves an IL2CPP class pointer to its IL2CPP System.Type object</summary>
    public static IntPtr ResolveIl2CppType(IntPtr clazz)
    {
        IntPtr internalFromHandle = GetIl2CppMethod(GetIl2CppClass("mscorlib.dll", "System", "Type"), false, "internal_from_handle", "System.Type", ["System.IntPtr"]);

        IntPtr typeHandle = il2cpp_class_get_type(clazz);
        void* typeHandlePtr = &typeHandle;
        IntPtr systemType = Il2CppInvoke(internalFromHandle, IntPtr.Zero, &typeHandlePtr);
        return systemType;
    }

    public static IntPtr GetIl2CppClassFromType(Type type)
    {
        string assemblyName = type.Assembly!.GetName().Name!;
        if (assemblyName == "System.Private.CoreLib")
            assemblyName = "mscorlib"; // precaution for corlib types as il2cpp has them under mscorlib.dll

        OriginalTypeNameAttribute? attr = type.GetCustomAttribute<OriginalTypeNameAttribute>();
        return attr != null
            ? GetIl2CppClass($"{attr.AssemblyName}.dll", attr.Namespace, attr.Name)
            : GetIl2CppClass($"{assemblyName}.dll", type.Namespace!, type.Name);
    }

    /// <summary>
    /// Invokes an il2cpp method, boxes arguments from a managed object[] array, and returns
    /// the result as a boxed object (or null for void). Exceptions from il2cpp are rethrown.
    /// </summary>
    public static T? InvokeMethod<T>(IntPtr method, IntPtr instance, object?[] args)
    {
        var returnType = il2cpp_method_get_return_type(method);
        return UnboxResult<T>(returnType, InvokeMethodInternal(method, instance, args));
    }

    public static void InvokeVoidMethod(IntPtr method, IntPtr instance, object?[] args)
    {
        var returnType = il2cpp_method_get_return_type(method);
        if (il2cpp_type_get_type(returnType) != 1) // not void
            throw new InvalidOperationException("Return type must be void for InvokeVoidMethod");
        InvokeMethodInternal(method, instance, args);
    }

    public static T? ReadField<T>(IntPtr fieldPtr, IntPtr instance)
    {
        IntPtr type = il2cpp_field_get_type(fieldPtr);
        IntPtr fieldValue = il2cpp_field_get_value_object(fieldPtr, instance);
        return UnboxResult<T>(type, fieldValue);
    }

    public static void WriteField(IntPtr fieldPtr, IntPtr instance, object? value)
    {
        void* rawValue = value switch
        {
            null => null,
            string str => (void*)ManagedStringToIl2Cpp(str),
            Il2CppObject il2cppObj => (void*)il2cppObj.Pointer,
            bool v => NativeUtilities.CopyToUnmanaged(v),
            byte v => NativeUtilities.CopyToUnmanaged(v),
            sbyte v => NativeUtilities.CopyToUnmanaged(v),
            short v => NativeUtilities.CopyToUnmanaged(v),
            ushort v => NativeUtilities.CopyToUnmanaged(v),
            int v => NativeUtilities.CopyToUnmanaged(v),
            uint v => NativeUtilities.CopyToUnmanaged(v),
            long v => NativeUtilities.CopyToUnmanaged(v),
            ulong v => NativeUtilities.CopyToUnmanaged(v),
            float v => NativeUtilities.CopyToUnmanaged(v),
            double v => NativeUtilities.CopyToUnmanaged(v),
            char v => NativeUtilities.CopyToUnmanaged(v),
            _ => null,
        };

        int flags = il2cpp_field_get_flags(fieldPtr);
        bool isStatic = (flags & 0x10) != 0;

        if (isStatic)
            il2cpp_field_static_set_value(fieldPtr, rawValue);
        else
            il2cpp_field_set_value(instance, fieldPtr, rawValue);
    }

    public static void ReadStructToRef<T>(IntPtr ptr, ref T instance) where T : unmanaged, IIl2CppStruct<T>
    {
        T.ReadTo(ptr, ref instance);
    }

    // =========================================================================
    // Internal helpers
    // =========================================================================

    private static IntPtr InvokeMethodInternal(IntPtr method, IntPtr instance, object?[] args)
    {
        var returnType = il2cpp_method_get_return_type(method);
        if (args == null || args.Length == 0)
            return Il2CppInvoke(method, instance, null);

        var ptrs = new void*[args.Length];
        var handles = new GCHandle[args.Length];
        try
        {
            for (int i = 0; i < args.Length; i++)
                ptrs[i] = MarshalMethodArgument(args[i], ref handles[i]);
            fixed (void** pArgs = ptrs)
                return Il2CppInvoke(method, instance, pArgs);
        }
        finally
        {
            foreach (GCHandle h in handles)
                if (h.IsAllocated) h.Free();
        }
    }

    /// <summary>Invokes an IL2CPP method with a single IL2CPP array item</summary>
    private static IntPtr InvokeWithArray(IntPtr method, IntPtr instance, IntPtr arrayItem)
    {
        void* arg = (void*)arrayItem;
        void** pArgs = &arg;
        return Il2CppInvoke(method, instance, pArgs);
    }

    /// <summary>il2cpp_runtime_invoke wrapper with exception handling</summary>
    private static IntPtr Il2CppInvoke(IntPtr method, IntPtr instance, void** args)
    {
        IntPtr exc = IntPtr.Zero;
        IntPtr result = il2cpp_runtime_invoke(method, instance, args, ref exc);

        if (exc != IntPtr.Zero)
        {
            byte[] buf = new byte[1024];
            fixed (byte* pBuf = buf)
            {
                // TODO: make an actual Il2CppException and throw it
                il2cpp_format_exception(exc, (IntPtr)pBuf, buf.Length);
                XarsuExports.Log($"Exception in {il2cpp_method_get_name(method)}: {Marshal.PtrToStringAnsi((IntPtr)pBuf)}");
            }
            return IntPtr.Zero;
        }

        return result;
    }

    /// <summary>Marshals a single managed argument to a pointer</summary>
    private static void* MarshalMethodArgument(object? arg, ref GCHandle handle)
    {
        switch (arg)
        {
            case string str:
                return (void*)ManagedStringToIl2Cpp(str);

            case Il2CppObject il2cppObj:
                return (void*)il2cppObj.Pointer;

            case IIl2CppStruct il2cppStruct:
                return (void*)il2cppStruct.WriteToNative();

            default:
                handle = GCHandle.Alloc(arg, GCHandleType.Pinned);
                return (void*)handle.AddrOfPinnedObject();
        }
    }

    /// <summary>
    /// Unbox the given pointer to a managed object
    /// </summary>
    private static T? UnboxResult<T>(IntPtr returnType, IntPtr result)
    {
        if (result == IntPtr.Zero) return default;

        int typeEnum = il2cpp_type_get_type(returnType);

        // Il2CppTypeEnum value types are in the range 2..13 (BOOLEAN through R8, etc.)
        // Reference types (CLASS, OBJECT, STRING, SZARRAY, etc.) are above that.
        // See: https://github.com/Perfare/Il2CppDumper/blob/master/Il2CppDumper/Il2Cpp/Il2CppClass.cs#L96
        bool isValueType = typeEnum is >= 2 and <= 13;
        bool isString = typeEnum == 14; // special case: strings are reference types but need to be converted back to managed strings

        if (typeEnum == 0x11) // struct
            return UnboxStruct<T>(result);
        if (isValueType)
            return UnboxValueTypeUnsafe<T>(result);
        if (isString)
            return (T?)(object?)Il2CppStringToManaged(result);
        if (typeof(Il2CppObject).IsAssignableFrom(typeof(T)))
            return (T?)(object?)Il2CppObject.Wrap(typeof(T), result);
        return default;
    }

    private static T? UnboxStruct<T>(IntPtr ptr)
    {
        if (typeof(IIl2CppStruct).IsAssignableFrom(typeof(T)))
        {
            IntPtr dataPtr = new(il2cpp_object_unbox(ptr));
            return (T)((IIl2CppStruct)default(T)!).ReadFromNative(dataPtr);
        }
        return default;
    }

    private static T? UnboxValueTypeUnsafe<T>(IntPtr result)
    {
        void* data = il2cpp_object_unbox(result);
        return Unsafe.Read<T>(data);
    }

    private static void TraceLog(string message, params object?[] args)
    {
        string formatted = string.Format(message, args);
        XarsuExports.LogVerbose("[IL2CPP] " + formatted);
    }
}