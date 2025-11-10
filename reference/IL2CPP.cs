using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace xarsu.Reference;

#pragma warning disable IDE1006 // Naming Styles: uses the original export names
public static unsafe partial class IL2CPP
{
    private static readonly Dictionary<string, IntPtr> _imageMap = [];
    private static readonly IntPtr _handle;
    private static readonly Delegates _exports;

    [GeneratedRegex("\\`\\d+")]
    private static partial Regex GenericMatch();

    static IL2CPP()
    {
        string library = XarsuExports.GetIl2CppLibraryName();
        if (!NativeLibrary.TryLoad(library, out _handle))
            throw new DllNotFoundException($"Unable to load {library}");

        _exports = new Delegates(_handle);

        var domain = il2cpp_domain_get();
        if (domain == IntPtr.Zero)
            throw new Exception("il2cpp_get_domain returned null");

        uint assemblyCount = 0;
        var assemblies = il2cpp_domain_get_assemblies(domain, ref assemblyCount);
        for (uint i = 0; i < assemblyCount; i++)
        {
            var assembly = assemblies[i];
            var image = il2cpp_assembly_get_image(assembly);
            var name = il2cpp_image_get_name(image);
            if (name == null)
                continue;
            _imageMap[name] = image;
        }
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
                "Method {ClassName}::{MethodName} was stubbed with a random matching method of the same name", className, methodName);
            TraceLog(
                "Stubby return type/target: {LastMethod} / {ReturnTypeName}", il2cpp_type_get_name(il2cpp_method_get_return_type(lastMethod)), returnTypeName);
            TraceLog("Stubby parameter types/targets follow:");
            for (var i = 0; i < argTypes.Length; i++)
            {
                var paramType = il2cpp_method_get_param(lastMethod, (uint)i);
                var typeName = il2cpp_type_get_name(paramType);
                TraceLog("    {TypeName} / {ArgType}", typeName, argTypes[i]);
            }

