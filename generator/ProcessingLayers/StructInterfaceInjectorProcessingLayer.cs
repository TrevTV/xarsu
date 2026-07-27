using AsmResolver.PE.DotNet.Cil;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.Utils;
using System.Reflection;
using xarsu.Generator.Extensions;
using xarsu.Generator.Operands;
using xarsu.Reference;

namespace xarsu.Generator.ProcessingLayers;

internal class StructInterfaceInjectorProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Struct Interface Injector";

    public override string Id => "struct_interface_injector";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var xarsuIl2CppStruct = appContext.ResolveTypeOrThrow(typeof(IIl2CppStruct<>));

        string[] primitiveTypes = [
            "Il2CppSystem.SByte",
            "Il2CppSystem.Byte",
            "Il2CppSystem.Int16",
            "Il2CppSystem.UInt16",
            "Il2CppSystem.Int32",
            "Il2CppSystem.UInt32",
            "Il2CppSystem.Int64",
            "Il2CppSystem.UInt64",
            "Il2CppSystem.Single",
            "Il2CppSystem.Double",
            "Il2CppSystem.Char",
            "Il2CppSystem.Boolean",
            "Il2CppSystem.IntPtr",
            "Il2CppSystem.UIntPtr",
            "Il2CppSystem.String"
        ];

        // skipping primitives as they are handled special due to remapping
        var types = appContext.AllTypes.Where(t => t.IsValueType && !primitiveTypes.Contains(t.FullName)).ToList();

        foreach (var type in types)
        {
            var xarsuIl2CppStructGeneric = xarsuIl2CppStruct.MakeGenericInstanceType(type);
            var il2cppStructSize = xarsuIl2CppStruct.GetPropertyByName(nameof(IIl2CppStruct<>.Size));

            type.InterfaceContexts.Add(xarsuIl2CppStructGeneric);

            // add the size property
            var getter = GenerateSizeGetter(type, xarsuIl2CppStructGeneric, xarsuIl2CppStruct);
            var property = new InjectedPropertyAnalysisContext(
                $"{xarsuIl2CppStructGeneric.FullName}.{nameof(IIl2CppStruct<>.Size)}",
                il2cppStructSize.PropertyType,
                getter,
                null,
                PropertyAttributes.None,
                type
            );

            type.Methods.Add(getter);
            type.Properties.Add(property);

            string assemblyName = MiscUtils.CleanPathElement(type.DeclaringAssembly.DefaultName) + ".dll";
            string namespaceName = type.DefaultNamespace ?? "";
            string className = type.DefaultName ?? "";

            // read method
            var readMethod = GenerateReadMethod(type, xarsuIl2CppStructGeneric, xarsuIl2CppStruct, assemblyName, namespaceName, className);
            type.Methods.Add(readMethod);

            // readto method
            var readToMethod = GenerateReadToMethod(type, xarsuIl2CppStructGeneric, xarsuIl2CppStruct, assemblyName, namespaceName, className);
            type.Methods.Add(readToMethod);

            // write method
            var writeMethod = GenerateWriteMethod(type, xarsuIl2CppStructGeneric, xarsuIl2CppStruct, assemblyName, namespaceName, className);
            type.Methods.Add(writeMethod);
        }
    }

    private InjectedMethodAnalysisContext GenerateSizeGetter(TypeAnalysisContext typeContext, TypeAnalysisContext xarsuIl2CppStructGeneric, TypeAnalysisContext xarsuIl2CppStruct)
    {
        var appContext = typeContext.AppContext;

        var il2cppStructSizeOrig = xarsuIl2CppStruct.GetMethodByName($"get_{nameof(IIl2CppStruct<>.Size)}");
        var il2cppStructSize = new ConcreteGenericMethodAnalysisContext(il2cppStructSizeOrig, [typeContext], []);

        var xarsuIl2CppStaticClass = appContext.ResolveTypeOrThrow(typeof(IL2CPP));
        var il2cppGetIl2CppClass = xarsuIl2CppStaticClass.GetMethodByName(nameof(IL2CPP.GetIl2CppClass));
        var il2cppGetIl2CppNestedType = xarsuIl2CppStaticClass.GetMethodByName(nameof(IL2CPP.GetIl2CppNestedType));
        var il2cppGetValueSize = xarsuIl2CppStaticClass.GetMethodByName(nameof(IL2CPP.il2cpp_class_value_size));

        string assemblyName = MiscUtils.CleanPathElement(typeContext.DeclaringAssembly.DefaultName) + ".dll";
        string namespaceName = typeContext.DefaultNamespace ?? "";
        string className = typeContext.DefaultName ?? "";
        bool isNestedType = typeContext.DeclaringType != null;
        string declaringTypeNamespace = typeContext.DeclaringType?.DefaultNamespace ?? "";
        string declaringClassName = typeContext.DeclaringType?.DefaultName ?? "";

        var method = new InjectedMethodAnalysisContext(
            typeContext,
            $"{xarsuIl2CppStructGeneric.FullName}.get_{nameof(IIl2CppStruct<>.Size)}",
            il2cppStructSize.ReturnType,
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName,
            []);
        method.IsInjected = true;
        method.Overrides.Add(il2cppStructSize);

        LocalVariable variable = new(appContext.SystemTypes.SystemUInt32Type);

        IReadOnlyList<Instruction> instructions = [
            // call GetIl2CppClass to get the class pointer
            new(CilOpCodes.Ldstr, assemblyName),
            new(CilOpCodes.Ldstr, isNestedType ? declaringTypeNamespace : namespaceName),
            new(CilOpCodes.Ldstr, isNestedType ? declaringClassName : className),
            new(CilOpCodes.Call, il2cppGetIl2CppClass),
        ];

        if (isNestedType)
        {
            instructions =
            [
                // call GetIl2CppNestedType to get the nested type pointer
                .. instructions,
                new(CilOpCodes.Ldstr, className),
                new(CilOpCodes.Call, il2cppGetIl2CppNestedType),
            ];
        }

        instructions =
        [
            // call il2cpp_class_value_size to get the size
            .. instructions,
            new(CilOpCodes.Ldloca, variable),
            new(CilOpCodes.Call, il2cppGetValueSize),
            // return the size
            new(CilOpCodes.Ret)
        ];

        method.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions,
            LocalVariables = [
                variable
            ]
        });

        return method;
    }

    private InjectedMethodAnalysisContext GenerateReadToMethod(TypeAnalysisContext typeContext, TypeAnalysisContext xarsuIl2CppStructGeneric, TypeAnalysisContext xarsuIl2CppStruct, string assemblyName, string namespaceName, string className)
    {
        var appContext = typeContext.AppContext;

        var il2cppStructReadOrig = xarsuIl2CppStruct.GetMethodByName(nameof(IIl2CppStruct<>.ReadTo));
        var il2cppStructRead = new ConcreteGenericMethodAnalysisContext(il2cppStructReadOrig, [typeContext], []);

        var xarsuNativeUtilitiesClass = appContext.ResolveTypeOrThrow(typeof(NativeUtilities));
        var readValueAtOffsetMethod = xarsuNativeUtilitiesClass.GetMethodByName(nameof(NativeUtilities.ReadValueAtOffset));

        var method = new InjectedMethodAnalysisContext(
            typeContext,
            $"{xarsuIl2CppStructGeneric.FullName}.{nameof(IIl2CppStruct<>.ReadTo)}",
            appContext.SystemTypes.SystemVoidType,
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName,
            [appContext.SystemTypes.SystemIntPtrType, typeContext.MakeByReferenceType()],
            ["ptr", "instance"],
            [ParameterAttributes.None, ParameterAttributes.None]);
        method.IsInjected = true;
        method.Overrides.Add(il2cppStructRead);

        LocalVariable result = new(typeContext);

        List<Instruction> instructions = [
        ];

        foreach (var field in typeContext.Fields)
        {
            if (field.IsStatic || !field.Visibility.HasFlag(FieldAttributes.Public))
                continue;

            var readValueAtOffsetGeneric = readValueAtOffsetMethod.MakeGenericInstanceMethod(field.FieldType);
            
            instructions.AddRange([
                // instance.field = ReadValueAtOffset(ptr, offset)
                new(CilOpCodes.Ldarg_1),
                new(CilOpCodes.Ldarg_0),              // ptr
                new(CilOpCodes.Ldc_I4, field.Offset), // offset
                new(CilOpCodes.Call, readValueAtOffsetGeneric),
                new(CilOpCodes.Stfld, field),
            ]);
        }

        instructions.AddRange([
            new(CilOpCodes.Ret)
        ]);

        method.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions,
            LocalVariables = [result]
        });

        return method;
    }

    private InjectedMethodAnalysisContext GenerateReadMethod(TypeAnalysisContext typeContext, TypeAnalysisContext xarsuIl2CppStructGeneric, TypeAnalysisContext xarsuIl2CppStruct, string assemblyName, string namespaceName, string className)
    {
        var appContext = typeContext.AppContext;

        var il2cppStructReadOrig = xarsuIl2CppStruct.GetMethodByName(nameof(IIl2CppStruct<>.Read));
        var il2cppStructRead = new ConcreteGenericMethodAnalysisContext(il2cppStructReadOrig, [typeContext], []);

        var xarsuNativeUtilitiesClass = appContext.ResolveTypeOrThrow(typeof(NativeUtilities));
        var readValueAtOffsetMethod = xarsuNativeUtilitiesClass.GetMethodByName(nameof(NativeUtilities.ReadValueAtOffset));

        var method = new InjectedMethodAnalysisContext(
            typeContext,
            $"{xarsuIl2CppStructGeneric.FullName}.{nameof(IIl2CppStruct<>.Read)}",
            typeContext,
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName,
            [appContext.SystemTypes.SystemIntPtrType],
            ["ptr"],
            [ParameterAttributes.None]);
        method.IsInjected = true;
        method.Overrides.Add(il2cppStructRead);

        LocalVariable result = new(typeContext);

        List<Instruction> instructions = [
            // T result = default
            new(CilOpCodes.Ldloca, result),
            new(CilOpCodes.Initobj, typeContext),
        ];

        foreach (var field in typeContext.Fields)
        {
            if (field.IsStatic || !field.Visibility.HasFlag(FieldAttributes.Public))
                continue;

            var readValueAtOffsetGeneric = readValueAtOffsetMethod.MakeGenericInstanceMethod(field.FieldType);
            
            instructions.AddRange([
                // instance.field = ReadValueAtOffset(ptr, offset)
                new(CilOpCodes.Ldloca, result),
                new(CilOpCodes.Ldarg_0),              // ptr
                new(CilOpCodes.Ldc_I4, field.Offset), // offset
                new(CilOpCodes.Call, readValueAtOffsetGeneric),
                new(CilOpCodes.Stfld, field),
            ]);
        }

        instructions.AddRange([
            new(CilOpCodes.Ldloc, result),
            new(CilOpCodes.Ret)
        ]);

        method.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions,
            LocalVariables = [result]
        });

        return method;
    }

    private InjectedMethodAnalysisContext GenerateWriteMethod(TypeAnalysisContext typeContext, TypeAnalysisContext xarsuIl2CppStructGeneric, TypeAnalysisContext xarsuIl2CppStruct, string assemblyName, string namespaceName, string className)
    {
        var appContext = typeContext.AppContext;

        var il2cppStructWriteOrig = xarsuIl2CppStruct.GetMethodByName(nameof(IIl2CppStruct<>.Write));
        var il2cppStructWrite = new ConcreteGenericMethodAnalysisContext(il2cppStructWriteOrig, [typeContext], []);

        var xarsuNativeUtilitiesClass = appContext.ResolveTypeOrThrow(typeof(NativeUtilities));
        var writeValueAtOffsetMethod = xarsuNativeUtilitiesClass.GetMethodByName(nameof(NativeUtilities.WriteValueAtOffset));

        var method = new InjectedMethodAnalysisContext(
            typeContext,
            $"{xarsuIl2CppStructGeneric.FullName}.{nameof(IIl2CppStruct<>.Write)}",
            appContext.SystemTypes.SystemVoidType,
            MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName,
            [typeContext, appContext.SystemTypes.SystemIntPtrType],
            ["instance", "ptr"],
            [ParameterAttributes.None, ParameterAttributes.None]);
        method.IsInjected = true;
        method.Overrides.Add(il2cppStructWrite);

        List<Instruction> instructions = [];

        foreach (var field in typeContext.Fields)
        {
            if (field.IsStatic || !field.Visibility.HasFlag(FieldAttributes.Public))
                continue;

            var writeValueAtOffsetGeneric = writeValueAtOffsetMethod.MakeGenericInstanceMethod(field.FieldType);

            instructions.AddRange([
                // WriteValueAtOffset(ptr, offset, instance.field)
                new(CilOpCodes.Ldarg_1),              // ptr
                new(CilOpCodes.Ldc_I4, field.Offset), // offset
                new(CilOpCodes.Ldarg_0),
                new(CilOpCodes.Ldfld, field),         // value
                new(CilOpCodes.Call, writeValueAtOffsetGeneric)
            ]);
        }

        instructions.Add(new(CilOpCodes.Ret));

        method.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions
        });

        return method;
    }
}