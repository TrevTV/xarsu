using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using Cpp2IL.Core;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.Utils;
using Cpp2IL.Core.Utils.AsmResolver;
using System.Reflection;
using xarsu.Generator.Extensions;

namespace xarsu.Generator.ProcessingLayers;

internal class FieldAccessorProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Field Accessors";

    public override string Id => "field_accessors";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected || type.IsValueType) // structs don't need accessors
                    continue;

                foreach (var field in type.Fields)
                {
                    if (field.IsInjected)
                        continue;

                    // fields are replaced with properties
                    var getter = GenerateGetterMethod(field, type);
                    var setter = GenerateSetterMethod(field, type);

                    var property = new InjectedPropertyAnalysisContext(
                        field.Name,
                        field.FieldType,
                        getter,
                        setter,
                        PropertyAttributes.None,
                        type);

                    type.Methods.Add(getter);
                    type.Methods.Add(setter);

                    type.Properties.Add(property);
                }

                type.Fields.Clear(); // remove the original fields since they're now properties
            }
        }
    }

    private static InjectedMethodAnalysisContext GenerateGetterMethod(FieldAnalysisContext field, TypeAnalysisContext declaringType)
    {
        var appContext = Cpp2IlApi.CurrentAppContext!;

        var xarsuIl2CppStaticClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.IL2CPP));
        var il2cppGetIl2CppClass = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppClass));
        var il2cppGetIl2CppField = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppField));
        var il2cppReadField = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.ReadField)).MakeGenericInstanceMethod(field.FieldType);

        var xarsuIl2CppObjectClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.Il2CppObject));
        var il2cppObjectGetPointer = xarsuIl2CppObjectClass.GetPropertyByName(nameof(xarsu.Reference.Il2CppObject.Pointer)).Getter!;
        var il2cppObjectWrap = xarsuIl2CppObjectClass.Methods.First(m => m.Name == nameof(xarsu.Reference.Il2CppObject.Wrap) && m.GenericParameters.Count == 1)!.MakeConcreteGenericMethod([], [field.FieldType]);

        var xarsuObjectPointerClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.ObjectPointer));
        var objPointerExplicitFromIntPtr = xarsuObjectPointerClass.GetExplicitConversionFrom(appContext.SystemTypes.SystemIntPtrType);
        var objPointerExplicitIntPtr = xarsuObjectPointerClass.GetExplicitConversionTo(appContext.SystemTypes.SystemIntPtrType);

        var getter = new InjectedMethodAnalysisContext(
            declaringType,
            $"get_{field.Name}",
            field.FieldType,
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            []);
        getter.IsInjected = true;

        // compute the data we need for the GetIl2CppMethod call
        string assemblyName = MiscUtils.CleanPathElement(declaringType.DeclaringAssembly.DefaultName) + ".dll";
        string namespaceName = declaringType.DefaultNamespace ?? "";
        string className = declaringType.DefaultName ?? "";
        string fieldName = field.DefaultName ?? "";
        string fieldTypeName = GetTypeName(field.FieldType);

        Operands.Instruction instanceOp = field.IsStatic
            ? new(CilOpCodes.Ldnull) // static fields don't need an instance
            : new(CilOpCodes.Ldarg_0); // load the instance for non-static fields

        List<Operands.Instruction> instructions =
        [
            // TODO: nested type handling
            // call GetIl2CppClass to get the class pointer
            new(CilOpCodes.Ldstr, assemblyName),
            new(CilOpCodes.Ldstr, namespaceName),
            new(CilOpCodes.Ldstr, className),
            new(CilOpCodes.Call, il2cppGetIl2CppClass),
            // call GetIl2CppField to get the field pointer
            new(CilOpCodes.Ldstr, fieldName),
            new(CilOpCodes.Call, il2cppGetIl2CppField),
        ];

        // load the instance pointer (or null for static fields)
        if (field.IsStatic)
            instructions.Add(new(CilOpCodes.Ldnull));
        else
        {
            // Il2CppObject.Pointer (casted to IntPtr)
            instructions.Add(new(CilOpCodes.Ldarg_0));
            instructions.Add(new(CilOpCodes.Callvirt, il2cppObjectGetPointer));
            instructions.Add(new(CilOpCodes.Call, objPointerExplicitIntPtr));
        }

        // call ReadField
        instructions.Add(new(CilOpCodes.Call, il2cppReadField));
        instructions.Add(new(CilOpCodes.Ret));

        getter.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions
        });

        return getter;
    }

    private static InjectedMethodAnalysisContext GenerateSetterMethod(FieldAnalysisContext field, TypeAnalysisContext declaringType)
    {
        var appContext = Cpp2IlApi.CurrentAppContext!;

        var xarsuIl2CppStaticClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.IL2CPP));
        var il2cppGetIl2CppClass = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppClass));
        var il2cppGetIl2CppField = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppField));
        var il2cppWriteField = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.WriteField));

        var xarsuIl2CppObjectClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.Il2CppObject));
        var il2cppObjectGetPointer = xarsuIl2CppObjectClass.GetPropertyByName(nameof(xarsu.Reference.Il2CppObject.Pointer)).Getter!;
        var il2cppObjectWrap = xarsuIl2CppObjectClass.Methods.First(m => m.Name == nameof(xarsu.Reference.Il2CppObject.Wrap) && m.GenericParameters.Count == 1)!.MakeConcreteGenericMethod([], [field.FieldType]);

        var xarsuObjectPointerClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.ObjectPointer));
        var objPointerExplicitFromIntPtr = xarsuObjectPointerClass.GetExplicitConversionFrom(appContext.SystemTypes.SystemIntPtrType);
        var objPointerExplicitIntPtr = xarsuObjectPointerClass.GetExplicitConversionTo(appContext.SystemTypes.SystemIntPtrType);

        var setter = new InjectedMethodAnalysisContext(
            declaringType,
            $"set_{field.Name}",
            appContext.SystemTypes.SystemVoidType,
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            [field.FieldType],
            ["value"],
            [ParameterAttributes.None]);

        setter.IsInjected = true;

        // compute the data we need for the GetIl2CppMethod call
        string assemblyName = MiscUtils.CleanPathElement(declaringType.DeclaringAssembly.DefaultName) + ".dll";
        string namespaceName = declaringType.DefaultNamespace ?? "";
        string className = declaringType.DefaultName ?? "";
        string fieldName = field.DefaultName ?? "";
        string fieldTypeName = GetTypeName(field.FieldType);

        Operands.Instruction instanceOp = field.IsStatic
            ? new(CilOpCodes.Ldnull) // static fields don't need an instance
            : new(CilOpCodes.Ldarg_0); // load the instance for non-static fields

        List<Operands.Instruction> instructions =
        [
            // call GetIl2CppClass to get the class pointer
            new(CilOpCodes.Ldstr, assemblyName),
            new(CilOpCodes.Ldstr, namespaceName),
            new(CilOpCodes.Ldstr, className),
            new(CilOpCodes.Call, il2cppGetIl2CppClass),
            // call GetIl2CppField to get the field pointer
            new(CilOpCodes.Ldstr, fieldName),
            new(CilOpCodes.Call, il2cppGetIl2CppField),
        ];

        // load the instance pointer (or null for static fields)
        if (field.IsStatic)
        {
            instructions.Add(new(CilOpCodes.Ldnull));
            instructions.Add(new(CilOpCodes.Ldarg_0));
        }
        else
        {
            // Il2CppObject.Pointer (casted to IntPtr)
            instructions.Add(new(CilOpCodes.Ldarg_0));
            instructions.Add(new(CilOpCodes.Callvirt, il2cppObjectGetPointer));
            instructions.Add(new(CilOpCodes.Call, objPointerExplicitIntPtr));

            // load the value argument
            instructions.Add(new(CilOpCodes.Ldarg_1));
        }

        // box the value if it's a value type, since WriteField expects an object
        if (field.FieldType.IsValueType)
            instructions.Add(new(CilOpCodes.Box, field.FieldType));

        // call WriteField to write the field value
        instructions.Add(new(CilOpCodes.Call, il2cppWriteField));

        instructions.Add(new(CilOpCodes.Ret));

        setter.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions
        });

        return setter;
    }

    // TODO: move these to extensions or util class
    /// <summary>
    /// Gets a flattened il2cpp type name string matching what GetIl2CppMethod expects.
    /// Strips generic arity markers (`1, `2 etc.) to match the IL2CPP runtime name format.
    /// </summary>
    private static string GetTypeName(TypeAnalysisContext? ctx)
    {
        if (ctx == null) return "System.Void";
        // Use the full dotnet name, stripping generic backtick arity to match IL2CPP naming.
        var name = ctx.DefaultName ?? "System.Object";
        // Replace nested type separator and generic arity.
        name = System.Text.RegularExpressions.Regex.Replace(name, @"`\d+", "");
        name = name.Replace('/', '.').Replace('+', '.');
        return name;
    }
}