            return lastMethod;
        }

        TraceLog("Unable to find method {ClassName}::{MethodName}; signature follows", className, methodName);
        TraceLog("    return {ReturnTypeName}", returnTypeName);
        foreach (var argType in argTypes)
            TraceLog("    {ArgType}", argType);
        TraceLog("Available methods of this name follow:");
        iter = IntPtr.Zero;
        while ((method = il2cpp_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
        {
            if (il2cpp_method_get_name(method) != methodName)
                continue;

            var nParams = il2cpp_method_get_param_count(method);
            TraceLog("Method starts");
            TraceLog(
                "     return {MethodTypeName}", il2cpp_type_get_name(il2cpp_method_get_return_type(method)));
            for (var i = 0; i < nParams; i++)
            {
                var paramType = il2cpp_method_get_param(method, (uint)i);
                var typeName = il2cpp_type_get_name(paramType);
                TraceLog("    {TypeName}", typeName);
            }

            return method;
        }

        return IntPtr.Zero;
    }

    private static void TraceLog(string message, params object?[] args)
    {
        string formatted = string.Format(message, args);
        XarsuExports.LogVerbose("[IL2CPP] " + formatted);
    }

    public static void il2cpp_init_utf16(IntPtr domain_name) => _exports.il2cpp_init_utf16(domain_name);

    public static void il2cpp_set_config_dir(IntPtr config_path) => _exports.il2cpp_set_config_dir(config_path);

    public static void il2cpp_set_temp_dir(IntPtr temp_path) => _exports.il2cpp_set_temp_dir(temp_path);

    public static void il2cpp_set_commandline_arguments_utf16(int argc, IntPtr argv, IntPtr basedir) => _exports.il2cpp_set_commandline_arguments_utf16(argc, argv, basedir);

    public static void il2cpp_set_config(IntPtr executablePath) => _exports.il2cpp_set_config(executablePath);

    public static IntPtr il2cpp_get_corlib() => _exports.il2cpp_get_corlib();

    public static IntPtr il2cpp_resolve_icall([MarshalAs(UnmanagedType.LPStr)] string name) => _exports.il2cpp_resolve_icall(name);

    public static void il2cpp_free(IntPtr ptr) => _exports.il2cpp_free(ptr);

    public static uint il2cpp_array_length(IntPtr array) => _exports.il2cpp_array_length(array);

    public static IntPtr il2cpp_array_new(IntPtr elementTypeInfo, ulong length) => _exports.il2cpp_array_new(elementTypeInfo, length);

    public static IntPtr il2cpp_array_new_full(IntPtr array_class, ref ulong lengths, ref ulong lower_bounds) => _exports.il2cpp_array_new_full(array_class, ref lengths, ref lower_bounds);

    public static int il2cpp_array_element_size(IntPtr array_class) => _exports.il2cpp_array_element_size(array_class);

    public static IntPtr il2cpp_assembly_get_image(IntPtr assembly) => _exports.il2cpp_assembly_get_image(assembly);

    public static IntPtr il2cpp_class_enum_basetype(IntPtr klass) => _exports.il2cpp_class_enum_basetype(klass);

    public static bool il2cpp_class_is_generic(IntPtr klass) => _exports.il2cpp_class_is_generic(klass);

    public static bool il2cpp_class_is_inflated(IntPtr klass) => _exports.il2cpp_class_is_inflated(klass);

    public static bool il2cpp_class_is_assignable_from(IntPtr klass, IntPtr oklass) => _exports.il2cpp_class_is_assignable_from(klass, oklass);

    public static bool il2cpp_class_is_subclass_of(IntPtr klass, IntPtr klassc, [MarshalAs(UnmanagedType.I1)] bool check_interfaces) => _exports.il2cpp_class_is_subclass_of(klass, klassc, check_interfaces);

    public static bool il2cpp_class_has_parent(IntPtr klass, IntPtr klassc) => _exports.il2cpp_class_has_parent(klass, klassc);

    public static IntPtr il2cpp_class_from_name(IntPtr image, [MarshalAs(UnmanagedType.LPUTF8Str)] string namespaze, [MarshalAs(UnmanagedType.LPUTF8Str)] string name) => _exports.il2cpp_class_from_name(image, namespaze, name);

    public static IntPtr il2cpp_class_get_element_class(IntPtr klass) => _exports.il2cpp_class_get_element_class(klass);

    public static IntPtr il2cpp_class_get_fields(IntPtr klass, ref IntPtr iter) => _exports.il2cpp_class_get_fields(klass, ref iter);

    public static IntPtr il2cpp_class_get_interfaces(IntPtr klass, ref IntPtr iter) => _exports.il2cpp_class_get_interfaces(klass, ref iter);

    public static IntPtr il2cpp_class_get_property_from_name(IntPtr klass, IntPtr name) => _exports.il2cpp_class_get_property_from_name(klass, name);

    public static IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter) => _exports.il2cpp_class_get_methods(klass, ref iter);

    public static nint il2cpp_class_get_name(IntPtr klass) => _exports.il2cpp_class_get_name(klass);

    public static IntPtr il2cpp_class_get_parent(IntPtr klass) => _exports.il2cpp_class_get_parent(klass);

    public static int il2cpp_class_instance_size(IntPtr klass) => _exports.il2cpp_class_instance_size(klass);

    public static bool il2cpp_class_is_valuetype(IntPtr klass) => _exports.il2cpp_class_is_valuetype(klass);

    public static bool il2cpp_class_is_blittable(IntPtr klass) => _exports.il2cpp_class_is_blittable(klass);

    public static bool il2cpp_class_is_abstract(IntPtr klass) => _exports.il2cpp_class_is_abstract(klass);

    public static bool il2cpp_class_is_interface(IntPtr klass) => _exports.il2cpp_class_is_interface(klass);

    public static IntPtr il2cpp_class_from_type(IntPtr type) => _exports.il2cpp_class_from_type(type);

    public static uint il2cpp_class_get_type_token(IntPtr klass) => _exports.il2cpp_class_get_type_token(klass);

    public static bool il2cpp_class_has_attribute(IntPtr klass, IntPtr attr_class) => _exports.il2cpp_class_has_attribute(klass, attr_class);

    public static bool il2cpp_class_has_references(IntPtr klass) => _exports.il2cpp_class_has_references(klass);

    public static bool il2cpp_class_is_enum(IntPtr klass) => _exports.il2cpp_class_is_enum(klass);

    public static nint il2cpp_class_get_assemblyname(IntPtr klass) => _exports.il2cpp_class_get_assemblyname(klass);

    public static uint il2cpp_class_get_bitmap_size(IntPtr klass) => _exports.il2cpp_class_get_bitmap_size(klass);

    public static bool il2cpp_stats_dump_to_file(IntPtr path) => _exports.il2cpp_stats_dump_to_file(path);

    public static IntPtr il2cpp_domain_get() => _exports.il2cpp_domain_get();

    public static IntPtr il2cpp_domain_assembly_open(IntPtr domain, IntPtr name) => _exports.il2cpp_domain_assembly_open(domain, name);

    public static IntPtr* il2cpp_domain_get_assemblies(IntPtr domain, ref uint size) => _exports.il2cpp_domain_get_assemblies(domain, ref size);

    public static IntPtr il2cpp_exception_from_name_msg(IntPtr image, IntPtr name_space, IntPtr name, IntPtr msg) => _exports.il2cpp_exception_from_name_msg(image, name_space, name, msg);

    public static void il2cpp_format_exception(IntPtr ex, void* message, int message_size) => _exports.il2cpp_format_exception(ex, message, message_size);

    public static void il2cpp_unhandled_exception(IntPtr ex) => _exports.il2cpp_unhandled_exception(ex);

    public static nint il2cpp_field_get_name(IntPtr field) => _exports.il2cpp_field_get_name(field);

    public static uint il2cpp_field_get_offset(IntPtr field) => _exports.il2cpp_field_get_offset(field);

    public static void il2cpp_field_get_value(IntPtr obj, IntPtr field, void* value) => _exports.il2cpp_field_get_value(obj, field, value);

    public static bool il2cpp_field_has_attribute(IntPtr field, IntPtr attr_class) => _exports.il2cpp_field_has_attribute(field, attr_class);

    public static void il2cpp_field_static_get_value(IntPtr field, void* value) => _exports.il2cpp_field_static_get_value(field, value);

    public static void il2cpp_field_set_value_object(IntPtr instance, IntPtr field, IntPtr value) => _exports.il2cpp_field_set_value_object(instance, field, value);

    public static int il2cpp_gc_collect_a_little() => _exports.il2cpp_gc_collect_a_little();

    public static void il2cpp_gc_enable() => _exports.il2cpp_gc_enable();

    public static bool il2cpp_gc_is_disabled() => _exports.il2cpp_gc_is_disabled();

    public static long il2cpp_gc_get_heap_size() => _exports.il2cpp_gc_get_heap_size();

    public static nint il2cpp_gchandle_new(IntPtr obj, [MarshalAs(UnmanagedType.I1)] bool pinned) => _exports.il2cpp_gchandle_new(obj, pinned);

    public static IntPtr il2cpp_gchandle_get_target(nint gchandle) => _exports.il2cpp_gchandle_get_target(gchandle);

    public static IntPtr il2cpp_unity_liveness_calculation_begin(IntPtr filter, int max_object_count, IntPtr callback, IntPtr userdata, IntPtr onWorldStarted, IntPtr onWorldStopped) => _exports.il2cpp_unity_liveness_calculation_begin(filter, max_object_count, callback, userdata, onWorldStarted, onWorldStopped);

    public static void il2cpp_unity_liveness_calculation_from_root(IntPtr root, IntPtr state) => _exports.il2cpp_unity_liveness_calculation_from_root(root, state);

    public static IntPtr il2cpp_method_get_return_type(IntPtr method) => _exports.il2cpp_method_get_return_type(method);

    public static string? il2cpp_method_get_name(IntPtr method) => Marshal.PtrToStringUTF8(_exports.il2cpp_method_get_name(method));

    public static IntPtr il2cpp_method_get_object(IntPtr method, IntPtr refclass) => _exports.il2cpp_method_get_object(method, refclass);

    public static bool il2cpp_method_is_generic(IntPtr method) => _exports.il2cpp_method_is_generic(method);

    public static bool il2cpp_method_is_inflated(IntPtr method) => _exports.il2cpp_method_is_inflated(method);

    public static bool il2cpp_method_is_instance(IntPtr method) => _exports.il2cpp_method_is_instance(method);

    public static uint il2cpp_method_get_param_count(IntPtr method) => _exports.il2cpp_method_get_param_count(method);

    public static IntPtr il2cpp_method_get_param(IntPtr method, uint index) => _exports.il2cpp_method_get_param(method, index);

    public static bool il2cpp_method_has_attribute(IntPtr method, IntPtr attr_class) => _exports.il2cpp_method_has_attribute(method, attr_class);

    public static uint il2cpp_method_get_token(IntPtr method) => _exports.il2cpp_method_get_token(method);

    public static void il2cpp_profiler_install(IntPtr prof, IntPtr shutdown_callback) => _exports.il2cpp_profiler_install(prof, shutdown_callback);

    public static void il2cpp_profiler_install_allocation(IntPtr callback) => _exports.il2cpp_profiler_install_allocation(callback);

    public static void il2cpp_profiler_install_fileio(IntPtr callback) => _exports.il2cpp_profiler_install_fileio(callback);

    public static uint il2cpp_property_get_flags(IntPtr prop) => _exports.il2cpp_property_get_flags(prop);

    public static IntPtr il2cpp_property_get_set_method(IntPtr prop) => _exports.il2cpp_property_get_set_method(prop);

    public static IntPtr il2cpp_property_get_parent(IntPtr prop) => _exports.il2cpp_property_get_parent(prop);

    public static uint il2cpp_object_get_size(IntPtr obj) => _exports.il2cpp_object_get_size(obj);

    public static IntPtr il2cpp_object_new(IntPtr klass) => _exports.il2cpp_object_new(klass);

    public static IntPtr il2cpp_object_unbox(IntPtr obj) => _exports.il2cpp_object_unbox(obj);

    public static IntPtr il2cpp_value_box(IntPtr klass, IntPtr data) => _exports.il2cpp_value_box(klass, data);

    public static bool il2cpp_monitor_try_enter(IntPtr obj, uint timeout) => _exports.il2cpp_monitor_try_enter(obj, timeout);

    public static void il2cpp_monitor_pulse(IntPtr obj) => _exports.il2cpp_monitor_pulse(obj);

    public static void il2cpp_monitor_wait(IntPtr obj) => _exports.il2cpp_monitor_wait(obj);

    public static bool il2cpp_monitor_try_wait(IntPtr obj, uint timeout) => _exports.il2cpp_monitor_try_wait(obj, timeout);

    public static IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc) => _exports.il2cpp_runtime_invoke(method, obj, param, ref exc);

    public static IntPtr il2cpp_runtime_invoke_convert_args(IntPtr method, IntPtr obj, void** param, int paramCount, ref IntPtr exc) => _exports.il2cpp_runtime_invoke_convert_args(method, obj, param, paramCount, ref exc);

    public static void il2cpp_runtime_object_init(IntPtr obj) => _exports.il2cpp_runtime_object_init(obj);

    public static char* il2cpp_string_chars(IntPtr str) => _exports.il2cpp_string_chars(str);

    public static IntPtr il2cpp_string_new_len(string str, uint length) => _exports.il2cpp_string_new_len(str, length);

    public static IntPtr il2cpp_string_new_wrapper(string str) => _exports.il2cpp_string_new_wrapper(str);

    public static IntPtr il2cpp_string_is_interned(string str) => _exports.il2cpp_string_is_interned(str);

    public static IntPtr il2cpp_thread_attach(IntPtr domain) => _exports.il2cpp_thread_attach(domain);

    public static void** il2cpp_thread_get_all_attached_threads(ref uint size) => _exports.il2cpp_thread_get_all_attached_threads(ref size);

    public static bool il2cpp_is_vm_thread(IntPtr thread) => _exports.il2cpp_is_vm_thread(thread);

    public static void il2cpp_thread_walk_frame_stack(IntPtr thread, IntPtr func, IntPtr user_data) => _exports.il2cpp_thread_walk_frame_stack(thread, func, user_data);

    public static bool il2cpp_current_thread_get_top_frame(IntPtr frame) => _exports.il2cpp_current_thread_get_top_frame(frame);

    public static bool il2cpp_thread_get_top_frame(IntPtr thread, IntPtr frame) => _exports.il2cpp_thread_get_top_frame(thread, frame);

    public static bool il2cpp_current_thread_get_frame_at(int offset, IntPtr frame) => _exports.il2cpp_current_thread_get_frame_at(offset, frame);

    public static bool il2cpp_thread_get_frame_at(IntPtr thread, int offset, IntPtr frame) => _exports.il2cpp_thread_get_frame_at(thread, offset, frame);

    public static int il2cpp_thread_get_stack_depth(IntPtr thread) => _exports.il2cpp_thread_get_stack_depth(thread);

    public static int il2cpp_type_get_type(IntPtr type) => _exports.il2cpp_type_get_type(type);

    public static string? il2cpp_type_get_name(IntPtr type) => Marshal.PtrToStringUTF8(_exports.il2cpp_type_get_name(type));

    public static bool il2cpp_type_is_byref(IntPtr type) => _exports.il2cpp_type_is_byref(type);

    public static bool il2cpp_type_equals(IntPtr type, IntPtr otherType) => _exports.il2cpp_type_equals(type, otherType);

    public static IntPtr il2cpp_image_get_assembly(IntPtr image) => _exports.il2cpp_image_get_assembly(image);

    public static string? il2cpp_image_get_name(IntPtr image) => Marshal.PtrToStringUTF8(_exports.il2cpp_image_get_name(image));

    public static nint il2cpp_image_get_filename(IntPtr image) => _exports.il2cpp_image_get_filename(image);

    public static uint il2cpp_image_get_class_count(IntPtr image) => _exports.il2cpp_image_get_class_count(image);

    public static IntPtr il2cpp_capture_memory_snapshot() => _exports.il2cpp_capture_memory_snapshot();

    public static void il2cpp_set_find_plugin_callback(IntPtr method) => _exports.il2cpp_set_find_plugin_callback(method);

    public static void il2cpp_debugger_set_agent_options(IntPtr options) => _exports.il2cpp_debugger_set_agent_options(options);

    public static bool il2cpp_is_debugger_attached() => _exports.il2cpp_is_debugger_attached();

    public static IntPtr il2cpp_custom_attrs_from_class(IntPtr klass) => _exports.il2cpp_custom_attrs_from_class(klass);

    public static IntPtr il2cpp_custom_attrs_get_attr(IntPtr ainfo, IntPtr attr_klass) => _exports.il2cpp_custom_attrs_get_attr(ainfo, attr_klass);

    public static bool il2cpp_custom_attrs_has_attr(IntPtr ainfo, IntPtr attr_klass) => _exports.il2cpp_custom_attrs_has_attr(ainfo, attr_klass);

    public static void il2cpp_custom_attrs_free(IntPtr ainfo) => _exports.il2cpp_custom_attrs_free(ainfo);

    private class Delegates
    {

        public Delegates(IntPtr handle)
        {
            #region Load Exports

            il2cpp_init_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_init_utf16_delegate>(handle, "il2cpp_init_utf16");
            il2cpp_set_config_dir = NativeLibraryUtil.LoadFunction<il2cpp_set_config_dir_delegate>(handle, "il2cpp_set_config_dir");
            il2cpp_set_temp_dir = NativeLibraryUtil.LoadFunction<il2cpp_set_temp_dir_delegate>(handle, "il2cpp_set_temp_dir");
            il2cpp_set_commandline_arguments_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_set_commandline_arguments_utf16_delegate>(handle, "il2cpp_set_commandline_arguments_utf16");
            il2cpp_set_config = NativeLibraryUtil.LoadFunction<il2cpp_set_config_delegate>(handle, "il2cpp_set_config");
            il2cpp_get_corlib = NativeLibraryUtil.LoadFunction<il2cpp_get_corlib_delegate>(handle, "il2cpp_get_corlib");
            il2cpp_resolve_icall = NativeLibraryUtil.LoadFunction<il2cpp_resolve_icall_delegate>(handle, "il2cpp_resolve_icall");
            il2cpp_free = NativeLibraryUtil.LoadFunction<il2cpp_free_delegate>(handle, "il2cpp_free");
            il2cpp_array_length = NativeLibraryUtil.LoadFunction<il2cpp_array_length_delegate>(handle, "il2cpp_array_length");
            il2cpp_array_new = NativeLibraryUtil.LoadFunction<il2cpp_array_new_delegate>(handle, "il2cpp_array_new");
            il2cpp_array_new_full = NativeLibraryUtil.LoadFunction<il2cpp_array_new_full_delegate>(handle, "il2cpp_array_new_full");
            il2cpp_array_element_size = NativeLibraryUtil.LoadFunction<il2cpp_array_element_size_delegate>(handle, "il2cpp_array_element_size");
            il2cpp_assembly_get_image = NativeLibraryUtil.LoadFunction<il2cpp_assembly_get_image_delegate>(handle, "il2cpp_assembly_get_image");
            il2cpp_class_enum_basetype = NativeLibraryUtil.LoadFunction<il2cpp_class_enum_basetype_delegate>(handle, "il2cpp_class_enum_basetype");
            il2cpp_class_is_generic = NativeLibraryUtil.LoadFunction<il2cpp_class_is_generic_delegate>(handle, "il2cpp_class_is_generic");
            il2cpp_class_is_inflated = NativeLibraryUtil.LoadFunction<il2cpp_class_is_inflated_delegate>(handle, "il2cpp_class_is_inflated");
            il2cpp_class_is_assignable_from = NativeLibraryUtil.LoadFunction<il2cpp_class_is_assignable_from_delegate>(handle, "il2cpp_class_is_assignable_from");
            il2cpp_class_is_subclass_of = NativeLibraryUtil.LoadFunction<il2cpp_class_is_subclass_of_delegate>(handle, "il2cpp_class_is_subclass_of");
            il2cpp_class_has_parent = NativeLibraryUtil.LoadFunction<il2cpp_class_has_parent_delegate>(handle, "il2cpp_class_has_parent");
            il2cpp_class_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_from_name_delegate>(handle, "il2cpp_class_from_name");
            il2cpp_class_get_element_class = NativeLibraryUtil.LoadFunction<il2cpp_class_get_element_class_delegate>(handle, "il2cpp_class_get_element_class");
            il2cpp_class_get_fields = NativeLibraryUtil.LoadFunction<il2cpp_class_get_fields_delegate>(handle, "il2cpp_class_get_fields");
            il2cpp_class_get_interfaces = NativeLibraryUtil.LoadFunction<il2cpp_class_get_interfaces_delegate>(handle, "il2cpp_class_get_interfaces");
            il2cpp_class_get_property_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_property_from_name_delegate>(handle, "il2cpp_class_get_property_from_name");
            il2cpp_class_get_methods = NativeLibraryUtil.LoadFunction<il2cpp_class_get_methods_delegate>(handle, "il2cpp_class_get_methods");
            il2cpp_class_get_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_name_delegate>(handle, "il2cpp_class_get_name");
            il2cpp_class_get_parent = NativeLibraryUtil.LoadFunction<il2cpp_class_get_parent_delegate>(handle, "il2cpp_class_get_parent");
            il2cpp_class_instance_size = NativeLibraryUtil.LoadFunction<il2cpp_class_instance_size_delegate>(handle, "il2cpp_class_instance_size");
            il2cpp_class_is_valuetype = NativeLibraryUtil.LoadFunction<il2cpp_class_is_valuetype_delegate>(handle, "il2cpp_class_is_valuetype");
            il2cpp_class_is_blittable = NativeLibraryUtil.LoadFunction<il2cpp_class_is_blittable_delegate>(handle, "il2cpp_class_is_blittable");
            il2cpp_class_is_abstract = NativeLibraryUtil.LoadFunction<il2cpp_class_is_abstract_delegate>(handle, "il2cpp_class_is_abstract");
            il2cpp_class_is_interface = NativeLibraryUtil.LoadFunction<il2cpp_class_is_interface_delegate>(handle, "il2cpp_class_is_interface");
            il2cpp_class_from_type = NativeLibraryUtil.LoadFunction<il2cpp_class_from_type_delegate>(handle, "il2cpp_class_from_type");
            il2cpp_class_get_type_token = NativeLibraryUtil.LoadFunction<il2cpp_class_get_type_token_delegate>(handle, "il2cpp_class_get_type_token");
            il2cpp_class_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_class_has_attribute_delegate>(handle, "il2cpp_class_has_attribute");
            il2cpp_class_has_references = NativeLibraryUtil.LoadFunction<il2cpp_class_has_references_delegate>(handle, "il2cpp_class_has_references");
            il2cpp_class_is_enum = NativeLibraryUtil.LoadFunction<il2cpp_class_is_enum_delegate>(handle, "il2cpp_class_is_enum");
            il2cpp_class_get_assemblyname = NativeLibraryUtil.LoadFunction<il2cpp_class_get_assemblyname_delegate>(handle, "il2cpp_class_get_assemblyname");
            il2cpp_class_get_bitmap_size = NativeLibraryUtil.LoadFunction<il2cpp_class_get_bitmap_size_delegate>(handle, "il2cpp_class_get_bitmap_size");
            il2cpp_stats_dump_to_file = NativeLibraryUtil.LoadFunction<il2cpp_stats_dump_to_file_delegate>(handle, "il2cpp_stats_dump_to_file");
            il2cpp_domain_get = NativeLibraryUtil.LoadFunction<il2cpp_domain_get_delegate>(handle, "il2cpp_domain_get");
            il2cpp_domain_assembly_open = NativeLibraryUtil.LoadFunction<il2cpp_domain_assembly_open_delegate>(handle, "il2cpp_domain_assembly_open");
            il2cpp_domain_get_assemblies = NativeLibraryUtil.LoadFunction<il2cpp_domain_get_assemblies_delegate>(handle, "il2cpp_domain_get_assemblies");
            il2cpp_exception_from_name_msg = NativeLibraryUtil.LoadFunction<il2cpp_exception_from_name_msg_delegate>(handle, "il2cpp_exception_from_name_msg");
            il2cpp_format_exception = NativeLibraryUtil.LoadFunction<il2cpp_format_exception_delegate>(handle, "il2cpp_format_exception");
            il2cpp_unhandled_exception = NativeLibraryUtil.LoadFunction<il2cpp_unhandled_exception_delegate>(handle, "il2cpp_unhandled_exception");
            il2cpp_field_get_name = NativeLibraryUtil.LoadFunction<il2cpp_field_get_name_delegate>(handle, "il2cpp_field_get_name");
            il2cpp_field_get_offset = NativeLibraryUtil.LoadFunction<il2cpp_field_get_offset_delegate>(handle, "il2cpp_field_get_offset");
            il2cpp_field_get_value = NativeLibraryUtil.LoadFunction<il2cpp_field_get_value_delegate>(handle, "il2cpp_field_get_value");
            il2cpp_field_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_field_has_attribute_delegate>(handle, "il2cpp_field_has_attribute");
            il2cpp_field_static_get_value = NativeLibraryUtil.LoadFunction<il2cpp_field_static_get_value_delegate>(handle, "il2cpp_field_static_get_value");
            il2cpp_field_set_value_object = NativeLibraryUtil.LoadFunction<il2cpp_field_set_value_object_delegate>(handle, "il2cpp_field_set_value_object");
            il2cpp_gc_collect_a_little = NativeLibraryUtil.LoadFunction<il2cpp_gc_collect_a_little_delegate>(handle, "il2cpp_gc_collect_a_little");
            il2cpp_gc_enable = NativeLibraryUtil.LoadFunction<il2cpp_gc_enable_delegate>(handle, "il2cpp_gc_enable");
            il2cpp_gc_is_disabled = NativeLibraryUtil.LoadFunction<il2cpp_gc_is_disabled_delegate>(handle, "il2cpp_gc_is_disabled");
            il2cpp_gc_get_heap_size = NativeLibraryUtil.LoadFunction<il2cpp_gc_get_heap_size_delegate>(handle, "il2cpp_gc_get_heap_size");
            il2cpp_gchandle_new = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_new_delegate>(handle, "il2cpp_gchandle_new");
            il2cpp_gchandle_get_target = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_get_target_delegate>(handle, "il2cpp_gchandle_get_target");
            il2cpp_unity_liveness_calculation_begin = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_begin_delegate>(handle, "il2cpp_unity_liveness_calculation_begin");
            il2cpp_unity_liveness_calculation_from_root = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_from_root_delegate>(handle, "il2cpp_unity_liveness_calculation_from_root");
            il2cpp_method_get_return_type = NativeLibraryUtil.LoadFunction<il2cpp_method_get_return_type_delegate>(handle, "il2cpp_method_get_return_type");
            il2cpp_method_get_name = NativeLibraryUtil.LoadFunction<il2cpp_method_get_name_delegate>(handle, "il2cpp_method_get_name");
            il2cpp_method_get_object = NativeLibraryUtil.LoadFunction<il2cpp_method_get_object_delegate>(handle, "il2cpp_method_get_from_reflection");
            il2cpp_method_is_generic = NativeLibraryUtil.LoadFunction<il2cpp_method_is_generic_delegate>(handle, "il2cpp_method_is_generic");
            il2cpp_method_is_inflated = NativeLibraryUtil.LoadFunction<il2cpp_method_is_inflated_delegate>(handle, "il2cpp_method_is_inflated");
            il2cpp_method_is_instance = NativeLibraryUtil.LoadFunction<il2cpp_method_is_instance_delegate>(handle, "il2cpp_method_is_instance");
            il2cpp_method_get_param_count = NativeLibraryUtil.LoadFunction<il2cpp_method_get_param_count_delegate>(handle, "il2cpp_method_get_param_count");
            il2cpp_method_get_param = NativeLibraryUtil.LoadFunction<il2cpp_method_get_param_delegate>(handle, "il2cpp_method_get_param");
            il2cpp_method_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_method_has_attribute_delegate>(handle, "il2cpp_method_has_attribute");
            il2cpp_method_get_token = NativeLibraryUtil.LoadFunction<il2cpp_method_get_token_delegate>(handle, "il2cpp_method_get_token");
            il2cpp_profiler_install = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_delegate>(handle, "il2cpp_profiler_install");
            il2cpp_profiler_install_allocation = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_allocation_delegate>(handle, "il2cpp_profiler_install_allocation");
            il2cpp_profiler_install_fileio = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_fileio_delegate>(handle, "il2cpp_profiler_install_fileio");
            il2cpp_property_get_flags = NativeLibraryUtil.LoadFunction<il2cpp_property_get_flags_delegate>(handle, "il2cpp_property_get_flags");
            il2cpp_property_get_set_method = NativeLibraryUtil.LoadFunction<il2cpp_property_get_set_method_delegate>(handle, "il2cpp_property_get_set_method");
            il2cpp_property_get_parent = NativeLibraryUtil.LoadFunction<il2cpp_property_get_parent_delegate>(handle, "il2cpp_property_get_parent");
            il2cpp_object_get_size = NativeLibraryUtil.LoadFunction<il2cpp_object_get_size_delegate>(handle, "il2cpp_object_get_size");
            il2cpp_object_new = NativeLibraryUtil.LoadFunction<il2cpp_object_new_delegate>(handle, "il2cpp_object_new");
            il2cpp_object_unbox = NativeLibraryUtil.LoadFunction<il2cpp_object_unbox_delegate>(handle, "il2cpp_object_unbox");
            il2cpp_value_box = NativeLibraryUtil.LoadFunction<il2cpp_value_box_delegate>(handle, "il2cpp_value_box");
            il2cpp_monitor_try_enter = NativeLibraryUtil.LoadFunction<il2cpp_monitor_try_enter_delegate>(handle, "il2cpp_monitor_try_enter");
            il2cpp_monitor_pulse = NativeLibraryUtil.LoadFunction<il2cpp_monitor_pulse_delegate>(handle, "il2cpp_monitor_pulse");
            il2cpp_monitor_wait = NativeLibraryUtil.LoadFunction<il2cpp_monitor_wait_delegate>(handle, "il2cpp_monitor_wait");
            il2cpp_monitor_try_wait = NativeLibraryUtil.LoadFunction<il2cpp_monitor_try_wait_delegate>(handle, "il2cpp_monitor_try_wait");
            il2cpp_runtime_invoke = NativeLibraryUtil.LoadFunction<il2cpp_runtime_invoke_delegate>(handle, "il2cpp_runtime_invoke");
            il2cpp_runtime_invoke_convert_args = NativeLibraryUtil.LoadFunction<il2cpp_runtime_invoke_convert_args_delegate>(handle, "il2cpp_runtime_invoke_convert_args");
            il2cpp_runtime_object_init = NativeLibraryUtil.LoadFunction<il2cpp_runtime_object_init_delegate>(handle, "il2cpp_runtime_object_init");
            il2cpp_string_chars = NativeLibraryUtil.LoadFunction<il2cpp_string_chars_delegate>(handle, "il2cpp_string_chars");
            il2cpp_string_new_len = NativeLibraryUtil.LoadFunction<il2cpp_string_new_len_delegate>(handle, "il2cpp_string_new_len");
            il2cpp_string_new_wrapper = NativeLibraryUtil.LoadFunction<il2cpp_string_new_wrapper_delegate>(handle, "il2cpp_string_new_wrapper");
            il2cpp_string_is_interned = NativeLibraryUtil.LoadFunction<il2cpp_string_is_interned_delegate>(handle, "il2cpp_string_is_interned");
            il2cpp_thread_attach = NativeLibraryUtil.LoadFunction<il2cpp_thread_attach_delegate>(handle, "il2cpp_thread_attach");
            il2cpp_thread_get_all_attached_threads = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_all_attached_threads_delegate>(handle, "il2cpp_thread_get_all_attached_threads");
            il2cpp_is_vm_thread = NativeLibraryUtil.LoadFunction<il2cpp_is_vm_thread_delegate>(handle, "il2cpp_is_vm_thread");
            il2cpp_thread_walk_frame_stack = NativeLibraryUtil.LoadFunction<il2cpp_thread_walk_frame_stack_delegate>(handle, "il2cpp_thread_walk_frame_stack");
            il2cpp_current_thread_get_top_frame = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_get_top_frame_delegate>(handle, "il2cpp_current_thread_get_top_frame");
            il2cpp_thread_get_top_frame = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_top_frame_delegate>(handle, "il2cpp_thread_get_top_frame");
            il2cpp_current_thread_get_frame_at = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_get_frame_at_delegate>(handle, "il2cpp_current_thread_get_frame_at");
            il2cpp_thread_get_frame_at = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_frame_at_delegate>(handle, "il2cpp_thread_get_frame_at");
            il2cpp_thread_get_stack_depth = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_stack_depth_delegate>(handle, "il2cpp_thread_get_stack_depth");
            il2cpp_type_get_type = NativeLibraryUtil.LoadFunction<il2cpp_type_get_type_delegate>(handle, "il2cpp_type_get_type");
            il2cpp_type_get_name = NativeLibraryUtil.LoadFunction<il2cpp_type_get_name_delegate>(handle, "il2cpp_type_get_name");
            il2cpp_type_is_byref = NativeLibraryUtil.LoadFunction<il2cpp_type_is_byref_delegate>(handle, "il2cpp_type_is_byref");
            il2cpp_type_equals = NativeLibraryUtil.LoadFunction<il2cpp_type_equals_delegate>(handle, "il2cpp_type_equals");
            il2cpp_image_get_assembly = NativeLibraryUtil.LoadFunction<il2cpp_image_get_assembly_delegate>(handle, "il2cpp_image_get_assembly");
            il2cpp_image_get_name = NativeLibraryUtil.LoadFunction<il2cpp_image_get_name_delegate>(handle, "il2cpp_image_get_name");
            il2cpp_image_get_filename = NativeLibraryUtil.LoadFunction<il2cpp_image_get_filename_delegate>(handle, "il2cpp_image_get_filename");
            il2cpp_image_get_class_count = NativeLibraryUtil.LoadFunction<il2cpp_image_get_class_count_delegate>(handle, "il2cpp_image_get_class_count");
            il2cpp_capture_memory_snapshot = NativeLibraryUtil.LoadFunction<il2cpp_capture_memory_snapshot_delegate>(handle, "il2cpp_capture_memory_snapshot");
            il2cpp_set_find_plugin_callback = NativeLibraryUtil.LoadFunction<il2cpp_set_find_plugin_callback_delegate>(handle, "il2cpp_set_find_plugin_callback");
            il2cpp_debugger_set_agent_options = NativeLibraryUtil.LoadFunction<il2cpp_debugger_set_agent_options_delegate>(handle, "il2cpp_debugger_set_agent_options");
            il2cpp_is_debugger_attached = NativeLibraryUtil.LoadFunction<il2cpp_is_debugger_attached_delegate>(handle, "il2cpp_is_debugger_attached");
            il2cpp_custom_attrs_from_class = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_from_class_delegate>(handle, "il2cpp_custom_attrs_from_class");
            il2cpp_custom_attrs_get_attr = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_get_attr_delegate>(handle, "il2cpp_custom_attrs_get_attr");
            il2cpp_custom_attrs_has_attr = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_has_attr_delegate>(handle, "il2cpp_custom_attrs_has_attr");
            il2cpp_custom_attrs_free = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_free_delegate>(handle, "il2cpp_custom_attrs_free");

            #endregion
        }

        #region Delegate Definitions

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_init_utf16_delegate(IntPtr domain_name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_config_dir_delegate(IntPtr config_path);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_temp_dir_delegate(IntPtr temp_path);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_commandline_arguments_utf16_delegate(int argc, IntPtr argv, IntPtr basedir);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_config_delegate(IntPtr executablePath);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_get_corlib_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_resolve_icall_delegate([MarshalAs(UnmanagedType.LPStr)] string name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_free_delegate(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_array_length_delegate(IntPtr array);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_new_delegate(IntPtr elementTypeInfo, ulong length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_new_full_delegate(IntPtr array_class, ref ulong lengths, ref ulong lower_bounds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_array_element_size_delegate(IntPtr array_class);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_enum_basetype_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_assembly_get_image_delegate(IntPtr assembly);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_generic_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_inflated_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_assignable_from_delegate(IntPtr klass, IntPtr oklass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_subclass_of_delegate(IntPtr klass, IntPtr klassc, [MarshalAs(UnmanagedType.I1)] bool check_interfaces);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_parent_delegate(IntPtr klass, IntPtr klassc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_name_delegate(IntPtr image, [MarshalAs(UnmanagedType.LPUTF8Str)] string namespaze, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_element_class_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_fields_delegate(IntPtr klass, ref IntPtr iter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_interfaces_delegate(IntPtr klass, ref IntPtr iter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_property_from_name_delegate(IntPtr klass, IntPtr name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_methods_delegate(IntPtr klass, ref IntPtr iter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_class_get_name_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_parent_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_instance_size_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_valuetype_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_blittable_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_abstract_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_interface_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_type_delegate(IntPtr type);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_class_get_type_token_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_attribute_delegate(IntPtr klass, IntPtr attr_class);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_references_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_enum_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_class_get_assemblyname_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_class_get_bitmap_size_delegate(IntPtr klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_stats_dump_to_file_delegate(IntPtr path);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_domain_get_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_domain_assembly_open_delegate(IntPtr domain, IntPtr name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr* il2cpp_domain_get_assemblies_delegate(IntPtr domain, ref uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_exception_from_name_msg_delegate(IntPtr image, IntPtr name_space, IntPtr name, IntPtr msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_format_exception_delegate(IntPtr ex, void* message, int message_size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unhandled_exception_delegate(IntPtr ex);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_field_get_name_delegate(IntPtr field);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_field_get_offset_delegate(IntPtr field);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_get_value_delegate(IntPtr obj, IntPtr field, void* value);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_field_has_attribute_delegate(IntPtr field, IntPtr attr_class);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_static_get_value_delegate(IntPtr field, void* value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_set_value_object_delegate(IntPtr instance, IntPtr field, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_gc_collect_a_little_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_enable_delegate();

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_gc_is_disabled_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate long il2cpp_gc_get_heap_size_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_gchandle_new_delegate(IntPtr obj, [MarshalAs(UnmanagedType.I1)] bool pinned);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_gchandle_get_target_delegate(nint gchandle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_unity_liveness_calculation_begin_delegate(IntPtr filter, int max_object_count, IntPtr callback, IntPtr userdata, IntPtr onWorldStarted, IntPtr onWorldStopped);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_calculation_from_root_delegate(IntPtr root, IntPtr state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_return_type_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_method_get_name_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_object_delegate(IntPtr method, IntPtr refclass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_generic_delegate(IntPtr method);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_inflated_delegate(IntPtr method);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_instance_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_method_get_param_count_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_param_delegate(IntPtr method, uint index);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_has_attribute_delegate(IntPtr method, IntPtr attr_class);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_method_get_token_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_delegate(IntPtr prof, IntPtr shutdown_callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_allocation_delegate(IntPtr callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_fileio_delegate(IntPtr callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_property_get_flags_delegate(IntPtr prop);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_property_get_set_method_delegate(IntPtr prop);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_property_get_parent_delegate(IntPtr prop);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_object_get_size_delegate(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_object_new_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_object_unbox_delegate(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_value_box_delegate(IntPtr klass, IntPtr data);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_monitor_try_enter_delegate(IntPtr obj, uint timeout);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_pulse_delegate(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_wait_delegate(IntPtr obj);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_monitor_try_wait_delegate(IntPtr obj, uint timeout);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_runtime_invoke_delegate(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_runtime_invoke_convert_args_delegate(IntPtr method, IntPtr obj, void** param, int paramCount, ref IntPtr exc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_runtime_object_init_delegate(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate char* il2cpp_string_chars_delegate(IntPtr str);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_len_delegate(string str, uint length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_wrapper_delegate(string str);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_is_interned_delegate(string str);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_thread_attach_delegate(IntPtr domain);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void** il2cpp_thread_get_all_attached_threads_delegate(ref uint size);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_is_vm_thread_delegate(IntPtr thread);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_thread_walk_frame_stack_delegate(IntPtr thread, IntPtr func, IntPtr user_data);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_current_thread_get_top_frame_delegate(IntPtr frame);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_thread_get_top_frame_delegate(IntPtr thread, IntPtr frame);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_current_thread_get_frame_at_delegate(int offset, IntPtr frame);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_thread_get_frame_at_delegate(IntPtr thread, int offset, IntPtr frame);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_thread_get_stack_depth_delegate(IntPtr thread);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_type_get_type_delegate(IntPtr type);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_type_get_name_delegate(IntPtr type);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_is_byref_delegate(IntPtr type);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_equals_delegate(IntPtr type, IntPtr otherType);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_image_get_assembly_delegate(IntPtr image);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_image_get_name_delegate(IntPtr image);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_image_get_filename_delegate(IntPtr image);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_image_get_class_count_delegate(IntPtr image);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_capture_memory_snapshot_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_find_plugin_callback_delegate(IntPtr method);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_debugger_set_agent_options_delegate(IntPtr options);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_is_debugger_attached_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_from_class_delegate(IntPtr klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_get_attr_delegate(IntPtr ainfo, IntPtr attr_klass);

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_custom_attrs_has_attr_delegate(IntPtr ainfo, IntPtr attr_klass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_custom_attrs_free_delegate(IntPtr ainfo);

        #endregion

        #region Delegate Instances

        public il2cpp_init_utf16_delegate il2cpp_init_utf16 { get; }
        public il2cpp_set_config_dir_delegate il2cpp_set_config_dir { get; }
        public il2cpp_set_temp_dir_delegate il2cpp_set_temp_dir { get; }
        public il2cpp_set_commandline_arguments_utf16_delegate il2cpp_set_commandline_arguments_utf16 { get; }
        public il2cpp_set_config_delegate il2cpp_set_config { get; }
        public il2cpp_get_corlib_delegate il2cpp_get_corlib { get; }
        public il2cpp_resolve_icall_delegate il2cpp_resolve_icall { get; }
        public il2cpp_free_delegate il2cpp_free { get; }
        public il2cpp_array_length_delegate il2cpp_array_length { get; }
        public il2cpp_array_new_delegate il2cpp_array_new { get; }
        public il2cpp_array_new_full_delegate il2cpp_array_new_full { get; }
        public il2cpp_array_element_size_delegate il2cpp_array_element_size { get; }
        public il2cpp_assembly_get_image_delegate il2cpp_assembly_get_image { get; }
        public il2cpp_class_enum_basetype_delegate il2cpp_class_enum_basetype { get; }
        public il2cpp_class_is_generic_delegate il2cpp_class_is_generic { get; }
        public il2cpp_class_is_inflated_delegate il2cpp_class_is_inflated { get; }
        public il2cpp_class_is_assignable_from_delegate il2cpp_class_is_assignable_from { get; }
        public il2cpp_class_is_subclass_of_delegate il2cpp_class_is_subclass_of { get; }
        public il2cpp_class_has_parent_delegate il2cpp_class_has_parent { get; }
        public il2cpp_class_from_name_delegate il2cpp_class_from_name { get; }
        public il2cpp_class_get_element_class_delegate il2cpp_class_get_element_class { get; }
        public il2cpp_class_get_fields_delegate il2cpp_class_get_fields { get; }
        public il2cpp_class_get_interfaces_delegate il2cpp_class_get_interfaces { get; }
        public il2cpp_class_get_property_from_name_delegate il2cpp_class_get_property_from_name { get; }
        public il2cpp_class_get_methods_delegate il2cpp_class_get_methods { get; }
        public il2cpp_class_get_name_delegate il2cpp_class_get_name { get; }
        public il2cpp_class_get_parent_delegate il2cpp_class_get_parent { get; }
        public il2cpp_class_instance_size_delegate il2cpp_class_instance_size { get; }
        public il2cpp_class_is_valuetype_delegate il2cpp_class_is_valuetype { get; }
        public il2cpp_class_is_blittable_delegate il2cpp_class_is_blittable { get; }
        public il2cpp_class_is_abstract_delegate il2cpp_class_is_abstract { get; }
        public il2cpp_class_is_interface_delegate il2cpp_class_is_interface { get; }
        public il2cpp_class_from_type_delegate il2cpp_class_from_type { get; }
        public il2cpp_class_get_type_token_delegate il2cpp_class_get_type_token { get; }
        public il2cpp_class_has_attribute_delegate il2cpp_class_has_attribute { get; }
        public il2cpp_class_has_references_delegate il2cpp_class_has_references { get; }
        public il2cpp_class_is_enum_delegate il2cpp_class_is_enum { get; }
        public il2cpp_class_get_assemblyname_delegate il2cpp_class_get_assemblyname { get; }
        public il2cpp_class_get_bitmap_size_delegate il2cpp_class_get_bitmap_size { get; }
        public il2cpp_stats_dump_to_file_delegate il2cpp_stats_dump_to_file { get; }
        public il2cpp_domain_get_delegate il2cpp_domain_get { get; }
        public il2cpp_domain_assembly_open_delegate il2cpp_domain_assembly_open { get; }
        public il2cpp_domain_get_assemblies_delegate il2cpp_domain_get_assemblies { get; }
        public il2cpp_exception_from_name_msg_delegate il2cpp_exception_from_name_msg { get; }
        public il2cpp_format_exception_delegate il2cpp_format_exception { get; }
        public il2cpp_unhandled_exception_delegate il2cpp_unhandled_exception { get; }
        public il2cpp_field_get_name_delegate il2cpp_field_get_name { get; }
        public il2cpp_field_get_offset_delegate il2cpp_field_get_offset { get; }
        public il2cpp_field_get_value_delegate il2cpp_field_get_value { get; }
        public il2cpp_field_has_attribute_delegate il2cpp_field_has_attribute { get; }
        public il2cpp_field_static_get_value_delegate il2cpp_field_static_get_value { get; }
        public il2cpp_field_set_value_object_delegate il2cpp_field_set_value_object { get; }
        public il2cpp_gc_collect_a_little_delegate il2cpp_gc_collect_a_little { get; }
        public il2cpp_gc_enable_delegate il2cpp_gc_enable { get; }
        public il2cpp_gc_is_disabled_delegate il2cpp_gc_is_disabled { get; }
        public il2cpp_gc_get_heap_size_delegate il2cpp_gc_get_heap_size { get; }
        public il2cpp_gchandle_new_delegate il2cpp_gchandle_new { get; }
        public il2cpp_gchandle_get_target_delegate il2cpp_gchandle_get_target { get; }
        public il2cpp_unity_liveness_calculation_begin_delegate il2cpp_unity_liveness_calculation_begin { get; }
        public il2cpp_unity_liveness_calculation_from_root_delegate il2cpp_unity_liveness_calculation_from_root { get; }
        public il2cpp_method_get_return_type_delegate il2cpp_method_get_return_type { get; }
        public il2cpp_method_get_name_delegate il2cpp_method_get_name { get; }
        public il2cpp_method_get_object_delegate il2cpp_method_get_object { get; }
        public il2cpp_method_is_generic_delegate il2cpp_method_is_generic { get; }
        public il2cpp_method_is_inflated_delegate il2cpp_method_is_inflated { get; }
        public il2cpp_method_is_instance_delegate il2cpp_method_is_instance { get; }
        public il2cpp_method_get_param_count_delegate il2cpp_method_get_param_count { get; }
        public il2cpp_method_get_param_delegate il2cpp_method_get_param { get; }
        public il2cpp_method_has_attribute_delegate il2cpp_method_has_attribute { get; }
        public il2cpp_method_get_token_delegate il2cpp_method_get_token { get; }
        public il2cpp_profiler_install_delegate il2cpp_profiler_install { get; }
        public il2cpp_profiler_install_allocation_delegate il2cpp_profiler_install_allocation { get; }
        public il2cpp_profiler_install_fileio_delegate il2cpp_profiler_install_fileio { get; }
        public il2cpp_property_get_flags_delegate il2cpp_property_get_flags { get; }
        public il2cpp_property_get_set_method_delegate il2cpp_property_get_set_method { get; }
        public il2cpp_property_get_parent_delegate il2cpp_property_get_parent { get; }
        public il2cpp_object_get_size_delegate il2cpp_object_get_size { get; }
        public il2cpp_object_new_delegate il2cpp_object_new { get; }
        public il2cpp_object_unbox_delegate il2cpp_object_unbox { get; }
        public il2cpp_value_box_delegate il2cpp_value_box { get; }
        public il2cpp_monitor_try_enter_delegate il2cpp_monitor_try_enter { get; }
        public il2cpp_monitor_pulse_delegate il2cpp_monitor_pulse { get; }
        public il2cpp_monitor_wait_delegate il2cpp_monitor_wait { get; }
        public il2cpp_monitor_try_wait_delegate il2cpp_monitor_try_wait { get; }
        public il2cpp_runtime_invoke_delegate il2cpp_runtime_invoke { get; }
        public il2cpp_runtime_invoke_convert_args_delegate il2cpp_runtime_invoke_convert_args { get; }
        public il2cpp_runtime_object_init_delegate il2cpp_runtime_object_init { get; }
        public il2cpp_string_chars_delegate il2cpp_string_chars { get; }
        public il2cpp_string_new_len_delegate il2cpp_string_new_len { get; }
        public il2cpp_string_new_wrapper_delegate il2cpp_string_new_wrapper { get; }
        public il2cpp_string_is_interned_delegate il2cpp_string_is_interned { get; }
        public il2cpp_thread_attach_delegate il2cpp_thread_attach { get; }
        public il2cpp_thread_get_all_attached_threads_delegate il2cpp_thread_get_all_attached_threads { get; }
        public il2cpp_is_vm_thread_delegate il2cpp_is_vm_thread { get; }
        public il2cpp_thread_walk_frame_stack_delegate il2cpp_thread_walk_frame_stack { get; }
        public il2cpp_current_thread_get_top_frame_delegate il2cpp_current_thread_get_top_frame { get; }
        public il2cpp_thread_get_top_frame_delegate il2cpp_thread_get_top_frame { get; }
        public il2cpp_current_thread_get_frame_at_delegate il2cpp_current_thread_get_frame_at { get; }
        public il2cpp_thread_get_frame_at_delegate il2cpp_thread_get_frame_at { get; }
        public il2cpp_thread_get_stack_depth_delegate il2cpp_thread_get_stack_depth { get; }
        public il2cpp_type_get_type_delegate il2cpp_type_get_type { get; }
        public il2cpp_type_get_name_delegate il2cpp_type_get_name { get; }
        public il2cpp_type_is_byref_delegate il2cpp_type_is_byref { get; }
        public il2cpp_type_equals_delegate il2cpp_type_equals { get; }
        public il2cpp_image_get_assembly_delegate il2cpp_image_get_assembly { get; }
        public il2cpp_image_get_name_delegate il2cpp_image_get_name { get; }
        public il2cpp_image_get_filename_delegate il2cpp_image_get_filename { get; }
        public il2cpp_image_get_class_count_delegate il2cpp_image_get_class_count { get; }
        public il2cpp_capture_memory_snapshot_delegate il2cpp_capture_memory_snapshot { get; }
        public il2cpp_set_find_plugin_callback_delegate il2cpp_set_find_plugin_callback { get; }
        public il2cpp_debugger_set_agent_options_delegate il2cpp_debugger_set_agent_options { get; }
        public il2cpp_is_debugger_attached_delegate il2cpp_is_debugger_attached { get; }
        public il2cpp_custom_attrs_from_class_delegate il2cpp_custom_attrs_from_class { get; }
        public il2cpp_custom_attrs_get_attr_delegate il2cpp_custom_attrs_get_attr { get; }
        public il2cpp_custom_attrs_has_attr_delegate il2cpp_custom_attrs_has_attr { get; }
        public il2cpp_custom_attrs_free_delegate il2cpp_custom_attrs_free { get; }

        #endregion
    }
}
#pragma warning restore IDE1006 // Naming Styles