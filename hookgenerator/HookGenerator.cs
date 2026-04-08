using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace xarsu.HookGenerator;

[Generator]
public class HookGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methods = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "xarsu.Reference.Il2CppHookAttribute",
                predicate: (node, _) => node is MethodDeclarationSyntax,
                transform: (ctx, _) => GetHookInfo(ctx));

        context.RegisterSourceOutput(methods.Collect(), Generate);
    }

    private static HookInfo? GetHookInfo(GeneratorAttributeSyntaxContext ctx)
    {
        if (ctx.TargetSymbol is not IMethodSymbol method) return null;
        var attr = ctx.Attributes[0];

        if (attr.ConstructorArguments[0].Value is not INamedTypeSymbol declaringType) return null;
        string il2cppMethodName = attr.ConstructorArguments[1].Value?.ToString() ?? "";

        var il2cppMethod = declaringType.GetMembers(il2cppMethodName)
            .OfType<IMethodSymbol>()
            .FirstOrDefault();

        bool isStatic = il2cppMethod?.IsStatic ?? true;
        string? instanceTypeName = isStatic ? null : declaringType.ToDisplayString();

        return new HookInfo(
            ContainingClass: method.ContainingType.ToDisplayString(),
            HookMethodName: method.Name,
            Parameters: [.. method.Parameters],
            ReturnType: method.ReturnType.ToDisplayString(),
            ReturnTypeSymbol: method.ReturnType,
            Il2CppDeclaringType: declaringType.ToDisplayString(),
            Il2CppMethodName: il2cppMethodName,
            IsStatic: isStatic,
            InstanceTypeName: instanceTypeName
        );
    }

    // returns the raw delegate parameter type for a given parameter type symbol
    private static string GetRawParamType(ITypeSymbol type)
    {
        if (!type.IsValueType)
            return "System.IntPtr"; // reference types/objects passed as pointer

        if (IsPrimitive(type))
            return type.ToDisplayString(); // primitives passed as-is

        if (type.TypeKind == TypeKind.Enum)
        {
            // use the underlying enum type
            var enumType = ((INamedTypeSymbol)type).EnumUnderlyingType;
            return enumType?.ToDisplayString() ?? "System.Int32";
        }

        if (type.TypeKind == TypeKind.Struct)
            return "System.IntPtr"; // structs passed as pointer

        return "System.IntPtr";
    }

    // generates the expression to convert a raw param to the user-facing type
    private static string GetConvertToUserType(ITypeSymbol type, string paramName)
    {
        if (!type.IsValueType)
            return $"xarsu.Reference.Il2CppObject.Wrap<{type.ToDisplayString()}>({paramName})";

        if (IsPrimitive(type))
            return paramName; // no conversion needed

        if (type.TypeKind == TypeKind.Enum)
            return $"({type.ToDisplayString()}){paramName}"; // cast from underlying int

        if (type.TypeKind == TypeKind.Struct)
        {
            // check if IIl2CppStruct — use Read, otherwise treat as blittable
            if (ImplementsIl2CppStruct(type))
                return $"{type.ToDisplayString()}.Read({paramName})";
            else
                return $"*({type.ToDisplayString()}*)({paramName})"; // blittable struct
        }

        return paramName;
    }

    // generates the expression to convert a user-facing type back to raw
    private static string GetConvertFromUserType(ITypeSymbol type, string paramName)
    {
        if (!type.IsValueType)
            return $"{paramName}?.Pointer.Value ?? System.IntPtr.Zero";

        if (IsPrimitive(type))
            return paramName;

        if (type.TypeKind == TypeKind.Enum)
            return $"(System.Int32){paramName}";

        if (type.TypeKind == TypeKind.Struct)
        {
            if (ImplementsIl2CppStruct(type))
                return $"{type.ToDisplayString()}.WriteToNativeStatic({paramName})"; // returns IntPtr
            else
                return $"(System.IntPtr)(unsafe {{ System.Runtime.CompilerServices.Unsafe.AsPointer(ref {paramName}) }})";
        }

        return paramName;
    }

    private static bool IsPrimitive(ITypeSymbol type)
    {
        return type.SpecialType is
            SpecialType.System_Boolean or
            SpecialType.System_Byte or
            SpecialType.System_SByte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 or
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_Char or
            SpecialType.System_IntPtr or
            SpecialType.System_UIntPtr;
    }

    private static bool ImplementsIl2CppStruct(ITypeSymbol type)
        => type.AllInterfaces.Any(i => i.Name == "IIl2CppStruct");

    private static void Generate(SourceProductionContext ctx, ImmutableArray<HookInfo?> hooks)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#pragma warning disable CS8600, CS8602, CS8603, CS8604");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Runtime.InteropServices;");
        sb.AppendLine("using xarsu.Reference;");
        sb.AppendLine();

        foreach (var group in hooks.Where(h => h != null).GroupBy(h => h!.ContainingClass))
        {
            string ns = GetNamespace(group.Key);
            string className = GetClassName(group.Key);

            sb.AppendLine($"namespace {ns} {{");
            sb.AppendLine($"partial class {className} {{");

            foreach (var hook in group)
            {
                string delegateName = $"{hook!.HookMethodName}_Delegate";
                string hookFieldName = $"{hook.HookMethodName}_Hook";
                string trampolineName = $"{hook.HookMethodName}_Original";

                // build raw params list (what Dobby sees)
                var rawParams = new List<string>();
                if (!hook.IsStatic)
                    rawParams.Add("System.IntPtr __instancePtr");

                var hookParams = hook.Parameters.Skip(hook.IsStatic ? 0 : 1).ToList();
                foreach (var param in hookParams)
                    rawParams.Add($"{GetRawParamType(param.Type)} {param.Name}");

                string rawParamList = string.Join(", ", rawParams);

                // build user-facing params list (what the hook method sees)
                var userParams = new List<string>();
                if (!hook.IsStatic)
                    userParams.Add($"{hook.InstanceTypeName}? {hook.Parameters[0].Name}");
                foreach (var param in hookParams)
                    userParams.Add($"{param.Type.ToDisplayString()} {param.Name}");

                string userParamList = string.Join(", ", userParams);

                // ----- delegate -----
                sb.AppendLine($"    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
                sb.AppendLine($"    public delegate {GetRawReturnType(hook.ReturnTypeSymbol)} {delegateName}({rawParamList});");
                sb.AppendLine();

                // ----- hook field -----
                sb.AppendLine($"    public static xarsu.Reference.Il2CppHook<{delegateName}>? {hookFieldName};");
                sb.AppendLine();

                // ----- Original wrapper (user-facing, handles conversion back to raw) -----
                sb.AppendLine($"    public static {hook.ReturnType} {trampolineName}({userParamList}) {{");
                var trampolineArgs = new List<string>();
                if (!hook.IsStatic)
                    trampolineArgs.Add($"{hook.Parameters[0].Name}?.Pointer.Value ?? System.IntPtr.Zero");
                foreach (var param in hookParams)
                    trampolineArgs.Add(GetConvertFromUserType(param.Type, param.Name));

                string trampolineArgStr = string.Join(", ", trampolineArgs);
                if (hook.ReturnType != "void")
                {
                    string rawInvoke = $"{hookFieldName}?.Original?.Invoke({trampolineArgStr}) ?? default";
                    sb.AppendLine($"        return {GetConvertToUserType(hook.ReturnTypeSymbol, $"({rawInvoke})")};");
                }
                else
                    sb.AppendLine($"        {hookFieldName}?.Original?.Invoke({trampolineArgStr});");
                sb.AppendLine($"    }}");
                sb.AppendLine();

                // ----- Detour (what Dobby calls, converts to user types then calls hook method) -----
                sb.AppendLine($"    private static {GetRawReturnType(hook.ReturnTypeSymbol)} {hook.HookMethodName}_Detour({rawParamList}) {{");

                // convert args from raw to user types
                if (!hook.IsStatic)
                    sb.AppendLine($"        var {hook.Parameters[0].Name} = __instancePtr.AsIl2CppOrNull<{hook.InstanceTypeName}>();");

                foreach (var param in hookParams)
                {
                    string converted = GetConvertToUserType(param.Type, param.Name);
                    if (converted != param.Name) // only emit conversion if needed
                        sb.AppendLine($"        var __{param.Name} = {converted};");
                }

                // build call args to user's hook method
                var callArgs = new List<string>();
                if (!hook.IsStatic)
                    callArgs.Add(hook.Parameters[0].Name);
                foreach (var param in hookParams)
                {
                    string converted = GetConvertToUserType(param.Type, param.Name);
                    callArgs.Add(converted != param.Name ? $"__{param.Name}" : param.Name);
                }

                string callArgStr = string.Join(", ", callArgs);

                if (hook.ReturnType != "void")
                {
                    // convert return value back to raw
                    sb.AppendLine($"        var __result = {hook.HookMethodName}({callArgStr});");
                    sb.AppendLine($"        return {GetConvertFromUserType(hook.ReturnTypeSymbol, "__result")};");
                }
                else
                    sb.AppendLine($"        {hook.HookMethodName}({callArgStr});");

                sb.AppendLine($"    }}");
                sb.AppendLine();

                // ----- Install -----
                sb.AppendLine($"    public static void Install_{hook.HookMethodName}() {{");
                sb.AppendLine($"        var __method = xarsu.Reference.IL2CPP.GetIl2CppMethodPointer(typeof({hook.Il2CppDeclaringType}).GetMethod(\"{hook.Il2CppMethodName}\"));");
                sb.AppendLine($"        {hookFieldName} = xarsu.Reference.Il2CppHook.Install<{delegateName}>(__method, {hook.HookMethodName}_Detour);");
                sb.AppendLine($"    }}");
                sb.AppendLine();
            }

            sb.AppendLine("}");
            sb.AppendLine("}");
        }

        sb.AppendLine("#pragma warning restore CS8600, CS8602, CS8603, CS8604");
        sb.AppendLine("#nullable disable");

        ctx.AddSource("Hooks.g.cs", sb.ToString());
    }

    private static string GetRawReturnType(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Void) return "void";
        return GetRawParamType(type);
    }

    private static string GetNamespace(string fullName)
    {
        int last = fullName.LastIndexOf('.');
        return last >= 0 ? fullName[..last] : "global";
    }

    private static string GetClassName(string fullName)
    {
        int last = fullName.LastIndexOf('.');
        return last >= 0 ? fullName[(last + 1)..] : fullName;
    }

    record HookInfo(
        string ContainingClass,
        string HookMethodName,
        IParameterSymbol[] Parameters,
        string ReturnType,
        ITypeSymbol ReturnTypeSymbol,
        string Il2CppDeclaringType,
        string Il2CppMethodName,
        bool IsStatic,
        string? InstanceTypeName);
}