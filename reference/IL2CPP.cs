using System.Runtime.InteropServices;

namespace xarsu.Reference;

#pragma warning disable IDE1006 // Naming Styles: uses the original export names
public static unsafe partial class IL2CPP
{
    private static readonly Dictionary<string, IntPtr> _imageMap = [];
    private static readonly IntPtr _handle;
    private static readonly Delegates _exports;

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

    public static void il2cpp_init(nint domain_name) => _exports.il2cpp_init(domain_name);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_shutdown() => _exports.il2cpp_shutdown();
    public static void il2cpp_set_config_dir(nint config_path) => _exports.il2cpp_set_config_dir(config_path);
    public static void il2cpp_set_data_dir(nint data_path) => _exports.il2cpp_set_data_dir(data_path);
    public static void il2cpp_set_commandline_arguments(int argc, nint argv, nint basedir) => _exports.il2cpp_set_commandline_arguments(argc, argv, basedir);
    public static void il2cpp_set_memory_callbacks(IntPtr callbacks) => _exports.il2cpp_set_memory_callbacks(callbacks);
    public static IntPtr il2cpp_get_corlib() => _exports.il2cpp_get_corlib();
    public static void il2cpp_add_internal_call(nint name, IntPtr method) => _exports.il2cpp_add_internal_call(name, method);
    public static IntPtr il2cpp_resolve_icall(string name) => _exports.il2cpp_resolve_icall(Marshal.StringToCoTaskMemUTF8(name));
    public static void* il2cpp_alloc(ref ulong size) => _exports.il2cpp_alloc(ref size);
    public static void il2cpp_free(void* ptr) => _exports.il2cpp_free(ptr);
    public static IntPtr il2cpp_array_class_get(IntPtr element_class, uint rank) => _exports.il2cpp_array_class_get(element_class, rank);
    public static uint il2cpp_array_length(IntPtr array) => _exports.il2cpp_array_length(array);
    public static uint il2cpp_array_get_byte_length(IntPtr array) => _exports.il2cpp_array_get_byte_length(array);
    public static IntPtr il2cpp_array_new(IntPtr elementIl2CppClass, uint length) => _exports.il2cpp_array_new(elementIl2CppClass, length);
    public static IntPtr il2cpp_array_new_specific(IntPtr arrayIl2CppClass, ulong length) => _exports.il2cpp_array_new_specific(arrayIl2CppClass, length);
    public static IntPtr il2cpp_array_new_full(IntPtr array_class, ulong* lengths, ulong* lower_bounds) => _exports.il2cpp_array_new_full(array_class, lengths, lower_bounds);
    public static IntPtr il2cpp_bounded_array_class_get(IntPtr element_class, uint rank, bool bounded) => _exports.il2cpp_bounded_array_class_get(element_class, rank, bounded);
    public static int il2cpp_array_element_size(IntPtr array_class) => _exports.il2cpp_array_element_size(array_class);
    public static IntPtr il2cpp_assembly_get_image(IntPtr assembly) => _exports.il2cpp_assembly_get_image(assembly);
    public static IntPtr il2cpp_class_enum_basetype(IntPtr klass) => _exports.il2cpp_class_enum_basetype(klass);
    public static bool il2cpp_class_is_generic(IntPtr klass) => _exports.il2cpp_class_is_generic(klass);
    public static bool il2cpp_class_is_inflated(IntPtr klass) => _exports.il2cpp_class_is_inflated(klass);
    public static bool il2cpp_class_is_assignable_from(IntPtr klass, IntPtr oklass) => _exports.il2cpp_class_is_assignable_from(klass, oklass);
    public static bool il2cpp_class_is_subclass_of(IntPtr klass, IntPtr klassc, bool check_interfaces) => _exports.il2cpp_class_is_subclass_of(klass, klassc, check_interfaces);
    public static bool il2cpp_class_has_parent(IntPtr klass, IntPtr klassc) => _exports.il2cpp_class_has_parent(klass, klassc);
    public static IntPtr il2cpp_class_from_il2cpp_type(IntPtr type) => _exports.il2cpp_class_from_il2cpp_type(type);
    public static IntPtr il2cpp_class_from_name(IntPtr image, string namespaze, string name) => _exports.il2cpp_class_from_name(image, Marshal.StringToCoTaskMemUTF8(namespaze), Marshal.StringToCoTaskMemUTF8(name));
    public static IntPtr il2cpp_class_from_system_type(IntPtr type) => _exports.il2cpp_class_from_system_type(type);
    public static IntPtr il2cpp_class_get_element_class(IntPtr klass) => _exports.il2cpp_class_get_element_class(klass);
    public static IntPtr il2cpp_class_get_events(IntPtr klass, void** iter) => _exports.il2cpp_class_get_events(klass, iter);
    public static IntPtr il2cpp_class_get_fields(IntPtr klass, void** iter) => _exports.il2cpp_class_get_fields(klass, iter);
    public static IntPtr il2cpp_class_get_interfaces(IntPtr klass, void** iter) => _exports.il2cpp_class_get_interfaces(klass, iter);
    public static IntPtr il2cpp_class_get_properties(IntPtr klass, void** iter) => _exports.il2cpp_class_get_properties(klass, iter);
    public static IntPtr il2cpp_class_get_property_from_name(IntPtr klass, nint name) => _exports.il2cpp_class_get_property_from_name(klass, name);
    public static IntPtr il2cpp_class_get_field_from_name(IntPtr klass, string name) => _exports.il2cpp_class_get_field_from_name(klass, Marshal.StringToCoTaskMemUTF8(name));
    public static IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter) => _exports.il2cpp_class_get_methods(klass, ref iter);
    public static IntPtr il2cpp_class_get_method_from_name(IntPtr klass, nint name, int argsCount) => _exports.il2cpp_class_get_method_from_name(klass, name, argsCount);
    public static string? il2cpp_class_get_name(IntPtr klass) => Marshal.PtrToStringUTF8(_exports.il2cpp_class_get_name(klass));
    public static string? il2cpp_class_get_namespace(IntPtr klass) => Marshal.PtrToStringUTF8(_exports.il2cpp_class_get_namespace(klass));
    public static IntPtr il2cpp_class_get_parent(IntPtr klass) => _exports.il2cpp_class_get_parent(klass);
    public static IntPtr il2cpp_class_get_declaring_type(IntPtr klass) => _exports.il2cpp_class_get_declaring_type(klass);
    public static int il2cpp_class_instance_size(IntPtr klass) => _exports.il2cpp_class_instance_size(klass);
    public static ulong il2cpp_class_num_fields(IntPtr enumKlass) => _exports.il2cpp_class_num_fields(enumKlass);
    public static bool il2cpp_class_is_valuetype(IntPtr klass) => _exports.il2cpp_class_is_valuetype(klass);
    public static int il2cpp_class_value_size(IntPtr klass, ref uint align) => _exports.il2cpp_class_value_size(klass, ref align);
    public static int il2cpp_class_get_flags(IntPtr klass) => _exports.il2cpp_class_get_flags(klass);
    public static bool il2cpp_class_is_abstract(IntPtr klass) => _exports.il2cpp_class_is_abstract(klass);
    public static bool il2cpp_class_is_interface(IntPtr klass) => _exports.il2cpp_class_is_interface(klass);
    public static int il2cpp_class_array_element_size(IntPtr klass) => _exports.il2cpp_class_array_element_size(klass);
    public static IntPtr il2cpp_class_from_type(IntPtr type) => _exports.il2cpp_class_from_type(type);
    public static IntPtr il2cpp_class_get_type(IntPtr klass) => _exports.il2cpp_class_get_type(klass);
    public static bool il2cpp_class_has_attribute(IntPtr klass, IntPtr attr_class) => _exports.il2cpp_class_has_attribute(klass, attr_class);
    public static bool il2cpp_class_has_references(IntPtr klass) => _exports.il2cpp_class_has_references(klass);
    public static bool il2cpp_class_is_enum(IntPtr klass) => _exports.il2cpp_class_is_enum(klass);
    public static IntPtr il2cpp_class_get_image(IntPtr klass) => _exports.il2cpp_class_get_image(klass);
    public static string? il2cpp_class_get_assemblyname(IntPtr klass) => Marshal.PtrToStringUTF8(_exports.il2cpp_class_get_assemblyname(klass));
    public static ulong il2cpp_class_get_bitmap_size(IntPtr klass) => _exports.il2cpp_class_get_bitmap_size(klass);
    public static void il2cpp_class_get_bitmap(IntPtr klass, ulong bitmap) => _exports.il2cpp_class_get_bitmap(klass, bitmap);
    public static bool il2cpp_stats_dump_to_file(nint path) => _exports.il2cpp_stats_dump_to_file(path);
    public static ulong il2cpp_stats_get_value(int stat) => _exports.il2cpp_stats_get_value(stat);
    public static IntPtr il2cpp_domain_get() => _exports.il2cpp_domain_get();
    public static IntPtr il2cpp_domain_assembly_open(IntPtr domain, nint name) => _exports.il2cpp_domain_assembly_open(domain, name);
    public static IntPtr* il2cpp_domain_get_assemblies(IntPtr domain, ref uint size) => _exports.il2cpp_domain_get_assemblies(domain, ref size);
    public static void il2cpp_raise_exception(IntPtr arg0) => _exports.il2cpp_raise_exception(arg0);
    public static IntPtr il2cpp_exception_from_name_msg(IntPtr image, nint name_space, nint name, nint msg) => _exports.il2cpp_exception_from_name_msg(image, name_space, name, msg);
    public static IntPtr il2cpp_get_exception_argument_null(nint arg) => _exports.il2cpp_get_exception_argument_null(arg);
    public static void il2cpp_format_exception(IntPtr ex, nint message, int message_size) => _exports.il2cpp_format_exception(ex, message, message_size);
    public static void il2cpp_format_stack_trace(IntPtr ex, nint output, int output_size) => _exports.il2cpp_format_stack_trace(ex, output, output_size);
    public static void il2cpp_unhandled_exception(IntPtr arg0) => _exports.il2cpp_unhandled_exception(arg0);
    public static int il2cpp_field_get_flags(IntPtr field) => _exports.il2cpp_field_get_flags(field);
    public static string? il2cpp_field_get_name(IntPtr field) => Marshal.PtrToStringUTF8(_exports.il2cpp_field_get_name(field));
    public static IntPtr il2cpp_field_get_parent(IntPtr field) => _exports.il2cpp_field_get_parent(field);
    public static ulong il2cpp_field_get_offset(IntPtr field) => _exports.il2cpp_field_get_offset(field);
    public static IntPtr il2cpp_field_get_type(IntPtr field) => _exports.il2cpp_field_get_type(field);
    public static void il2cpp_field_get_value(IntPtr obj, IntPtr field, void* value) => _exports.il2cpp_field_get_value(obj, field, value);
    public static IntPtr il2cpp_field_get_value_object(IntPtr field, IntPtr obj) => _exports.il2cpp_field_get_value_object(field, obj);
    public static bool il2cpp_field_has_attribute(IntPtr field, IntPtr attr_class) => _exports.il2cpp_field_has_attribute(field, attr_class);
    public static void il2cpp_field_set_value(IntPtr obj, IntPtr field, void* value) => _exports.il2cpp_field_set_value(obj, field, value);
    public static void il2cpp_field_static_get_value(IntPtr field, void* value) => _exports.il2cpp_field_static_get_value(field, value);
    public static void il2cpp_field_static_set_value(IntPtr field, void* value) => _exports.il2cpp_field_static_set_value(field, value);
    public static void il2cpp_gc_collect(int maxGenerations) => _exports.il2cpp_gc_collect(maxGenerations);
    public static long il2cpp_gc_get_used_size() => _exports.il2cpp_gc_get_used_size();
    public static long il2cpp_gc_get_heap_size() => _exports.il2cpp_gc_get_heap_size();
    public static uint il2cpp_gchandle_new(IntPtr obj, bool pinned) => _exports.il2cpp_gchandle_new(obj, pinned);
    public static uint il2cpp_gchandle_new_weakref(IntPtr obj, bool track_resurrection) => _exports.il2cpp_gchandle_new_weakref(obj, track_resurrection);
    public static IntPtr il2cpp_gchandle_get_target(IntPtr gchandle) => _exports.il2cpp_gchandle_get_target(gchandle);
    public static void il2cpp_gchandle_free(uint gchandle) => _exports.il2cpp_gchandle_free(gchandle);
    public static void* il2cpp_unity_liveness_calculation_begin(IntPtr filter, int max_object_count, IntPtr callback, void* userdata, IntPtr onWorldStarted, IntPtr onWorldStopped) => _exports.il2cpp_unity_liveness_calculation_begin(filter, max_object_count, callback, userdata, onWorldStarted, onWorldStopped);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27)
    public static void il2cpp_unity_liveness_calculation_end(void* state) => _exports.il2cpp_unity_liveness_calculation_end(state);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27)
    public static void il2cpp_unity_liveness_calculation_from_root(IntPtr root, void* state) => _exports.il2cpp_unity_liveness_calculation_from_root(root, state);
    public static void il2cpp_unity_liveness_calculation_from_statics(void* state) => _exports.il2cpp_unity_liveness_calculation_from_statics(state);
    public static IntPtr il2cpp_method_get_return_type(IntPtr method) => _exports.il2cpp_method_get_return_type(method);
    public static IntPtr il2cpp_method_get_declaring_type(IntPtr method) => _exports.il2cpp_method_get_declaring_type(method);
    public static string? il2cpp_method_get_name(IntPtr method) => Marshal.PtrToStringUTF8(_exports.il2cpp_method_get_name(method));
    public static IntPtr il2cpp_method_get_object(IntPtr method, IntPtr refclass) => _exports.il2cpp_method_get_object(method, refclass);
    public static bool il2cpp_method_is_generic(IntPtr method) => _exports.il2cpp_method_is_generic(method);
    public static bool il2cpp_method_is_inflated(IntPtr method) => _exports.il2cpp_method_is_inflated(method);
    public static bool il2cpp_method_is_instance(IntPtr method) => _exports.il2cpp_method_is_instance(method);
    public static uint il2cpp_method_get_param_count(IntPtr method) => _exports.il2cpp_method_get_param_count(method);
    public static IntPtr il2cpp_method_get_param(IntPtr method, uint index) => _exports.il2cpp_method_get_param(method, index);
    public static IntPtr il2cpp_method_get_class(IntPtr method) => _exports.il2cpp_method_get_class(method);
    public static bool il2cpp_method_has_attribute(IntPtr method, IntPtr attr_class) => _exports.il2cpp_method_has_attribute(method, attr_class);
    public static uint il2cpp_method_get_flags(IntPtr method, uint iflags) => _exports.il2cpp_method_get_flags(method, iflags);
    public static uint il2cpp_method_get_token(IntPtr method) => _exports.il2cpp_method_get_token(method);
    public static string? il2cpp_method_get_param_name(IntPtr method, uint index) => Marshal.PtrToStringUTF8(_exports.il2cpp_method_get_param_name(method, index));
    public static void il2cpp_profiler_install(IntPtr prof, IntPtr shutdown_callback) => _exports.il2cpp_profiler_install(prof, shutdown_callback);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static void il2cpp_profiler_set_events(IntPtr events) => _exports.il2cpp_profiler_set_events(events);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static void il2cpp_profiler_install_enter_leave(IntPtr enter, IntPtr fleave) => _exports.il2cpp_profiler_install_enter_leave(enter, fleave);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static void il2cpp_profiler_install_allocation(IntPtr callback) => _exports.il2cpp_profiler_install_allocation(callback);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static void il2cpp_profiler_install_gc(IntPtr callback, IntPtr heap_resize_callback) => _exports.il2cpp_profiler_install_gc(callback, heap_resize_callback);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static uint il2cpp_property_get_flags(IntPtr prop) => _exports.il2cpp_property_get_flags(prop);
    public static IntPtr il2cpp_property_get_get_method(IntPtr prop) => _exports.il2cpp_property_get_get_method(prop);
    public static IntPtr il2cpp_property_get_set_method(IntPtr prop) => _exports.il2cpp_property_get_set_method(prop);
    public static string? il2cpp_property_get_name(IntPtr prop) => Marshal.PtrToStringUTF8(_exports.il2cpp_property_get_name(prop));
    public static IntPtr il2cpp_property_get_parent(IntPtr prop) => _exports.il2cpp_property_get_parent(prop);
    public static IntPtr il2cpp_object_get_class(IntPtr obj) => _exports.il2cpp_object_get_class(obj);
    public static uint il2cpp_object_get_size(IntPtr obj) => _exports.il2cpp_object_get_size(obj);
    public static IntPtr il2cpp_object_get_virtual_method(IntPtr obj, IntPtr method) => _exports.il2cpp_object_get_virtual_method(obj, method);
    public static IntPtr il2cpp_object_new(IntPtr klass) => _exports.il2cpp_object_new(klass);
    public static void* il2cpp_object_unbox(IntPtr obj) => _exports.il2cpp_object_unbox(obj);
    public static IntPtr il2cpp_value_box(IntPtr klass, IntPtr data) => _exports.il2cpp_value_box(klass, data);
    public static void il2cpp_monitor_enter(IntPtr obj) => _exports.il2cpp_monitor_enter(obj);
    public static bool il2cpp_monitor_try_enter(IntPtr obj, uint timeout) => _exports.il2cpp_monitor_try_enter(obj, timeout);
    public static void il2cpp_monitor_exit(IntPtr obj) => _exports.il2cpp_monitor_exit(obj);
    public static void il2cpp_monitor_pulse(IntPtr obj) => _exports.il2cpp_monitor_pulse(obj);
    public static void il2cpp_monitor_pulse_all(IntPtr obj) => _exports.il2cpp_monitor_pulse_all(obj);
    public static void il2cpp_monitor_wait(IntPtr obj) => _exports.il2cpp_monitor_wait(obj);
    public static bool il2cpp_monitor_try_wait(IntPtr obj, uint timeout) => _exports.il2cpp_monitor_try_wait(obj, timeout);
    public static IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc) => _exports.il2cpp_runtime_invoke(method, obj, param, ref exc);
    public static IntPtr il2cpp_runtime_invoke_convert_args(IntPtr method, void* obj, void** param, int paramCount, void** exc) => _exports.il2cpp_runtime_invoke_convert_args(method, obj, param, paramCount, exc);
    public static void il2cpp_runtime_class_init(IntPtr klass) => _exports.il2cpp_runtime_class_init(klass);
    public static void il2cpp_runtime_object_init(IntPtr obj) => _exports.il2cpp_runtime_object_init(obj);
    public static void il2cpp_runtime_object_init_exception(IntPtr obj, void** exc) => _exports.il2cpp_runtime_object_init_exception(obj, exc);
    public static void il2cpp_runtime_unhandled_exception_policy_set(int value) => _exports.il2cpp_runtime_unhandled_exception_policy_set(value);
    public static IntPtr il2cpp_delegate_begin_invoke(IntPtr del, void** param, IntPtr asyncCallback, IntPtr state) => _exports.il2cpp_delegate_begin_invoke(del, param, asyncCallback, state);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static IntPtr il2cpp_delegate_end_invoke(IntPtr asyncResult, void** out_args) => _exports.il2cpp_delegate_end_invoke(asyncResult, out_args);
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21)
    public static int il2cpp_string_length(IntPtr str) => _exports.il2cpp_string_length(str);
    public static char* il2cpp_string_chars(IntPtr str) => (char*)_exports.il2cpp_string_chars(str);
    public static IntPtr il2cpp_string_new(nint str) => _exports.il2cpp_string_new(str);
    public static IntPtr il2cpp_string_new_len(nint str, uint length) => _exports.il2cpp_string_new_len(str, length);
    public static IntPtr il2cpp_string_new_utf16(char* text, int len) => _exports.il2cpp_string_new_utf16((nint)text, len);
    public static IntPtr il2cpp_string_new_wrapper(nint str) => _exports.il2cpp_string_new_wrapper(str);
    public static IntPtr il2cpp_string_intern(IntPtr str) => _exports.il2cpp_string_intern(str);
    public static IntPtr il2cpp_string_is_interned(IntPtr str) => _exports.il2cpp_string_is_interned(str);
    public static string? il2cpp_thread_get_name(IntPtr thread, uint len) => Marshal.PtrToStringUTF8(_exports.il2cpp_thread_get_name(thread, len));
    // ⚠ Available from IL2CPP v16+ (16, 18, 19, 20, 21, 22, 23, 24)
    public static IntPtr il2cpp_thread_current() => _exports.il2cpp_thread_current();
    public static IntPtr il2cpp_thread_attach(IntPtr domain) => _exports.il2cpp_thread_attach(domain);
    public static void il2cpp_thread_detach(IntPtr thread) => _exports.il2cpp_thread_detach(thread);
    public static void** il2cpp_thread_get_all_attached_threads(ref ulong size) => _exports.il2cpp_thread_get_all_attached_threads(ref size);
    public static bool il2cpp_is_vm_thread(IntPtr thread) => _exports.il2cpp_is_vm_thread(thread);
    public static void il2cpp_current_thread_walk_frame_stack(IntPtr func, void* user_data) => _exports.il2cpp_current_thread_walk_frame_stack(func, user_data);
    public static void il2cpp_thread_walk_frame_stack(IntPtr thread, IntPtr func, void* user_data) => _exports.il2cpp_thread_walk_frame_stack(thread, func, user_data);
    public static bool il2cpp_current_thread_get_top_frame(IntPtr frame) => _exports.il2cpp_current_thread_get_top_frame(frame);
    public static bool il2cpp_thread_get_top_frame(IntPtr thread, IntPtr frame) => _exports.il2cpp_thread_get_top_frame(thread, frame);
    public static bool il2cpp_current_thread_get_frame_at(int offset, IntPtr frame) => _exports.il2cpp_current_thread_get_frame_at(offset, frame);
    public static bool il2cpp_thread_get_frame_at(IntPtr thread, int offset, IntPtr frame) => _exports.il2cpp_thread_get_frame_at(thread, offset, frame);
    public static int il2cpp_current_thread_get_stack_depth() => _exports.il2cpp_current_thread_get_stack_depth();
    public static int il2cpp_thread_get_stack_depth(IntPtr thread) => _exports.il2cpp_thread_get_stack_depth(thread);
    public static IntPtr il2cpp_type_get_object(IntPtr type) => _exports.il2cpp_type_get_object(type);
    public static int il2cpp_type_get_type(IntPtr type) => _exports.il2cpp_type_get_type(type);
    public static IntPtr il2cpp_type_get_class_or_element_class(IntPtr type) => _exports.il2cpp_type_get_class_or_element_class(type);
    public static string? il2cpp_type_get_name(IntPtr type) => Marshal.PtrToStringUTF8(_exports.il2cpp_type_get_name(type));
    public static IntPtr il2cpp_image_get_assembly(IntPtr image) => _exports.il2cpp_image_get_assembly(image);
    public static string? il2cpp_image_get_name(IntPtr image) => Marshal.PtrToStringUTF8(_exports.il2cpp_image_get_name(image));
    public static string? il2cpp_image_get_filename(IntPtr image) => Marshal.PtrToStringUTF8(_exports.il2cpp_image_get_filename(image));
    public static IntPtr il2cpp_image_get_entry_point(IntPtr image) => _exports.il2cpp_image_get_entry_point(image);
    public static IntPtr il2cpp_capture_memory_snapshot() => _exports.il2cpp_capture_memory_snapshot();
    public static void il2cpp_free_captured_memory_snapshot(IntPtr snapshot) => _exports.il2cpp_free_captured_memory_snapshot(snapshot);
    public static void il2cpp_set_find_plugin_callback(IntPtr method) => _exports.il2cpp_set_find_plugin_callback(method);
    // ⚠ Available from IL2CPP v18+ (18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static IntPtr il2cpp_class_get_nested_types(IntPtr klass, ref IntPtr iter) => _exports.il2cpp_class_get_nested_types(klass, ref iter);
    // ⚠ Available from IL2CPP v19+ (19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static int il2cpp_gc_collect_a_little() => _exports.il2cpp_gc_collect_a_little();
    // ⚠ Available from IL2CPP v21+ (21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_gc_disable() => _exports.il2cpp_gc_disable();
    // ⚠ Available from IL2CPP v21+ (21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_gc_enable() => _exports.il2cpp_gc_enable();
    // ⚠ Available from IL2CPP v21+ (21, 22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_init_utf16(IntPtr domain_name) => _exports.il2cpp_init_utf16(domain_name);
    // ⚠ Available from IL2CPP v22+ (22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_set_commandline_arguments_utf16(int argc, IntPtr argv, nint basedir) => _exports.il2cpp_set_commandline_arguments_utf16(argc, argv, basedir);
    // ⚠ Available from IL2CPP v22+ (22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_set_config_utf16(IntPtr executablePath) => _exports.il2cpp_set_config_utf16(executablePath);
    // ⚠ Available from IL2CPP v22+ (22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_set_config(nint executablePath) => _exports.il2cpp_set_config(executablePath);
    // ⚠ Available from IL2CPP v22+ (22, 23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_field_set_value_object(IntPtr instance, IntPtr field, IntPtr value) => _exports.il2cpp_field_set_value_object(instance, field, value);
    // ⚠ Available from IL2CPP v23+ (23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_register_log_callback(IntPtr method) => _exports.il2cpp_register_log_callback(method);
    // ⚠ Available from IL2CPP v23+ (23, 24, 25, 26, 27, 28, 29)
    public static void il2cpp_set_temp_dir(nint temp_path) => _exports.il2cpp_set_temp_dir(temp_path);
    // ⚠ Available from IL2CPP v24+ (24, 25, 26, 27, 28, 29)
    public static bool il2cpp_class_is_blittable(IntPtr klass) => _exports.il2cpp_class_is_blittable(klass);
    // ⚠ Available from IL2CPP v24+ (24, 25, 26, 27, 28, 29)
    public static void il2cpp_class_for_each(IntPtr arg0) => _exports.il2cpp_class_for_each(arg0);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static string? il2cpp_type_get_name_chunked(IntPtr type, IntPtr arg1) => Marshal.PtrToStringUTF8(_exports.il2cpp_type_get_name_chunked(type, arg1));
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_class_get_type_token(IntPtr klass) => _exports.il2cpp_class_get_type_token(klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static int il2cpp_class_get_rank(IntPtr klass) => _exports.il2cpp_class_get_rank(klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_class_get_data_size(IntPtr klass) => _exports.il2cpp_class_get_data_size(klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void* il2cpp_class_get_static_field_data(IntPtr klass) => _exports.il2cpp_class_get_static_field_data(klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_native_stack_trace(IntPtr ex, ulong** addresses, int numFrames, nint imageUUID) => _exports.il2cpp_native_stack_trace(ex, addresses, numFrames, imageUUID);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_field_is_literal(IntPtr field) => _exports.il2cpp_field_is_literal(field);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_gc_is_disabled() => _exports.il2cpp_gc_is_disabled();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static long il2cpp_gc_get_max_time_slice_ns() => _exports.il2cpp_gc_get_max_time_slice_ns();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_set_max_time_slice_ns(long maxTimeSlice) => _exports.il2cpp_gc_set_max_time_slice_ns(maxTimeSlice);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_gc_is_incremental() => _exports.il2cpp_gc_is_incremental();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_wbarrier_set_field(IntPtr obj, IntPtr targetAddress, IntPtr objec) => _exports.il2cpp_gc_wbarrier_set_field(obj, targetAddress, objec);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_gc_has_strict_wbarriers() => _exports.il2cpp_gc_has_strict_wbarriers();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_set_external_allocation_tracker(IntPtr arg0) => _exports.il2cpp_gc_set_external_allocation_tracker(arg0);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_set_external_wbarrier_tracker(IntPtr arg0) => _exports.il2cpp_gc_set_external_wbarrier_tracker(arg0);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_foreach_heap(IntPtr arg0) => _exports.il2cpp_gc_foreach_heap(arg0);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_stop_gc_world() => _exports.il2cpp_stop_gc_world();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_start_gc_world() => _exports.il2cpp_start_gc_world();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gchandle_foreach_get_target(IntPtr arg0) => _exports.il2cpp_gchandle_foreach_get_target(arg0);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_object_header_size() => _exports.il2cpp_object_header_size();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_array_object_header_size() => _exports.il2cpp_array_object_header_size();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_offset_of_array_length_in_array_object_header() => _exports.il2cpp_offset_of_array_length_in_array_object_header();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_offset_of_array_bounds_in_array_object_header() => _exports.il2cpp_offset_of_array_bounds_in_array_object_header();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_allocation_granularity() => _exports.il2cpp_allocation_granularity();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_method_get_from_reflection(IntPtr method) => _exports.il2cpp_method_get_from_reflection(method);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_override_stack_backtrace(IntPtr stackBacktraceFunc) => _exports.il2cpp_override_stack_backtrace(stackBacktraceFunc);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_type_is_byref(IntPtr type) => _exports.il2cpp_type_is_byref(type);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static uint il2cpp_type_get_attrs(IntPtr type) => _exports.il2cpp_type_get_attrs(type);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_type_equals(IntPtr type, IntPtr otherType) => _exports.il2cpp_type_equals(type, otherType);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static string? il2cpp_type_get_assembly_qualified_name(IntPtr type) => Marshal.PtrToStringUTF8(_exports.il2cpp_type_get_assembly_qualified_name(type));
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_type_is_static(IntPtr type) => _exports.il2cpp_type_is_static(type);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_type_is_pointer_type(IntPtr type) => _exports.il2cpp_type_is_pointer_type(type);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static ulong il2cpp_image_get_class_count(IntPtr image) => _exports.il2cpp_image_get_class_count(image);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_image_get_class(IntPtr image, ulong index) => _exports.il2cpp_image_get_class(image, index);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_debugger_set_agent_options(nint options) => _exports.il2cpp_debugger_set_agent_options(options);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_is_debugger_attached() => _exports.il2cpp_is_debugger_attached();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_register_debugger_agent_transport(IntPtr debuggerTransport) => _exports.il2cpp_register_debugger_agent_transport(debuggerTransport);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_debug_get_method_info(IntPtr arg0, IntPtr methodDebugInfo) => _exports.il2cpp_debug_get_method_info(arg0, methodDebugInfo);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct) => _exports.il2cpp_unity_install_unitytls_interface(unitytlsInterfaceStruct);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_custom_attrs_from_class(IntPtr klass) => _exports.il2cpp_custom_attrs_from_class(klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_custom_attrs_from_method(IntPtr method) => _exports.il2cpp_custom_attrs_from_method(method);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_custom_attrs_get_attr(IntPtr ainfo, IntPtr attr_klass) => _exports.il2cpp_custom_attrs_get_attr(ainfo, attr_klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static bool il2cpp_custom_attrs_has_attr(IntPtr ainfo, IntPtr attr_klass) => _exports.il2cpp_custom_attrs_has_attr(ainfo, attr_klass);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static IntPtr il2cpp_custom_attrs_construct(IntPtr cinfo) => _exports.il2cpp_custom_attrs_construct(cinfo);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_custom_attrs_free(IntPtr ainfo) => _exports.il2cpp_custom_attrs_free(ainfo);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_class_set_userdata(IntPtr klass, void* userdata) => _exports.il2cpp_class_set_userdata(klass, userdata);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static int il2cpp_class_get_userdata_offset() => _exports.il2cpp_class_get_userdata_offset();
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_set_default_thread_affinity(long affinity_mask) => _exports.il2cpp_set_default_thread_affinity(affinity_mask);
    // ⚠ Available from IL2CPP v25+ (25, 26, 27, 28, 29)
    public static void il2cpp_gc_start_incremental_collection() => _exports.il2cpp_gc_start_incremental_collection();
    // ⚠ Available from IL2CPP v27+ (27, 28, 29)
    public static void il2cpp_gc_set_mode(int mode) => _exports.il2cpp_gc_set_mode(mode);
    // ⚠ Available from IL2CPP v27+ (27, 28, 29)
    public static void* il2cpp_unity_liveness_allocate_struct(IntPtr filter, int max_object_count, IntPtr callback, void* userdata, IntPtr reallocate) => _exports.il2cpp_unity_liveness_allocate_struct(filter, max_object_count, callback, userdata, reallocate);
    // ⚠ Available from IL2CPP v28+ (28, 29)
    public static void il2cpp_unity_liveness_finalize(void* state) => _exports.il2cpp_unity_liveness_finalize(state);
    // ⚠ Available from IL2CPP v28+ (28, 29)
    public static void il2cpp_unity_liveness_free_struct(void* state) => _exports.il2cpp_unity_liveness_free_struct(state);
    // ⚠ Available from IL2CPP v28+ (28, 29)
    public static void* il2cpp_gc_alloc_fixed(ref ulong size) => _exports.il2cpp_gc_alloc_fixed(ref size);
    // ⚠ Available from IL2CPP v29+ (29)
    public static void il2cpp_gc_free_fixed(void* address) => _exports.il2cpp_gc_free_fixed(address);
    // ⚠ Available from IL2CPP v29+ (29)

    private class Delegates
    {

        public Delegates(IntPtr handle)
        {
            #region Load Exports

            il2cpp_init = NativeLibraryUtil.LoadFunction<il2cpp_init_delegate>(handle, "il2cpp_init"); // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_shutdown = NativeLibraryUtil.LoadFunction<il2cpp_shutdown_delegate>(handle, "il2cpp_shutdown"); // IL2CPP Versions: All
            il2cpp_set_config_dir = NativeLibraryUtil.LoadFunction<il2cpp_set_config_dir_delegate>(handle, "il2cpp_set_config_dir"); // IL2CPP Versions: All
            il2cpp_set_data_dir = NativeLibraryUtil.LoadFunction<il2cpp_set_data_dir_delegate>(handle, "il2cpp_set_data_dir"); // IL2CPP Versions: All
            il2cpp_set_commandline_arguments = NativeLibraryUtil.LoadFunction<il2cpp_set_commandline_arguments_delegate>(handle, "il2cpp_set_commandline_arguments"); // IL2CPP Versions: All
            il2cpp_set_memory_callbacks = NativeLibraryUtil.LoadFunction<il2cpp_set_memory_callbacks_delegate>(handle, "il2cpp_set_memory_callbacks"); // IL2CPP Versions: All
            il2cpp_get_corlib = NativeLibraryUtil.LoadFunction<il2cpp_get_corlib_delegate>(handle, "il2cpp_get_corlib"); // IL2CPP Versions: All
            il2cpp_add_internal_call = NativeLibraryUtil.LoadFunction<il2cpp_add_internal_call_delegate>(handle, "il2cpp_add_internal_call"); // IL2CPP Versions: All
            il2cpp_resolve_icall = NativeLibraryUtil.LoadFunction<il2cpp_resolve_icall_delegate>(handle, "il2cpp_resolve_icall"); // IL2CPP Versions: All
            il2cpp_alloc = NativeLibraryUtil.LoadFunction<il2cpp_alloc_delegate>(handle, "il2cpp_alloc"); // IL2CPP Versions: All
            il2cpp_free = NativeLibraryUtil.LoadFunction<il2cpp_free_delegate>(handle, "il2cpp_free"); // IL2CPP Versions: All
            il2cpp_array_class_get = NativeLibraryUtil.LoadFunction<il2cpp_array_class_get_delegate>(handle, "il2cpp_array_class_get"); // IL2CPP Versions: All
            il2cpp_array_length = NativeLibraryUtil.LoadFunction<il2cpp_array_length_delegate>(handle, "il2cpp_array_length"); // IL2CPP Versions: All
            il2cpp_array_get_byte_length = NativeLibraryUtil.LoadFunction<il2cpp_array_get_byte_length_delegate>(handle, "il2cpp_array_get_byte_length"); // IL2CPP Versions: All
            il2cpp_array_new = NativeLibraryUtil.LoadFunction<il2cpp_array_new_delegate>(handle, "il2cpp_array_new"); // IL2CPP Versions: All
            il2cpp_array_new_specific = NativeLibraryUtil.LoadFunction<il2cpp_array_new_specific_delegate>(handle, "il2cpp_array_new_specific"); // IL2CPP Versions: All
            il2cpp_array_new_full = NativeLibraryUtil.LoadFunction<il2cpp_array_new_full_delegate>(handle, "il2cpp_array_new_full"); // IL2CPP Versions: All
            il2cpp_bounded_array_class_get = NativeLibraryUtil.LoadFunction<il2cpp_bounded_array_class_get_delegate>(handle, "il2cpp_bounded_array_class_get"); // IL2CPP Versions: All
            il2cpp_array_element_size = NativeLibraryUtil.LoadFunction<il2cpp_array_element_size_delegate>(handle, "il2cpp_array_element_size"); // IL2CPP Versions: All
            il2cpp_assembly_get_image = NativeLibraryUtil.LoadFunction<il2cpp_assembly_get_image_delegate>(handle, "il2cpp_assembly_get_image"); // IL2CPP Versions: All
            il2cpp_class_enum_basetype = NativeLibraryUtil.LoadFunction<il2cpp_class_enum_basetype_delegate>(handle, "il2cpp_class_enum_basetype"); // IL2CPP Versions: All
            il2cpp_class_is_generic = NativeLibraryUtil.LoadFunction<il2cpp_class_is_generic_delegate>(handle, "il2cpp_class_is_generic"); // IL2CPP Versions: All
            il2cpp_class_is_inflated = NativeLibraryUtil.LoadFunction<il2cpp_class_is_inflated_delegate>(handle, "il2cpp_class_is_inflated"); // IL2CPP Versions: All
            il2cpp_class_is_assignable_from = NativeLibraryUtil.LoadFunction<il2cpp_class_is_assignable_from_delegate>(handle, "il2cpp_class_is_assignable_from"); // IL2CPP Versions: All
            il2cpp_class_is_subclass_of = NativeLibraryUtil.LoadFunction<il2cpp_class_is_subclass_of_delegate>(handle, "il2cpp_class_is_subclass_of"); // IL2CPP Versions: All
            il2cpp_class_has_parent = NativeLibraryUtil.LoadFunction<il2cpp_class_has_parent_delegate>(handle, "il2cpp_class_has_parent"); // IL2CPP Versions: All
            il2cpp_class_from_il2cpp_type = NativeLibraryUtil.LoadFunction<il2cpp_class_from_il2cpp_type_delegate>(handle, "il2cpp_class_from_il2cpp_type"); // IL2CPP Versions: All
            il2cpp_class_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_from_name_delegate>(handle, "il2cpp_class_from_name"); // IL2CPP Versions: All
            il2cpp_class_from_system_type = NativeLibraryUtil.LoadFunction<il2cpp_class_from_system_type_delegate>(handle, "il2cpp_class_from_system_type"); // IL2CPP Versions: All
            il2cpp_class_get_element_class = NativeLibraryUtil.LoadFunction<il2cpp_class_get_element_class_delegate>(handle, "il2cpp_class_get_element_class"); // IL2CPP Versions: All
            il2cpp_class_get_events = NativeLibraryUtil.LoadFunction<il2cpp_class_get_events_delegate>(handle, "il2cpp_class_get_events"); // IL2CPP Versions: All
            il2cpp_class_get_fields = NativeLibraryUtil.LoadFunction<il2cpp_class_get_fields_delegate>(handle, "il2cpp_class_get_fields"); // IL2CPP Versions: All
            il2cpp_class_get_interfaces = NativeLibraryUtil.LoadFunction<il2cpp_class_get_interfaces_delegate>(handle, "il2cpp_class_get_interfaces"); // IL2CPP Versions: All
            il2cpp_class_get_properties = NativeLibraryUtil.LoadFunction<il2cpp_class_get_properties_delegate>(handle, "il2cpp_class_get_properties"); // IL2CPP Versions: All
            il2cpp_class_get_property_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_property_from_name_delegate>(handle, "il2cpp_class_get_property_from_name"); // IL2CPP Versions: All
            il2cpp_class_get_field_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_field_from_name_delegate>(handle, "il2cpp_class_get_field_from_name"); // IL2CPP Versions: All
            il2cpp_class_get_methods = NativeLibraryUtil.LoadFunction<il2cpp_class_get_methods_delegate>(handle, "il2cpp_class_get_methods"); // IL2CPP Versions: All
            il2cpp_class_get_method_from_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_method_from_name_delegate>(handle, "il2cpp_class_get_method_from_name"); // IL2CPP Versions: All
            il2cpp_class_get_name = NativeLibraryUtil.LoadFunction<il2cpp_class_get_name_delegate>(handle, "il2cpp_class_get_name"); // IL2CPP Versions: All
            il2cpp_class_get_namespace = NativeLibraryUtil.LoadFunction<il2cpp_class_get_namespace_delegate>(handle, "il2cpp_class_get_namespace"); // IL2CPP Versions: All
            il2cpp_class_get_parent = NativeLibraryUtil.LoadFunction<il2cpp_class_get_parent_delegate>(handle, "il2cpp_class_get_parent"); // IL2CPP Versions: All
            il2cpp_class_get_declaring_type = NativeLibraryUtil.LoadFunction<il2cpp_class_get_declaring_type_delegate>(handle, "il2cpp_class_get_declaring_type"); // IL2CPP Versions: All
            il2cpp_class_instance_size = NativeLibraryUtil.LoadFunction<il2cpp_class_instance_size_delegate>(handle, "il2cpp_class_instance_size"); // IL2CPP Versions: All
            il2cpp_class_num_fields = NativeLibraryUtil.LoadFunction<il2cpp_class_num_fields_delegate>(handle, "il2cpp_class_num_fields"); // IL2CPP Versions: All
            il2cpp_class_is_valuetype = NativeLibraryUtil.LoadFunction<il2cpp_class_is_valuetype_delegate>(handle, "il2cpp_class_is_valuetype"); // IL2CPP Versions: All
            il2cpp_class_value_size = NativeLibraryUtil.LoadFunction<il2cpp_class_value_size_delegate>(handle, "il2cpp_class_value_size"); // IL2CPP Versions: All
            il2cpp_class_get_flags = NativeLibraryUtil.LoadFunction<il2cpp_class_get_flags_delegate>(handle, "il2cpp_class_get_flags"); // IL2CPP Versions: All
            il2cpp_class_is_abstract = NativeLibraryUtil.LoadFunction<il2cpp_class_is_abstract_delegate>(handle, "il2cpp_class_is_abstract"); // IL2CPP Versions: All
            il2cpp_class_is_interface = NativeLibraryUtil.LoadFunction<il2cpp_class_is_interface_delegate>(handle, "il2cpp_class_is_interface"); // IL2CPP Versions: All
            il2cpp_class_array_element_size = NativeLibraryUtil.LoadFunction<il2cpp_class_array_element_size_delegate>(handle, "il2cpp_class_array_element_size"); // IL2CPP Versions: All
            il2cpp_class_from_type = NativeLibraryUtil.LoadFunction<il2cpp_class_from_type_delegate>(handle, "il2cpp_class_from_type"); // IL2CPP Versions: All
            il2cpp_class_get_type = NativeLibraryUtil.LoadFunction<il2cpp_class_get_type_delegate>(handle, "il2cpp_class_get_type"); // IL2CPP Versions: All
            il2cpp_class_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_class_has_attribute_delegate>(handle, "il2cpp_class_has_attribute"); // IL2CPP Versions: All
            il2cpp_class_has_references = NativeLibraryUtil.LoadFunction<il2cpp_class_has_references_delegate>(handle, "il2cpp_class_has_references"); // IL2CPP Versions: All
            il2cpp_class_is_enum = NativeLibraryUtil.LoadFunction<il2cpp_class_is_enum_delegate>(handle, "il2cpp_class_is_enum"); // IL2CPP Versions: All
            il2cpp_class_get_image = NativeLibraryUtil.LoadFunction<il2cpp_class_get_image_delegate>(handle, "il2cpp_class_get_image"); // IL2CPP Versions: All
            il2cpp_class_get_assemblyname = NativeLibraryUtil.LoadFunction<il2cpp_class_get_assemblyname_delegate>(handle, "il2cpp_class_get_assemblyname"); // IL2CPP Versions: All
            il2cpp_class_get_bitmap_size = NativeLibraryUtil.LoadFunction<il2cpp_class_get_bitmap_size_delegate>(handle, "il2cpp_class_get_bitmap_size"); // IL2CPP Versions: All
            il2cpp_class_get_bitmap = NativeLibraryUtil.LoadFunction<il2cpp_class_get_bitmap_delegate>(handle, "il2cpp_class_get_bitmap"); // IL2CPP Versions: All
            il2cpp_stats_dump_to_file = NativeLibraryUtil.LoadFunction<il2cpp_stats_dump_to_file_delegate>(handle, "il2cpp_stats_dump_to_file"); // IL2CPP Versions: All
            il2cpp_stats_get_value = NativeLibraryUtil.LoadFunction<il2cpp_stats_get_value_delegate>(handle, "il2cpp_stats_get_value"); // IL2CPP Versions: All
            il2cpp_domain_get = NativeLibraryUtil.LoadFunction<il2cpp_domain_get_delegate>(handle, "il2cpp_domain_get"); // IL2CPP Versions: All
            il2cpp_domain_assembly_open = NativeLibraryUtil.LoadFunction<il2cpp_domain_assembly_open_delegate>(handle, "il2cpp_domain_assembly_open"); // IL2CPP Versions: All
            il2cpp_domain_get_assemblies = NativeLibraryUtil.LoadFunction<il2cpp_domain_get_assemblies_delegate>(handle, "il2cpp_domain_get_assemblies"); // IL2CPP Versions: All
            il2cpp_raise_exception = NativeLibraryUtil.LoadFunction<il2cpp_raise_exception_delegate>(handle, "il2cpp_raise_exception"); // IL2CPP Versions: All
            il2cpp_exception_from_name_msg = NativeLibraryUtil.LoadFunction<il2cpp_exception_from_name_msg_delegate>(handle, "il2cpp_exception_from_name_msg"); // IL2CPP Versions: All
            il2cpp_get_exception_argument_null = NativeLibraryUtil.LoadFunction<il2cpp_get_exception_argument_null_delegate>(handle, "il2cpp_get_exception_argument_null"); // IL2CPP Versions: All
            il2cpp_format_exception = NativeLibraryUtil.LoadFunction<il2cpp_format_exception_delegate>(handle, "il2cpp_format_exception"); // IL2CPP Versions: All
            il2cpp_format_stack_trace = NativeLibraryUtil.LoadFunction<il2cpp_format_stack_trace_delegate>(handle, "il2cpp_format_stack_trace"); // IL2CPP Versions: All
            il2cpp_unhandled_exception = NativeLibraryUtil.LoadFunction<il2cpp_unhandled_exception_delegate>(handle, "il2cpp_unhandled_exception"); // IL2CPP Versions: All
            il2cpp_field_get_flags = NativeLibraryUtil.LoadFunction<il2cpp_field_get_flags_delegate>(handle, "il2cpp_field_get_flags"); // IL2CPP Versions: All
            il2cpp_field_get_name = NativeLibraryUtil.LoadFunction<il2cpp_field_get_name_delegate>(handle, "il2cpp_field_get_name"); // IL2CPP Versions: All
            il2cpp_field_get_parent = NativeLibraryUtil.LoadFunction<il2cpp_field_get_parent_delegate>(handle, "il2cpp_field_get_parent"); // IL2CPP Versions: All
            il2cpp_field_get_offset = NativeLibraryUtil.LoadFunction<il2cpp_field_get_offset_delegate>(handle, "il2cpp_field_get_offset"); // IL2CPP Versions: All
            il2cpp_field_get_type = NativeLibraryUtil.LoadFunction<il2cpp_field_get_type_delegate>(handle, "il2cpp_field_get_type"); // IL2CPP Versions: All
            il2cpp_field_get_value = NativeLibraryUtil.LoadFunction<il2cpp_field_get_value_delegate>(handle, "il2cpp_field_get_value"); // IL2CPP Versions: All
            il2cpp_field_get_value_object = NativeLibraryUtil.LoadFunction<il2cpp_field_get_value_object_delegate>(handle, "il2cpp_field_get_value_object"); // IL2CPP Versions: All
            il2cpp_field_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_field_has_attribute_delegate>(handle, "il2cpp_field_has_attribute"); // IL2CPP Versions: All
            il2cpp_field_set_value = NativeLibraryUtil.LoadFunction<il2cpp_field_set_value_delegate>(handle, "il2cpp_field_set_value"); // IL2CPP Versions: All
            il2cpp_field_static_get_value = NativeLibraryUtil.LoadFunction<il2cpp_field_static_get_value_delegate>(handle, "il2cpp_field_static_get_value"); // IL2CPP Versions: All
            il2cpp_field_static_set_value = NativeLibraryUtil.LoadFunction<il2cpp_field_static_set_value_delegate>(handle, "il2cpp_field_static_set_value"); // IL2CPP Versions: All
            il2cpp_gc_collect = NativeLibraryUtil.LoadFunction<il2cpp_gc_collect_delegate>(handle, "il2cpp_gc_collect"); // IL2CPP Versions: All
            il2cpp_gc_get_used_size = NativeLibraryUtil.LoadFunction<il2cpp_gc_get_used_size_delegate>(handle, "il2cpp_gc_get_used_size"); // IL2CPP Versions: All
            il2cpp_gc_get_heap_size = NativeLibraryUtil.LoadFunction<il2cpp_gc_get_heap_size_delegate>(handle, "il2cpp_gc_get_heap_size"); // IL2CPP Versions: All
            il2cpp_gchandle_new = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_new_delegate>(handle, "il2cpp_gchandle_new"); // IL2CPP Versions: All
            il2cpp_gchandle_new_weakref = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_new_weakref_delegate>(handle, "il2cpp_gchandle_new_weakref"); // IL2CPP Versions: All
            il2cpp_gchandle_get_target = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_get_target_delegate>(handle, "il2cpp_gchandle_get_target"); // IL2CPP Versions: All
            il2cpp_gchandle_free = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_free_delegate>(handle, "il2cpp_gchandle_free"); // IL2CPP Versions: All
            il2cpp_unity_liveness_calculation_begin = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_begin_delegate>(handle, "il2cpp_unity_liveness_calculation_begin"); // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
            il2cpp_unity_liveness_calculation_end = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_end_delegate>(handle, "il2cpp_unity_liveness_calculation_end"); // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
            il2cpp_unity_liveness_calculation_from_root = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_from_root_delegate>(handle, "il2cpp_unity_liveness_calculation_from_root"); // IL2CPP Versions: All
            il2cpp_unity_liveness_calculation_from_statics = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_calculation_from_statics_delegate>(handle, "il2cpp_unity_liveness_calculation_from_statics"); // IL2CPP Versions: All
            il2cpp_method_get_return_type = NativeLibraryUtil.LoadFunction<il2cpp_method_get_return_type_delegate>(handle, "il2cpp_method_get_return_type"); // IL2CPP Versions: All
            il2cpp_method_get_declaring_type = NativeLibraryUtil.LoadFunction<il2cpp_method_get_declaring_type_delegate>(handle, "il2cpp_method_get_declaring_type"); // IL2CPP Versions: All
            il2cpp_method_get_name = NativeLibraryUtil.LoadFunction<il2cpp_method_get_name_delegate>(handle, "il2cpp_method_get_name"); // IL2CPP Versions: All
            il2cpp_method_get_object = NativeLibraryUtil.LoadFunction<il2cpp_method_get_object_delegate>(handle, "il2cpp_method_get_object"); // IL2CPP Versions: All
            il2cpp_method_is_generic = NativeLibraryUtil.LoadFunction<il2cpp_method_is_generic_delegate>(handle, "il2cpp_method_is_generic"); // IL2CPP Versions: All
            il2cpp_method_is_inflated = NativeLibraryUtil.LoadFunction<il2cpp_method_is_inflated_delegate>(handle, "il2cpp_method_is_inflated"); // IL2CPP Versions: All
            il2cpp_method_is_instance = NativeLibraryUtil.LoadFunction<il2cpp_method_is_instance_delegate>(handle, "il2cpp_method_is_instance"); // IL2CPP Versions: All
            il2cpp_method_get_param_count = NativeLibraryUtil.LoadFunction<il2cpp_method_get_param_count_delegate>(handle, "il2cpp_method_get_param_count"); // IL2CPP Versions: All
            il2cpp_method_get_param = NativeLibraryUtil.LoadFunction<il2cpp_method_get_param_delegate>(handle, "il2cpp_method_get_param"); // IL2CPP Versions: All
            il2cpp_method_get_class = NativeLibraryUtil.LoadFunction<il2cpp_method_get_class_delegate>(handle, "il2cpp_method_get_class"); // IL2CPP Versions: All
            il2cpp_method_has_attribute = NativeLibraryUtil.LoadFunction<il2cpp_method_has_attribute_delegate>(handle, "il2cpp_method_has_attribute"); // IL2CPP Versions: All
            il2cpp_method_get_flags = NativeLibraryUtil.LoadFunction<il2cpp_method_get_flags_delegate>(handle, "il2cpp_method_get_flags"); // IL2CPP Versions: All
            il2cpp_method_get_token = NativeLibraryUtil.LoadFunction<il2cpp_method_get_token_delegate>(handle, "il2cpp_method_get_token"); // IL2CPP Versions: All
            il2cpp_method_get_param_name = NativeLibraryUtil.LoadFunction<il2cpp_method_get_param_name_delegate>(handle, "il2cpp_method_get_param_name"); // IL2CPP Versions: All
            il2cpp_profiler_install = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_delegate>(handle, "il2cpp_profiler_install"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_profiler_set_events = NativeLibraryUtil.LoadFunction<il2cpp_profiler_set_events_delegate>(handle, "il2cpp_profiler_set_events"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_profiler_install_enter_leave = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_enter_leave_delegate>(handle, "il2cpp_profiler_install_enter_leave"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_profiler_install_allocation = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_allocation_delegate>(handle, "il2cpp_profiler_install_allocation"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_profiler_install_gc = NativeLibraryUtil.LoadFunction<il2cpp_profiler_install_gc_delegate>(handle, "il2cpp_profiler_install_gc"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_property_get_flags = NativeLibraryUtil.LoadFunction<il2cpp_property_get_flags_delegate>(handle, "il2cpp_property_get_flags"); // IL2CPP Versions: All
            il2cpp_property_get_get_method = NativeLibraryUtil.LoadFunction<il2cpp_property_get_get_method_delegate>(handle, "il2cpp_property_get_get_method"); // IL2CPP Versions: All
            il2cpp_property_get_set_method = NativeLibraryUtil.LoadFunction<il2cpp_property_get_set_method_delegate>(handle, "il2cpp_property_get_set_method"); // IL2CPP Versions: All
            il2cpp_property_get_name = NativeLibraryUtil.LoadFunction<il2cpp_property_get_name_delegate>(handle, "il2cpp_property_get_name"); // IL2CPP Versions: All
            il2cpp_property_get_parent = NativeLibraryUtil.LoadFunction<il2cpp_property_get_parent_delegate>(handle, "il2cpp_property_get_parent"); // IL2CPP Versions: All
            il2cpp_object_get_class = NativeLibraryUtil.LoadFunction<il2cpp_object_get_class_delegate>(handle, "il2cpp_object_get_class"); // IL2CPP Versions: All
            il2cpp_object_get_size = NativeLibraryUtil.LoadFunction<il2cpp_object_get_size_delegate>(handle, "il2cpp_object_get_size"); // IL2CPP Versions: All
            il2cpp_object_get_virtual_method = NativeLibraryUtil.LoadFunction<il2cpp_object_get_virtual_method_delegate>(handle, "il2cpp_object_get_virtual_method"); // IL2CPP Versions: All
            il2cpp_object_new = NativeLibraryUtil.LoadFunction<il2cpp_object_new_delegate>(handle, "il2cpp_object_new"); // IL2CPP Versions: All
            il2cpp_object_unbox = NativeLibraryUtil.LoadFunction<il2cpp_object_unbox_delegate>(handle, "il2cpp_object_unbox"); // IL2CPP Versions: All
            il2cpp_value_box = NativeLibraryUtil.LoadFunction<il2cpp_value_box_delegate>(handle, "il2cpp_value_box"); // IL2CPP Versions: All
            il2cpp_monitor_enter = NativeLibraryUtil.LoadFunction<il2cpp_monitor_enter_delegate>(handle, "il2cpp_monitor_enter"); // IL2CPP Versions: All
            il2cpp_monitor_try_enter = NativeLibraryUtil.LoadFunction<il2cpp_monitor_try_enter_delegate>(handle, "il2cpp_monitor_try_enter"); // IL2CPP Versions: All
            il2cpp_monitor_exit = NativeLibraryUtil.LoadFunction<il2cpp_monitor_exit_delegate>(handle, "il2cpp_monitor_exit"); // IL2CPP Versions: All
            il2cpp_monitor_pulse = NativeLibraryUtil.LoadFunction<il2cpp_monitor_pulse_delegate>(handle, "il2cpp_monitor_pulse"); // IL2CPP Versions: All
            il2cpp_monitor_pulse_all = NativeLibraryUtil.LoadFunction<il2cpp_monitor_pulse_all_delegate>(handle, "il2cpp_monitor_pulse_all"); // IL2CPP Versions: All
            il2cpp_monitor_wait = NativeLibraryUtil.LoadFunction<il2cpp_monitor_wait_delegate>(handle, "il2cpp_monitor_wait"); // IL2CPP Versions: All
            il2cpp_monitor_try_wait = NativeLibraryUtil.LoadFunction<il2cpp_monitor_try_wait_delegate>(handle, "il2cpp_monitor_try_wait"); // IL2CPP Versions: All
            il2cpp_runtime_invoke = NativeLibraryUtil.LoadFunction<il2cpp_runtime_invoke_delegate>(handle, "il2cpp_runtime_invoke"); // IL2CPP Versions: All
            il2cpp_runtime_invoke_convert_args = NativeLibraryUtil.LoadFunction<il2cpp_runtime_invoke_convert_args_delegate>(handle, "il2cpp_runtime_invoke_convert_args"); // IL2CPP Versions: All
            il2cpp_runtime_class_init = NativeLibraryUtil.LoadFunction<il2cpp_runtime_class_init_delegate>(handle, "il2cpp_runtime_class_init"); // IL2CPP Versions: All
            il2cpp_runtime_object_init = NativeLibraryUtil.LoadFunction<il2cpp_runtime_object_init_delegate>(handle, "il2cpp_runtime_object_init"); // IL2CPP Versions: All
            il2cpp_runtime_object_init_exception = NativeLibraryUtil.LoadFunction<il2cpp_runtime_object_init_exception_delegate>(handle, "il2cpp_runtime_object_init_exception"); // IL2CPP Versions: All
            il2cpp_runtime_unhandled_exception_policy_set = NativeLibraryUtil.LoadFunction<il2cpp_runtime_unhandled_exception_policy_set_delegate>(handle, "il2cpp_runtime_unhandled_exception_policy_set"); // IL2CPP Versions: All
            il2cpp_delegate_begin_invoke = NativeLibraryUtil.LoadFunction<il2cpp_delegate_begin_invoke_delegate>(handle, "il2cpp_delegate_begin_invoke"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_delegate_end_invoke = NativeLibraryUtil.LoadFunction<il2cpp_delegate_end_invoke_delegate>(handle, "il2cpp_delegate_end_invoke"); // IL2CPP Versions: 16, 18, 19, 20, 21
            il2cpp_string_length = NativeLibraryUtil.LoadFunction<il2cpp_string_length_delegate>(handle, "il2cpp_string_length"); // IL2CPP Versions: All
            il2cpp_string_chars = NativeLibraryUtil.LoadFunction<il2cpp_string_chars_delegate>(handle, "il2cpp_string_chars"); // IL2CPP Versions: All
            il2cpp_string_new = NativeLibraryUtil.LoadFunction<il2cpp_string_new_delegate>(handle, "il2cpp_string_new"); // IL2CPP Versions: All
            il2cpp_string_new_len = NativeLibraryUtil.LoadFunction<il2cpp_string_new_len_delegate>(handle, "il2cpp_string_new_len"); // IL2CPP Versions: All
            il2cpp_string_new_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_string_new_utf16_delegate>(handle, "il2cpp_string_new_utf16"); // IL2CPP Versions: All
            il2cpp_string_new_wrapper = NativeLibraryUtil.LoadFunction<il2cpp_string_new_wrapper_delegate>(handle, "il2cpp_string_new_wrapper"); // IL2CPP Versions: All
            il2cpp_string_intern = NativeLibraryUtil.LoadFunction<il2cpp_string_intern_delegate>(handle, "il2cpp_string_intern"); // IL2CPP Versions: All
            il2cpp_string_is_interned = NativeLibraryUtil.LoadFunction<il2cpp_string_is_interned_delegate>(handle, "il2cpp_string_is_interned"); // IL2CPP Versions: All
            il2cpp_thread_get_name = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_name_delegate>(handle, "il2cpp_thread_get_name"); // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24
            il2cpp_thread_current = NativeLibraryUtil.LoadFunction<il2cpp_thread_current_delegate>(handle, "il2cpp_thread_current"); // IL2CPP Versions: All
            il2cpp_thread_attach = NativeLibraryUtil.LoadFunction<il2cpp_thread_attach_delegate>(handle, "il2cpp_thread_attach"); // IL2CPP Versions: All
            il2cpp_thread_detach = NativeLibraryUtil.LoadFunction<il2cpp_thread_detach_delegate>(handle, "il2cpp_thread_detach"); // IL2CPP Versions: All
            il2cpp_thread_get_all_attached_threads = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_all_attached_threads_delegate>(handle, "il2cpp_thread_get_all_attached_threads"); // IL2CPP Versions: All
            il2cpp_is_vm_thread = NativeLibraryUtil.LoadFunction<il2cpp_is_vm_thread_delegate>(handle, "il2cpp_is_vm_thread"); // IL2CPP Versions: All
            il2cpp_current_thread_walk_frame_stack = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_walk_frame_stack_delegate>(handle, "il2cpp_current_thread_walk_frame_stack"); // IL2CPP Versions: All
            il2cpp_thread_walk_frame_stack = NativeLibraryUtil.LoadFunction<il2cpp_thread_walk_frame_stack_delegate>(handle, "il2cpp_thread_walk_frame_stack"); // IL2CPP Versions: All
            il2cpp_current_thread_get_top_frame = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_get_top_frame_delegate>(handle, "il2cpp_current_thread_get_top_frame"); // IL2CPP Versions: All
            il2cpp_thread_get_top_frame = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_top_frame_delegate>(handle, "il2cpp_thread_get_top_frame"); // IL2CPP Versions: All
            il2cpp_current_thread_get_frame_at = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_get_frame_at_delegate>(handle, "il2cpp_current_thread_get_frame_at"); // IL2CPP Versions: All
            il2cpp_thread_get_frame_at = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_frame_at_delegate>(handle, "il2cpp_thread_get_frame_at"); // IL2CPP Versions: All
            il2cpp_current_thread_get_stack_depth = NativeLibraryUtil.LoadFunction<il2cpp_current_thread_get_stack_depth_delegate>(handle, "il2cpp_current_thread_get_stack_depth"); // IL2CPP Versions: All
            il2cpp_thread_get_stack_depth = NativeLibraryUtil.LoadFunction<il2cpp_thread_get_stack_depth_delegate>(handle, "il2cpp_thread_get_stack_depth"); // IL2CPP Versions: All
            il2cpp_type_get_object = NativeLibraryUtil.LoadFunction<il2cpp_type_get_object_delegate>(handle, "il2cpp_type_get_object"); // IL2CPP Versions: All
            il2cpp_type_get_type = NativeLibraryUtil.LoadFunction<il2cpp_type_get_type_delegate>(handle, "il2cpp_type_get_type"); // IL2CPP Versions: All
            il2cpp_type_get_class_or_element_class = NativeLibraryUtil.LoadFunction<il2cpp_type_get_class_or_element_class_delegate>(handle, "il2cpp_type_get_class_or_element_class"); // IL2CPP Versions: All
            il2cpp_type_get_name = NativeLibraryUtil.LoadFunction<il2cpp_type_get_name_delegate>(handle, "il2cpp_type_get_name"); // IL2CPP Versions: All
            il2cpp_image_get_assembly = NativeLibraryUtil.LoadFunction<il2cpp_image_get_assembly_delegate>(handle, "il2cpp_image_get_assembly"); // IL2CPP Versions: All
            il2cpp_image_get_name = NativeLibraryUtil.LoadFunction<il2cpp_image_get_name_delegate>(handle, "il2cpp_image_get_name"); // IL2CPP Versions: All
            il2cpp_image_get_filename = NativeLibraryUtil.LoadFunction<il2cpp_image_get_filename_delegate>(handle, "il2cpp_image_get_filename"); // IL2CPP Versions: All
            il2cpp_image_get_entry_point = NativeLibraryUtil.LoadFunction<il2cpp_image_get_entry_point_delegate>(handle, "il2cpp_image_get_entry_point"); // IL2CPP Versions: All
            il2cpp_capture_memory_snapshot = NativeLibraryUtil.LoadFunction<il2cpp_capture_memory_snapshot_delegate>(handle, "il2cpp_capture_memory_snapshot"); // IL2CPP Versions: All
            il2cpp_free_captured_memory_snapshot = NativeLibraryUtil.LoadFunction<il2cpp_free_captured_memory_snapshot_delegate>(handle, "il2cpp_free_captured_memory_snapshot"); // IL2CPP Versions: All
            il2cpp_set_find_plugin_callback = NativeLibraryUtil.LoadFunction<il2cpp_set_find_plugin_callback_delegate>(handle, "il2cpp_set_find_plugin_callback"); // IL2CPP Versions: 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_class_get_nested_types = NativeLibraryUtil.LoadFunction<il2cpp_class_get_nested_types_delegate>(handle, "il2cpp_class_get_nested_types"); // IL2CPP Versions: 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_gc_collect_a_little = NativeLibraryUtil.LoadFunction<il2cpp_gc_collect_a_little_delegate>(handle, "il2cpp_gc_collect_a_little"); // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_gc_disable = NativeLibraryUtil.LoadFunction<il2cpp_gc_disable_delegate>(handle, "il2cpp_gc_disable"); // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_gc_enable = NativeLibraryUtil.LoadFunction<il2cpp_gc_enable_delegate>(handle, "il2cpp_gc_enable"); // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_init_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_init_utf16_delegate>(handle, "il2cpp_init_utf16"); // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_set_commandline_arguments_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_set_commandline_arguments_utf16_delegate>(handle, "il2cpp_set_commandline_arguments_utf16"); // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_set_config_utf16 = NativeLibraryUtil.LoadFunction<il2cpp_set_config_utf16_delegate>(handle, "il2cpp_set_config_utf16"); // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_set_config = NativeLibraryUtil.LoadFunction<il2cpp_set_config_delegate>(handle, "il2cpp_set_config"); // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
            il2cpp_field_set_value_object = NativeLibraryUtil.LoadFunction<il2cpp_field_set_value_object_delegate>(handle, "il2cpp_field_set_value_object"); // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29
            il2cpp_register_log_callback = NativeLibraryUtil.LoadFunction<il2cpp_register_log_callback_delegate>(handle, "il2cpp_register_log_callback"); // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29
            il2cpp_set_temp_dir = NativeLibraryUtil.LoadFunction<il2cpp_set_temp_dir_delegate>(handle, "il2cpp_set_temp_dir"); // IL2CPP Versions: 24, 25, 26, 27, 28, 29
            il2cpp_class_is_blittable = NativeLibraryUtil.LoadFunction<il2cpp_class_is_blittable_delegate>(handle, "il2cpp_class_is_blittable"); // IL2CPP Versions: 24, 25, 26, 27, 28, 29
            il2cpp_class_for_each = NativeLibraryUtil.LoadFunction<il2cpp_class_for_each_delegate>(handle, "il2cpp_class_for_each"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_get_name_chunked = NativeLibraryUtil.LoadFunction<il2cpp_type_get_name_chunked_delegate>(handle, "il2cpp_type_get_name_chunked"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_get_type_token = NativeLibraryUtil.LoadFunction<il2cpp_class_get_type_token_delegate>(handle, "il2cpp_class_get_type_token"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_get_rank = NativeLibraryUtil.LoadFunction<il2cpp_class_get_rank_delegate>(handle, "il2cpp_class_get_rank"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_get_data_size = NativeLibraryUtil.LoadFunction<il2cpp_class_get_data_size_delegate>(handle, "il2cpp_class_get_data_size"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_get_static_field_data = NativeLibraryUtil.LoadFunction<il2cpp_class_get_static_field_data_delegate>(handle, "il2cpp_class_get_static_field_data"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_native_stack_trace = NativeLibraryUtil.LoadFunction<il2cpp_native_stack_trace_delegate>(handle, "il2cpp_native_stack_trace"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_field_is_literal = NativeLibraryUtil.LoadFunction<il2cpp_field_is_literal_delegate>(handle, "il2cpp_field_is_literal"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_is_disabled = NativeLibraryUtil.LoadFunction<il2cpp_gc_is_disabled_delegate>(handle, "il2cpp_gc_is_disabled"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_get_max_time_slice_ns = NativeLibraryUtil.LoadFunction<il2cpp_gc_get_max_time_slice_ns_delegate>(handle, "il2cpp_gc_get_max_time_slice_ns"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_set_max_time_slice_ns = NativeLibraryUtil.LoadFunction<il2cpp_gc_set_max_time_slice_ns_delegate>(handle, "il2cpp_gc_set_max_time_slice_ns"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_is_incremental = NativeLibraryUtil.LoadFunction<il2cpp_gc_is_incremental_delegate>(handle, "il2cpp_gc_is_incremental"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_wbarrier_set_field = NativeLibraryUtil.LoadFunction<il2cpp_gc_wbarrier_set_field_delegate>(handle, "il2cpp_gc_wbarrier_set_field"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_has_strict_wbarriers = NativeLibraryUtil.LoadFunction<il2cpp_gc_has_strict_wbarriers_delegate>(handle, "il2cpp_gc_has_strict_wbarriers"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_set_external_allocation_tracker = NativeLibraryUtil.LoadFunction<il2cpp_gc_set_external_allocation_tracker_delegate>(handle, "il2cpp_gc_set_external_allocation_tracker"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_set_external_wbarrier_tracker = NativeLibraryUtil.LoadFunction<il2cpp_gc_set_external_wbarrier_tracker_delegate>(handle, "il2cpp_gc_set_external_wbarrier_tracker"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_foreach_heap = NativeLibraryUtil.LoadFunction<il2cpp_gc_foreach_heap_delegate>(handle, "il2cpp_gc_foreach_heap"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_stop_gc_world = NativeLibraryUtil.LoadFunction<il2cpp_stop_gc_world_delegate>(handle, "il2cpp_stop_gc_world"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_start_gc_world = NativeLibraryUtil.LoadFunction<il2cpp_start_gc_world_delegate>(handle, "il2cpp_start_gc_world"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gchandle_foreach_get_target = NativeLibraryUtil.LoadFunction<il2cpp_gchandle_foreach_get_target_delegate>(handle, "il2cpp_gchandle_foreach_get_target"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_object_header_size = NativeLibraryUtil.LoadFunction<il2cpp_object_header_size_delegate>(handle, "il2cpp_object_header_size"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_array_object_header_size = NativeLibraryUtil.LoadFunction<il2cpp_array_object_header_size_delegate>(handle, "il2cpp_array_object_header_size"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_offset_of_array_length_in_array_object_header = NativeLibraryUtil.LoadFunction<il2cpp_offset_of_array_length_in_array_object_header_delegate>(handle, "il2cpp_offset_of_array_length_in_array_object_header"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_offset_of_array_bounds_in_array_object_header = NativeLibraryUtil.LoadFunction<il2cpp_offset_of_array_bounds_in_array_object_header_delegate>(handle, "il2cpp_offset_of_array_bounds_in_array_object_header"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_allocation_granularity = NativeLibraryUtil.LoadFunction<il2cpp_allocation_granularity_delegate>(handle, "il2cpp_allocation_granularity"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_method_get_from_reflection = NativeLibraryUtil.LoadFunction<il2cpp_method_get_from_reflection_delegate>(handle, "il2cpp_method_get_from_reflection"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_override_stack_backtrace = NativeLibraryUtil.LoadFunction<il2cpp_override_stack_backtrace_delegate>(handle, "il2cpp_override_stack_backtrace"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_is_byref = NativeLibraryUtil.LoadFunction<il2cpp_type_is_byref_delegate>(handle, "il2cpp_type_is_byref"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_get_attrs = NativeLibraryUtil.LoadFunction<il2cpp_type_get_attrs_delegate>(handle, "il2cpp_type_get_attrs"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_equals = NativeLibraryUtil.LoadFunction<il2cpp_type_equals_delegate>(handle, "il2cpp_type_equals"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_get_assembly_qualified_name = NativeLibraryUtil.LoadFunction<il2cpp_type_get_assembly_qualified_name_delegate>(handle, "il2cpp_type_get_assembly_qualified_name"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_is_static = NativeLibraryUtil.LoadFunction<il2cpp_type_is_static_delegate>(handle, "il2cpp_type_is_static"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_type_is_pointer_type = NativeLibraryUtil.LoadFunction<il2cpp_type_is_pointer_type_delegate>(handle, "il2cpp_type_is_pointer_type"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_image_get_class_count = NativeLibraryUtil.LoadFunction<il2cpp_image_get_class_count_delegate>(handle, "il2cpp_image_get_class_count"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_image_get_class = NativeLibraryUtil.LoadFunction<il2cpp_image_get_class_delegate>(handle, "il2cpp_image_get_class"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_debugger_set_agent_options = NativeLibraryUtil.LoadFunction<il2cpp_debugger_set_agent_options_delegate>(handle, "il2cpp_debugger_set_agent_options"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_is_debugger_attached = NativeLibraryUtil.LoadFunction<il2cpp_is_debugger_attached_delegate>(handle, "il2cpp_is_debugger_attached"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_register_debugger_agent_transport = NativeLibraryUtil.LoadFunction<il2cpp_register_debugger_agent_transport_delegate>(handle, "il2cpp_register_debugger_agent_transport"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_debug_get_method_info = NativeLibraryUtil.LoadFunction<il2cpp_debug_get_method_info_delegate>(handle, "il2cpp_debug_get_method_info"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_unity_install_unitytls_interface = NativeLibraryUtil.LoadFunction<il2cpp_unity_install_unitytls_interface_delegate>(handle, "il2cpp_unity_install_unitytls_interface"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_from_class = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_from_class_delegate>(handle, "il2cpp_custom_attrs_from_class"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_from_method = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_from_method_delegate>(handle, "il2cpp_custom_attrs_from_method"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_get_attr = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_get_attr_delegate>(handle, "il2cpp_custom_attrs_get_attr"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_has_attr = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_has_attr_delegate>(handle, "il2cpp_custom_attrs_has_attr"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_construct = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_construct_delegate>(handle, "il2cpp_custom_attrs_construct"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_custom_attrs_free = NativeLibraryUtil.LoadFunction<il2cpp_custom_attrs_free_delegate>(handle, "il2cpp_custom_attrs_free"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_set_userdata = NativeLibraryUtil.LoadFunction<il2cpp_class_set_userdata_delegate>(handle, "il2cpp_class_set_userdata"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_class_get_userdata_offset = NativeLibraryUtil.LoadFunction<il2cpp_class_get_userdata_offset_delegate>(handle, "il2cpp_class_get_userdata_offset"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_set_default_thread_affinity = NativeLibraryUtil.LoadFunction<il2cpp_set_default_thread_affinity_delegate>(handle, "il2cpp_set_default_thread_affinity"); // IL2CPP Versions: 25, 26, 27, 28, 29
            il2cpp_gc_start_incremental_collection = NativeLibraryUtil.LoadFunction<il2cpp_gc_start_incremental_collection_delegate>(handle, "il2cpp_gc_start_incremental_collection"); // IL2CPP Versions: 27, 28, 29
            il2cpp_gc_set_mode = NativeLibraryUtil.LoadFunction<il2cpp_gc_set_mode_delegate>(handle, "il2cpp_gc_set_mode"); // IL2CPP Versions: 27, 28, 29
            il2cpp_unity_liveness_allocate_struct = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_allocate_struct_delegate>(handle, "il2cpp_unity_liveness_allocate_struct"); // IL2CPP Versions: 28, 29
            il2cpp_unity_liveness_finalize = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_finalize_delegate>(handle, "il2cpp_unity_liveness_finalize"); // IL2CPP Versions: 28, 29
            il2cpp_unity_liveness_free_struct = NativeLibraryUtil.LoadFunction<il2cpp_unity_liveness_free_struct_delegate>(handle, "il2cpp_unity_liveness_free_struct"); // IL2CPP Versions: 28, 29
            il2cpp_gc_alloc_fixed = NativeLibraryUtil.LoadFunction<il2cpp_gc_alloc_fixed_delegate>(handle, "il2cpp_gc_alloc_fixed"); // IL2CPP Versions: 29
            il2cpp_gc_free_fixed = NativeLibraryUtil.LoadFunction<il2cpp_gc_free_fixed_delegate>(handle, "il2cpp_gc_free_fixed"); // IL2CPP Versions: 29

            #endregion
        }

        #region Delegate Definitions (generated)

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_init_delegate(nint domain_name);
        // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_shutdown_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_config_dir_delegate(nint config_path);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_data_dir_delegate(nint data_path);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_commandline_arguments_delegate(int argc, nint argv, nint basedir);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_memory_callbacks_delegate(IntPtr callbacks);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_get_corlib_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_add_internal_call_delegate(nint name, IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_resolve_icall_delegate(nint name);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_alloc_delegate(ref ulong size);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_free_delegate(void* ptr);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_class_get_delegate(IntPtr element_class, uint rank);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_array_length_delegate(IntPtr array);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_array_get_byte_length_delegate(IntPtr array);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_new_delegate(IntPtr elementIl2CppClass, uint length);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_new_specific_delegate(IntPtr arrayIl2CppClass, ulong length);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_array_new_full_delegate(IntPtr array_class, ulong* lengths, ulong* lower_bounds);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_bounded_array_class_get_delegate(IntPtr element_class, uint rank, bool bounded);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_array_element_size_delegate(IntPtr array_class);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_assembly_get_image_delegate(IntPtr assembly);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_enum_basetype_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_generic_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_inflated_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_assignable_from_delegate(IntPtr klass, IntPtr oklass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_subclass_of_delegate(IntPtr klass, IntPtr klassc, bool check_interfaces);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_parent_delegate(IntPtr klass, IntPtr klassc);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_il2cpp_type_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_name_delegate(IntPtr image, nint namespaze, nint name);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_system_type_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_element_class_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_events_delegate(IntPtr klass, void** iter);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_fields_delegate(IntPtr klass, void** iter);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_interfaces_delegate(IntPtr klass, void** iter);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_properties_delegate(IntPtr klass, void** iter);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_property_from_name_delegate(IntPtr klass, nint name);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_field_from_name_delegate(IntPtr klass, nint name);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_methods_delegate(IntPtr klass, ref IntPtr iter);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_method_from_name_delegate(IntPtr klass, nint name, int argsCount);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_class_get_name_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_class_get_namespace_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_parent_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_declaring_type_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_instance_size_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate ulong il2cpp_class_num_fields_delegate(IntPtr enumKlass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_valuetype_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_value_size_delegate(IntPtr klass, ref uint align);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_get_flags_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_abstract_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_interface_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_array_element_size_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_from_type_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_type_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_attribute_delegate(IntPtr klass, IntPtr attr_class);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_has_references_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_enum_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_image_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_class_get_assemblyname_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate ulong il2cpp_class_get_bitmap_size_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_class_get_bitmap_delegate(IntPtr klass, ulong bitmap);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_stats_dump_to_file_delegate(nint path);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate ulong il2cpp_stats_get_value_delegate(int stat);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_domain_get_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_domain_assembly_open_delegate(IntPtr domain, nint name);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr* il2cpp_domain_get_assemblies_delegate(IntPtr domain, ref uint size);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_raise_exception_delegate(IntPtr arg0);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_exception_from_name_msg_delegate(IntPtr image, nint name_space, nint name, nint msg);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_get_exception_argument_null_delegate(nint arg);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_format_exception_delegate(IntPtr ex, nint message, int message_size);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_format_stack_trace_delegate(IntPtr ex, nint output, int output_size);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unhandled_exception_delegate(IntPtr arg0);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_field_get_flags_delegate(IntPtr field);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_field_get_name_delegate(IntPtr field);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_field_get_parent_delegate(IntPtr field);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate ulong il2cpp_field_get_offset_delegate(IntPtr field);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_field_get_type_delegate(IntPtr field);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_get_value_delegate(IntPtr obj, IntPtr field, void* value);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_field_get_value_object_delegate(IntPtr field, IntPtr obj);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_field_has_attribute_delegate(IntPtr field, IntPtr attr_class);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_set_value_delegate(IntPtr obj, IntPtr field, void* value);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_static_get_value_delegate(IntPtr field, void* value);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_static_set_value_delegate(IntPtr field, void* value);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_collect_delegate(int maxGenerations);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate long il2cpp_gc_get_used_size_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate long il2cpp_gc_get_heap_size_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_gchandle_new_delegate(IntPtr obj, bool pinned);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_gchandle_new_weakref_delegate(IntPtr obj, bool track_resurrection);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_gchandle_get_target_delegate(IntPtr gchandle);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gchandle_free_delegate(uint gchandle);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_unity_liveness_calculation_begin_delegate(IntPtr filter, int max_object_count, IntPtr callback, void* userdata, IntPtr onWorldStarted, IntPtr onWorldStopped);
        // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_calculation_end_delegate(void* state);
        // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_calculation_from_root_delegate(IntPtr root, void* state);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_calculation_from_statics_delegate(void* state);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_return_type_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_declaring_type_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_method_get_name_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_object_delegate(IntPtr method, IntPtr refclass);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_generic_delegate(IntPtr method);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_inflated_delegate(IntPtr method);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_is_instance_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_method_get_param_count_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_param_delegate(IntPtr method, uint index);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_class_delegate(IntPtr method);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_method_has_attribute_delegate(IntPtr method, IntPtr attr_class);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_method_get_flags_delegate(IntPtr method, uint iflags);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_method_get_token_delegate(IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_method_get_param_name_delegate(IntPtr method, uint index);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_delegate(IntPtr prof, IntPtr shutdown_callback);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_set_events_delegate(IntPtr events);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_enter_leave_delegate(IntPtr enter, IntPtr fleave);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_allocation_delegate(IntPtr callback);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_profiler_install_gc_delegate(IntPtr callback, IntPtr heap_resize_callback);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_property_get_flags_delegate(IntPtr prop);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_property_get_get_method_delegate(IntPtr prop);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_property_get_set_method_delegate(IntPtr prop);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_property_get_name_delegate(IntPtr prop);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_property_get_parent_delegate(IntPtr prop);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_object_get_class_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_object_get_size_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_object_get_virtual_method_delegate(IntPtr obj, IntPtr method);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_object_new_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_object_unbox_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_value_box_delegate(IntPtr klass, IntPtr data);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_enter_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_monitor_try_enter_delegate(IntPtr obj, uint timeout);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_exit_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_pulse_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_pulse_all_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_monitor_wait_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_monitor_try_wait_delegate(IntPtr obj, uint timeout);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_runtime_invoke_delegate(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_runtime_invoke_convert_args_delegate(IntPtr method, void* obj, void** param, int paramCount, void** exc);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_runtime_class_init_delegate(IntPtr klass);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_runtime_object_init_delegate(IntPtr obj);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_runtime_object_init_exception_delegate(IntPtr obj, void** exc);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_runtime_unhandled_exception_policy_set_delegate(int value);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_delegate_begin_invoke_delegate(IntPtr dele, void** param, IntPtr asyncCallback, IntPtr state);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_delegate_end_invoke_delegate(IntPtr asyncResult, void** out_args);
        // IL2CPP Versions: 16, 18, 19, 20, 21

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_string_length_delegate(IntPtr str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_chars_delegate(IntPtr str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_delegate(nint str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_len_delegate(nint str, uint length);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_utf16_delegate(IntPtr text, int len);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_new_wrapper_delegate(nint str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_intern_delegate(IntPtr str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_string_is_interned_delegate(IntPtr str);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_thread_get_name_delegate(IntPtr thread, uint len);
        // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_thread_current_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_thread_attach_delegate(IntPtr domain);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_thread_detach_delegate(IntPtr thread);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void** il2cpp_thread_get_all_attached_threads_delegate(ref ulong size);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_is_vm_thread_delegate(IntPtr thread);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_current_thread_walk_frame_stack_delegate(IntPtr func, void* user_data);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_thread_walk_frame_stack_delegate(IntPtr thread, IntPtr func, void* user_data);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_current_thread_get_top_frame_delegate(IntPtr frame);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_thread_get_top_frame_delegate(IntPtr thread, IntPtr frame);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_current_thread_get_frame_at_delegate(int offset, IntPtr frame);
        // IL2CPP Versions: All

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_thread_get_frame_at_delegate(IntPtr thread, int offset, IntPtr frame);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_current_thread_get_stack_depth_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_thread_get_stack_depth_delegate(IntPtr thread);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_type_get_object_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_type_get_type_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_type_get_class_or_element_class_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_type_get_name_delegate(IntPtr type);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_image_get_assembly_delegate(IntPtr image);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_image_get_name_delegate(IntPtr image);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_image_get_filename_delegate(IntPtr image);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_image_get_entry_point_delegate(IntPtr image);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_capture_memory_snapshot_delegate();
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_free_captured_memory_snapshot_delegate(IntPtr snapshot);
        // IL2CPP Versions: All

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_find_plugin_callback_delegate(IntPtr method);
        // IL2CPP Versions: 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_class_get_nested_types_delegate(IntPtr klass, ref IntPtr iter);
        // IL2CPP Versions: 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_gc_collect_a_little_delegate();
        // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_disable_delegate();
        // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_enable_delegate();
        // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_init_utf16_delegate(IntPtr domain_name);
        // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_commandline_arguments_utf16_delegate(int argc, IntPtr argv, nint basedir);
        // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_config_utf16_delegate(IntPtr executablePath);
        // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_config_delegate(nint executablePath);
        // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_field_set_value_object_delegate(IntPtr instance, IntPtr field, IntPtr value);
        // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_register_log_callback_delegate(IntPtr method);
        // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_temp_dir_delegate(nint temp_path);
        // IL2CPP Versions: 24, 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_class_is_blittable_delegate(IntPtr klass);
        // IL2CPP Versions: 24, 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_class_for_each_delegate(IntPtr arg0);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_type_get_name_chunked_delegate(IntPtr type, IntPtr arg1);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_class_get_type_token_delegate(IntPtr klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_get_rank_delegate(IntPtr klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_class_get_data_size_delegate(IntPtr klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_class_get_static_field_data_delegate(IntPtr klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_native_stack_trace_delegate(IntPtr ex, ulong** addresses, int numFrames, nint imageUUID);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_field_is_literal_delegate(IntPtr field);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_gc_is_disabled_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate long il2cpp_gc_get_max_time_slice_ns_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_set_max_time_slice_ns_delegate(long maxTimeSlice);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_gc_is_incremental_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_wbarrier_set_field_delegate(IntPtr obj, IntPtr targetAddress, IntPtr objec);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_gc_has_strict_wbarriers_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_set_external_allocation_tracker_delegate(IntPtr arg0);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_set_external_wbarrier_tracker_delegate(IntPtr arg0);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_foreach_heap_delegate(IntPtr arg0);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_stop_gc_world_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_start_gc_world_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gchandle_foreach_get_target_delegate(IntPtr arg0);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_object_header_size_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_array_object_header_size_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_offset_of_array_length_in_array_object_header_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_offset_of_array_bounds_in_array_object_header_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_allocation_granularity_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_method_get_from_reflection_delegate(IntPtr method);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_override_stack_backtrace_delegate(IntPtr stackBacktraceFunc);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_is_byref_delegate(IntPtr type);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate uint il2cpp_type_get_attrs_delegate(IntPtr type);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_equals_delegate(IntPtr type, IntPtr otherType);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate nint il2cpp_type_get_assembly_qualified_name_delegate(IntPtr type);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_is_static_delegate(IntPtr type);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_type_is_pointer_type_delegate(IntPtr type);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate ulong il2cpp_image_get_class_count_delegate(IntPtr image);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_image_get_class_delegate(IntPtr image, ulong index);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_debugger_set_agent_options_delegate(nint options);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_is_debugger_attached_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_register_debugger_agent_transport_delegate(IntPtr debuggerTransport);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_debug_get_method_info_delegate(IntPtr arg0, IntPtr methodDebugInfo);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_install_unitytls_interface_delegate(void* unitytlsInterfaceStruct);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_from_class_delegate(IntPtr klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_from_method_delegate(IntPtr method);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_get_attr_delegate(IntPtr ainfo, IntPtr attr_klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [return: MarshalAs(UnmanagedType.I1)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate bool il2cpp_custom_attrs_has_attr_delegate(IntPtr ainfo, IntPtr attr_klass);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr il2cpp_custom_attrs_construct_delegate(IntPtr cinfo);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_custom_attrs_free_delegate(IntPtr ainfo);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_class_set_userdata_delegate(IntPtr klass, void* userdata);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int il2cpp_class_get_userdata_offset_delegate();
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_set_default_thread_affinity_delegate(long affinity_mask);
        // IL2CPP Versions: 25, 26, 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_start_incremental_collection_delegate();
        // IL2CPP Versions: 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_set_mode_delegate(int mode);
        // IL2CPP Versions: 27, 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_unity_liveness_allocate_struct_delegate(IntPtr filter, int max_object_count, IntPtr callback, void* userdata, IntPtr reallocate);
        // IL2CPP Versions: 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_finalize_delegate(void* state);
        // IL2CPP Versions: 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_unity_liveness_free_struct_delegate(void* state);
        // IL2CPP Versions: 28, 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void* il2cpp_gc_alloc_fixed_delegate(ref ulong size);
        // IL2CPP Versions: 29

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void il2cpp_gc_free_fixed_delegate(void* address);
        // IL2CPP Versions: 29

        #endregion

        // ═══════════════════════════════════════════════════════
        // SECTION 4 – Delegate instance properties
        //   (paste inside Delegates class, inside #region Delegate Instances)
        // ═══════════════════════════════════════════════════════

        #region Delegate Instances (generated)

        public il2cpp_init_delegate il2cpp_init { get; } // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_shutdown_delegate il2cpp_shutdown { get; } // IL2CPP Versions: All
        public il2cpp_set_config_dir_delegate il2cpp_set_config_dir { get; } // IL2CPP Versions: All
        public il2cpp_set_data_dir_delegate il2cpp_set_data_dir { get; } // IL2CPP Versions: All
        public il2cpp_set_commandline_arguments_delegate il2cpp_set_commandline_arguments { get; } // IL2CPP Versions: All
        public il2cpp_set_memory_callbacks_delegate il2cpp_set_memory_callbacks { get; } // IL2CPP Versions: All
        public il2cpp_get_corlib_delegate il2cpp_get_corlib { get; } // IL2CPP Versions: All
        public il2cpp_add_internal_call_delegate il2cpp_add_internal_call { get; } // IL2CPP Versions: All
        public il2cpp_resolve_icall_delegate il2cpp_resolve_icall { get; } // IL2CPP Versions: All
        public il2cpp_alloc_delegate il2cpp_alloc { get; } // IL2CPP Versions: All
        public il2cpp_free_delegate il2cpp_free { get; } // IL2CPP Versions: All
        public il2cpp_array_class_get_delegate il2cpp_array_class_get { get; } // IL2CPP Versions: All
        public il2cpp_array_length_delegate il2cpp_array_length { get; } // IL2CPP Versions: All
        public il2cpp_array_get_byte_length_delegate il2cpp_array_get_byte_length { get; } // IL2CPP Versions: All
        public il2cpp_array_new_delegate il2cpp_array_new { get; } // IL2CPP Versions: All
        public il2cpp_array_new_specific_delegate il2cpp_array_new_specific { get; } // IL2CPP Versions: All
        public il2cpp_array_new_full_delegate il2cpp_array_new_full { get; } // IL2CPP Versions: All
        public il2cpp_bounded_array_class_get_delegate il2cpp_bounded_array_class_get { get; } // IL2CPP Versions: All
        public il2cpp_array_element_size_delegate il2cpp_array_element_size { get; } // IL2CPP Versions: All
        public il2cpp_assembly_get_image_delegate il2cpp_assembly_get_image { get; } // IL2CPP Versions: All
        public il2cpp_class_enum_basetype_delegate il2cpp_class_enum_basetype { get; } // IL2CPP Versions: All
        public il2cpp_class_is_generic_delegate il2cpp_class_is_generic { get; } // IL2CPP Versions: All
        public il2cpp_class_is_inflated_delegate il2cpp_class_is_inflated { get; } // IL2CPP Versions: All
        public il2cpp_class_is_assignable_from_delegate il2cpp_class_is_assignable_from { get; } // IL2CPP Versions: All
        public il2cpp_class_is_subclass_of_delegate il2cpp_class_is_subclass_of { get; } // IL2CPP Versions: All
        public il2cpp_class_has_parent_delegate il2cpp_class_has_parent { get; } // IL2CPP Versions: All
        public il2cpp_class_from_il2cpp_type_delegate il2cpp_class_from_il2cpp_type { get; } // IL2CPP Versions: All
        public il2cpp_class_from_name_delegate il2cpp_class_from_name { get; } // IL2CPP Versions: All
        public il2cpp_class_from_system_type_delegate il2cpp_class_from_system_type { get; } // IL2CPP Versions: All
        public il2cpp_class_get_element_class_delegate il2cpp_class_get_element_class { get; } // IL2CPP Versions: All
        public il2cpp_class_get_events_delegate il2cpp_class_get_events { get; } // IL2CPP Versions: All
        public il2cpp_class_get_fields_delegate il2cpp_class_get_fields { get; } // IL2CPP Versions: All
        public il2cpp_class_get_interfaces_delegate il2cpp_class_get_interfaces { get; } // IL2CPP Versions: All
        public il2cpp_class_get_properties_delegate il2cpp_class_get_properties { get; } // IL2CPP Versions: All
        public il2cpp_class_get_property_from_name_delegate il2cpp_class_get_property_from_name { get; } // IL2CPP Versions: All
        public il2cpp_class_get_field_from_name_delegate il2cpp_class_get_field_from_name { get; } // IL2CPP Versions: All
        public il2cpp_class_get_methods_delegate il2cpp_class_get_methods { get; } // IL2CPP Versions: All
        public il2cpp_class_get_method_from_name_delegate il2cpp_class_get_method_from_name { get; } // IL2CPP Versions: All
        public il2cpp_class_get_name_delegate il2cpp_class_get_name { get; } // IL2CPP Versions: All
        public il2cpp_class_get_namespace_delegate il2cpp_class_get_namespace { get; } // IL2CPP Versions: All
        public il2cpp_class_get_parent_delegate il2cpp_class_get_parent { get; } // IL2CPP Versions: All
        public il2cpp_class_get_declaring_type_delegate il2cpp_class_get_declaring_type { get; } // IL2CPP Versions: All
        public il2cpp_class_instance_size_delegate il2cpp_class_instance_size { get; } // IL2CPP Versions: All
        public il2cpp_class_num_fields_delegate il2cpp_class_num_fields { get; } // IL2CPP Versions: All
        public il2cpp_class_is_valuetype_delegate il2cpp_class_is_valuetype { get; } // IL2CPP Versions: All
        public il2cpp_class_value_size_delegate il2cpp_class_value_size { get; } // IL2CPP Versions: All
        public il2cpp_class_get_flags_delegate il2cpp_class_get_flags { get; } // IL2CPP Versions: All
        public il2cpp_class_is_abstract_delegate il2cpp_class_is_abstract { get; } // IL2CPP Versions: All
        public il2cpp_class_is_interface_delegate il2cpp_class_is_interface { get; } // IL2CPP Versions: All
        public il2cpp_class_array_element_size_delegate il2cpp_class_array_element_size { get; } // IL2CPP Versions: All
        public il2cpp_class_from_type_delegate il2cpp_class_from_type { get; } // IL2CPP Versions: All
        public il2cpp_class_get_type_delegate il2cpp_class_get_type { get; } // IL2CPP Versions: All
        public il2cpp_class_has_attribute_delegate il2cpp_class_has_attribute { get; } // IL2CPP Versions: All
        public il2cpp_class_has_references_delegate il2cpp_class_has_references { get; } // IL2CPP Versions: All
        public il2cpp_class_is_enum_delegate il2cpp_class_is_enum { get; } // IL2CPP Versions: All
        public il2cpp_class_get_image_delegate il2cpp_class_get_image { get; } // IL2CPP Versions: All
        public il2cpp_class_get_assemblyname_delegate il2cpp_class_get_assemblyname { get; } // IL2CPP Versions: All
        public il2cpp_class_get_bitmap_size_delegate il2cpp_class_get_bitmap_size { get; } // IL2CPP Versions: All
        public il2cpp_class_get_bitmap_delegate il2cpp_class_get_bitmap { get; } // IL2CPP Versions: All
        public il2cpp_stats_dump_to_file_delegate il2cpp_stats_dump_to_file { get; } // IL2CPP Versions: All
        public il2cpp_stats_get_value_delegate il2cpp_stats_get_value { get; } // IL2CPP Versions: All
        public il2cpp_domain_get_delegate il2cpp_domain_get { get; } // IL2CPP Versions: All
        public il2cpp_domain_assembly_open_delegate il2cpp_domain_assembly_open { get; } // IL2CPP Versions: All
        public il2cpp_domain_get_assemblies_delegate il2cpp_domain_get_assemblies { get; } // IL2CPP Versions: All
        public il2cpp_raise_exception_delegate il2cpp_raise_exception { get; } // IL2CPP Versions: All
        public il2cpp_exception_from_name_msg_delegate il2cpp_exception_from_name_msg { get; } // IL2CPP Versions: All
        public il2cpp_get_exception_argument_null_delegate il2cpp_get_exception_argument_null { get; } // IL2CPP Versions: All
        public il2cpp_format_exception_delegate il2cpp_format_exception { get; } // IL2CPP Versions: All
        public il2cpp_format_stack_trace_delegate il2cpp_format_stack_trace { get; } // IL2CPP Versions: All
        public il2cpp_unhandled_exception_delegate il2cpp_unhandled_exception { get; } // IL2CPP Versions: All
        public il2cpp_field_get_flags_delegate il2cpp_field_get_flags { get; } // IL2CPP Versions: All
        public il2cpp_field_get_name_delegate il2cpp_field_get_name { get; } // IL2CPP Versions: All
        public il2cpp_field_get_parent_delegate il2cpp_field_get_parent { get; } // IL2CPP Versions: All
        public il2cpp_field_get_offset_delegate il2cpp_field_get_offset { get; } // IL2CPP Versions: All
        public il2cpp_field_get_type_delegate il2cpp_field_get_type { get; } // IL2CPP Versions: All
        public il2cpp_field_get_value_delegate il2cpp_field_get_value { get; } // IL2CPP Versions: All
        public il2cpp_field_get_value_object_delegate il2cpp_field_get_value_object { get; } // IL2CPP Versions: All
        public il2cpp_field_has_attribute_delegate il2cpp_field_has_attribute { get; } // IL2CPP Versions: All
        public il2cpp_field_set_value_delegate il2cpp_field_set_value { get; } // IL2CPP Versions: All
        public il2cpp_field_static_get_value_delegate il2cpp_field_static_get_value { get; } // IL2CPP Versions: All
        public il2cpp_field_static_set_value_delegate il2cpp_field_static_set_value { get; } // IL2CPP Versions: All
        public il2cpp_gc_collect_delegate il2cpp_gc_collect { get; } // IL2CPP Versions: All
        public il2cpp_gc_get_used_size_delegate il2cpp_gc_get_used_size { get; } // IL2CPP Versions: All
        public il2cpp_gc_get_heap_size_delegate il2cpp_gc_get_heap_size { get; } // IL2CPP Versions: All
        public il2cpp_gchandle_new_delegate il2cpp_gchandle_new { get; } // IL2CPP Versions: All
        public il2cpp_gchandle_new_weakref_delegate il2cpp_gchandle_new_weakref { get; } // IL2CPP Versions: All
        public il2cpp_gchandle_get_target_delegate il2cpp_gchandle_get_target { get; } // IL2CPP Versions: All
        public il2cpp_gchandle_free_delegate il2cpp_gchandle_free { get; } // IL2CPP Versions: All
        public il2cpp_unity_liveness_calculation_begin_delegate il2cpp_unity_liveness_calculation_begin { get; } // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
        public il2cpp_unity_liveness_calculation_end_delegate il2cpp_unity_liveness_calculation_end { get; } // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
        public il2cpp_unity_liveness_calculation_from_root_delegate il2cpp_unity_liveness_calculation_from_root { get; } // IL2CPP Versions: All
        public il2cpp_unity_liveness_calculation_from_statics_delegate il2cpp_unity_liveness_calculation_from_statics { get; } // IL2CPP Versions: All
        public il2cpp_method_get_return_type_delegate il2cpp_method_get_return_type { get; } // IL2CPP Versions: All
        public il2cpp_method_get_declaring_type_delegate il2cpp_method_get_declaring_type { get; } // IL2CPP Versions: All
        public il2cpp_method_get_name_delegate il2cpp_method_get_name { get; } // IL2CPP Versions: All
        public il2cpp_method_get_object_delegate il2cpp_method_get_object { get; } // IL2CPP Versions: All
        public il2cpp_method_is_generic_delegate il2cpp_method_is_generic { get; } // IL2CPP Versions: All
        public il2cpp_method_is_inflated_delegate il2cpp_method_is_inflated { get; } // IL2CPP Versions: All
        public il2cpp_method_is_instance_delegate il2cpp_method_is_instance { get; } // IL2CPP Versions: All
        public il2cpp_method_get_param_count_delegate il2cpp_method_get_param_count { get; } // IL2CPP Versions: All
        public il2cpp_method_get_param_delegate il2cpp_method_get_param { get; } // IL2CPP Versions: All
        public il2cpp_method_get_class_delegate il2cpp_method_get_class { get; } // IL2CPP Versions: All
        public il2cpp_method_has_attribute_delegate il2cpp_method_has_attribute { get; } // IL2CPP Versions: All
        public il2cpp_method_get_flags_delegate il2cpp_method_get_flags { get; } // IL2CPP Versions: All
        public il2cpp_method_get_token_delegate il2cpp_method_get_token { get; } // IL2CPP Versions: All
        public il2cpp_method_get_param_name_delegate il2cpp_method_get_param_name { get; } // IL2CPP Versions: All
        public il2cpp_profiler_install_delegate il2cpp_profiler_install { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_profiler_set_events_delegate il2cpp_profiler_set_events { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_profiler_install_enter_leave_delegate il2cpp_profiler_install_enter_leave { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_profiler_install_allocation_delegate il2cpp_profiler_install_allocation { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_profiler_install_gc_delegate il2cpp_profiler_install_gc { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_property_get_flags_delegate il2cpp_property_get_flags { get; } // IL2CPP Versions: All
        public il2cpp_property_get_get_method_delegate il2cpp_property_get_get_method { get; } // IL2CPP Versions: All
        public il2cpp_property_get_set_method_delegate il2cpp_property_get_set_method { get; } // IL2CPP Versions: All
        public il2cpp_property_get_name_delegate il2cpp_property_get_name { get; } // IL2CPP Versions: All
        public il2cpp_property_get_parent_delegate il2cpp_property_get_parent { get; } // IL2CPP Versions: All
        public il2cpp_object_get_class_delegate il2cpp_object_get_class { get; } // IL2CPP Versions: All
        public il2cpp_object_get_size_delegate il2cpp_object_get_size { get; } // IL2CPP Versions: All
        public il2cpp_object_get_virtual_method_delegate il2cpp_object_get_virtual_method { get; } // IL2CPP Versions: All
        public il2cpp_object_new_delegate il2cpp_object_new { get; } // IL2CPP Versions: All
        public il2cpp_object_unbox_delegate il2cpp_object_unbox { get; } // IL2CPP Versions: All
        public il2cpp_value_box_delegate il2cpp_value_box { get; } // IL2CPP Versions: All
        public il2cpp_monitor_enter_delegate il2cpp_monitor_enter { get; } // IL2CPP Versions: All
        public il2cpp_monitor_try_enter_delegate il2cpp_monitor_try_enter { get; } // IL2CPP Versions: All
        public il2cpp_monitor_exit_delegate il2cpp_monitor_exit { get; } // IL2CPP Versions: All
        public il2cpp_monitor_pulse_delegate il2cpp_monitor_pulse { get; } // IL2CPP Versions: All
        public il2cpp_monitor_pulse_all_delegate il2cpp_monitor_pulse_all { get; } // IL2CPP Versions: All
        public il2cpp_monitor_wait_delegate il2cpp_monitor_wait { get; } // IL2CPP Versions: All
        public il2cpp_monitor_try_wait_delegate il2cpp_monitor_try_wait { get; } // IL2CPP Versions: All
        public il2cpp_runtime_invoke_delegate il2cpp_runtime_invoke { get; } // IL2CPP Versions: All
        public il2cpp_runtime_invoke_convert_args_delegate il2cpp_runtime_invoke_convert_args { get; } // IL2CPP Versions: All
        public il2cpp_runtime_class_init_delegate il2cpp_runtime_class_init { get; } // IL2CPP Versions: All
        public il2cpp_runtime_object_init_delegate il2cpp_runtime_object_init { get; } // IL2CPP Versions: All
        public il2cpp_runtime_object_init_exception_delegate il2cpp_runtime_object_init_exception { get; } // IL2CPP Versions: All
        public il2cpp_runtime_unhandled_exception_policy_set_delegate il2cpp_runtime_unhandled_exception_policy_set { get; } // IL2CPP Versions: All
        public il2cpp_delegate_begin_invoke_delegate il2cpp_delegate_begin_invoke { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_delegate_end_invoke_delegate il2cpp_delegate_end_invoke { get; } // IL2CPP Versions: 16, 18, 19, 20, 21
        public il2cpp_string_length_delegate il2cpp_string_length { get; } // IL2CPP Versions: All
        public il2cpp_string_chars_delegate il2cpp_string_chars { get; } // IL2CPP Versions: All
        public il2cpp_string_new_delegate il2cpp_string_new { get; } // IL2CPP Versions: All
        public il2cpp_string_new_len_delegate il2cpp_string_new_len { get; } // IL2CPP Versions: All
        public il2cpp_string_new_utf16_delegate il2cpp_string_new_utf16 { get; } // IL2CPP Versions: All
        public il2cpp_string_new_wrapper_delegate il2cpp_string_new_wrapper { get; } // IL2CPP Versions: All
        public il2cpp_string_intern_delegate il2cpp_string_intern { get; } // IL2CPP Versions: All
        public il2cpp_string_is_interned_delegate il2cpp_string_is_interned { get; } // IL2CPP Versions: All
        public il2cpp_thread_get_name_delegate il2cpp_thread_get_name { get; } // IL2CPP Versions: 16, 18, 19, 20, 21, 22, 23, 24
        public il2cpp_thread_current_delegate il2cpp_thread_current { get; } // IL2CPP Versions: All
        public il2cpp_thread_attach_delegate il2cpp_thread_attach { get; } // IL2CPP Versions: All
        public il2cpp_thread_detach_delegate il2cpp_thread_detach { get; } // IL2CPP Versions: All
        public il2cpp_thread_get_all_attached_threads_delegate il2cpp_thread_get_all_attached_threads { get; } // IL2CPP Versions: All
        public il2cpp_is_vm_thread_delegate il2cpp_is_vm_thread { get; } // IL2CPP Versions: All
        public il2cpp_current_thread_walk_frame_stack_delegate il2cpp_current_thread_walk_frame_stack { get; } // IL2CPP Versions: All
        public il2cpp_thread_walk_frame_stack_delegate il2cpp_thread_walk_frame_stack { get; } // IL2CPP Versions: All
        public il2cpp_current_thread_get_top_frame_delegate il2cpp_current_thread_get_top_frame { get; } // IL2CPP Versions: All
        public il2cpp_thread_get_top_frame_delegate il2cpp_thread_get_top_frame { get; } // IL2CPP Versions: All
        public il2cpp_current_thread_get_frame_at_delegate il2cpp_current_thread_get_frame_at { get; } // IL2CPP Versions: All
        public il2cpp_thread_get_frame_at_delegate il2cpp_thread_get_frame_at { get; } // IL2CPP Versions: All
        public il2cpp_current_thread_get_stack_depth_delegate il2cpp_current_thread_get_stack_depth { get; } // IL2CPP Versions: All
        public il2cpp_thread_get_stack_depth_delegate il2cpp_thread_get_stack_depth { get; } // IL2CPP Versions: All
        public il2cpp_type_get_object_delegate il2cpp_type_get_object { get; } // IL2CPP Versions: All
        public il2cpp_type_get_type_delegate il2cpp_type_get_type { get; } // IL2CPP Versions: All
        public il2cpp_type_get_class_or_element_class_delegate il2cpp_type_get_class_or_element_class { get; } // IL2CPP Versions: All
        public il2cpp_type_get_name_delegate il2cpp_type_get_name { get; } // IL2CPP Versions: All
        public il2cpp_image_get_assembly_delegate il2cpp_image_get_assembly { get; } // IL2CPP Versions: All
        public il2cpp_image_get_name_delegate il2cpp_image_get_name { get; } // IL2CPP Versions: All
        public il2cpp_image_get_filename_delegate il2cpp_image_get_filename { get; } // IL2CPP Versions: All
        public il2cpp_image_get_entry_point_delegate il2cpp_image_get_entry_point { get; } // IL2CPP Versions: All
        public il2cpp_capture_memory_snapshot_delegate il2cpp_capture_memory_snapshot { get; } // IL2CPP Versions: All
        public il2cpp_free_captured_memory_snapshot_delegate il2cpp_free_captured_memory_snapshot { get; } // IL2CPP Versions: All
        public il2cpp_set_find_plugin_callback_delegate il2cpp_set_find_plugin_callback { get; } // IL2CPP Versions: 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_class_get_nested_types_delegate il2cpp_class_get_nested_types { get; } // IL2CPP Versions: 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_gc_collect_a_little_delegate il2cpp_gc_collect_a_little { get; } // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_gc_disable_delegate il2cpp_gc_disable { get; } // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_gc_enable_delegate il2cpp_gc_enable { get; } // IL2CPP Versions: 21, 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_init_utf16_delegate il2cpp_init_utf16 { get; } // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_set_commandline_arguments_utf16_delegate il2cpp_set_commandline_arguments_utf16 { get; } // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_set_config_utf16_delegate il2cpp_set_config_utf16 { get; } // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_set_config_delegate il2cpp_set_config { get; } // IL2CPP Versions: 22, 23, 24, 25, 26, 27, 28, 29
        public il2cpp_field_set_value_object_delegate il2cpp_field_set_value_object { get; } // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29
        public il2cpp_register_log_callback_delegate il2cpp_register_log_callback { get; } // IL2CPP Versions: 23, 24, 25, 26, 27, 28, 29
        public il2cpp_set_temp_dir_delegate il2cpp_set_temp_dir { get; } // IL2CPP Versions: 24, 25, 26, 27, 28, 29
        public il2cpp_class_is_blittable_delegate il2cpp_class_is_blittable { get; } // IL2CPP Versions: 24, 25, 26, 27, 28, 29
        public il2cpp_class_for_each_delegate il2cpp_class_for_each { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_get_name_chunked_delegate il2cpp_type_get_name_chunked { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_get_type_token_delegate il2cpp_class_get_type_token { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_get_rank_delegate il2cpp_class_get_rank { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_get_data_size_delegate il2cpp_class_get_data_size { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_get_static_field_data_delegate il2cpp_class_get_static_field_data { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_native_stack_trace_delegate il2cpp_native_stack_trace { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_field_is_literal_delegate il2cpp_field_is_literal { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_is_disabled_delegate il2cpp_gc_is_disabled { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_get_max_time_slice_ns_delegate il2cpp_gc_get_max_time_slice_ns { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_set_max_time_slice_ns_delegate il2cpp_gc_set_max_time_slice_ns { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_is_incremental_delegate il2cpp_gc_is_incremental { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_wbarrier_set_field_delegate il2cpp_gc_wbarrier_set_field { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_has_strict_wbarriers_delegate il2cpp_gc_has_strict_wbarriers { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_set_external_allocation_tracker_delegate il2cpp_gc_set_external_allocation_tracker { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_set_external_wbarrier_tracker_delegate il2cpp_gc_set_external_wbarrier_tracker { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_foreach_heap_delegate il2cpp_gc_foreach_heap { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_stop_gc_world_delegate il2cpp_stop_gc_world { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_start_gc_world_delegate il2cpp_start_gc_world { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gchandle_foreach_get_target_delegate il2cpp_gchandle_foreach_get_target { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_object_header_size_delegate il2cpp_object_header_size { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_array_object_header_size_delegate il2cpp_array_object_header_size { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_offset_of_array_length_in_array_object_header_delegate il2cpp_offset_of_array_length_in_array_object_header { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_offset_of_array_bounds_in_array_object_header_delegate il2cpp_offset_of_array_bounds_in_array_object_header { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_allocation_granularity_delegate il2cpp_allocation_granularity { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_method_get_from_reflection_delegate il2cpp_method_get_from_reflection { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_override_stack_backtrace_delegate il2cpp_override_stack_backtrace { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_is_byref_delegate il2cpp_type_is_byref { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_get_attrs_delegate il2cpp_type_get_attrs { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_equals_delegate il2cpp_type_equals { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_get_assembly_qualified_name_delegate il2cpp_type_get_assembly_qualified_name { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_is_static_delegate il2cpp_type_is_static { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_type_is_pointer_type_delegate il2cpp_type_is_pointer_type { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_image_get_class_count_delegate il2cpp_image_get_class_count { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_image_get_class_delegate il2cpp_image_get_class { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_debugger_set_agent_options_delegate il2cpp_debugger_set_agent_options { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_is_debugger_attached_delegate il2cpp_is_debugger_attached { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_register_debugger_agent_transport_delegate il2cpp_register_debugger_agent_transport { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_debug_get_method_info_delegate il2cpp_debug_get_method_info { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_unity_install_unitytls_interface_delegate il2cpp_unity_install_unitytls_interface { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_from_class_delegate il2cpp_custom_attrs_from_class { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_from_method_delegate il2cpp_custom_attrs_from_method { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_get_attr_delegate il2cpp_custom_attrs_get_attr { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_has_attr_delegate il2cpp_custom_attrs_has_attr { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_construct_delegate il2cpp_custom_attrs_construct { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_custom_attrs_free_delegate il2cpp_custom_attrs_free { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_set_userdata_delegate il2cpp_class_set_userdata { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_class_get_userdata_offset_delegate il2cpp_class_get_userdata_offset { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_set_default_thread_affinity_delegate il2cpp_set_default_thread_affinity { get; } // IL2CPP Versions: 25, 26, 27, 28, 29
        public il2cpp_gc_start_incremental_collection_delegate il2cpp_gc_start_incremental_collection { get; } // IL2CPP Versions: 27, 28, 29
        public il2cpp_gc_set_mode_delegate il2cpp_gc_set_mode { get; } // IL2CPP Versions: 27, 28, 29
        public il2cpp_unity_liveness_allocate_struct_delegate il2cpp_unity_liveness_allocate_struct { get; } // IL2CPP Versions: 28, 29
        public il2cpp_unity_liveness_finalize_delegate il2cpp_unity_liveness_finalize { get; } // IL2CPP Versions: 28, 29
        public il2cpp_unity_liveness_free_struct_delegate il2cpp_unity_liveness_free_struct { get; } // IL2CPP Versions: 28, 29
        public il2cpp_gc_alloc_fixed_delegate il2cpp_gc_alloc_fixed { get; } // IL2CPP Versions: 29
        public il2cpp_gc_free_fixed_delegate il2cpp_gc_free_fixed { get; } // IL2CPP Versions: 29

        #endregion
    }
}
#pragma warning restore IDE1006 // Naming Styles