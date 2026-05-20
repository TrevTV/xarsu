using AsmResolver.PE.DotNet.Cil;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using xarsu.Generator.Extensions;
using xarsu.Generator.Operands;
using xarsu.Reference;

namespace xarsu.Generator.ProcessingLayers;

internal class EnumValueProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Enum Value Processing";
    public override string Id => "enum_value_processing";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected || !type.IsEnumType)
                    continue;

                type.OverrideAttributes = (type.Attributes & ~TypeAttributes.LayoutMask) | TypeAttributes.SequentialLayout;

                var valueField = type.Fields.First(f => f.Name == "value__");

                valueField.OverrideAttributes = FieldAttributes.Private;

                var staticCtor = GenerateStaticCtor(appContext, type);
                type.Methods.Add(staticCtor);

                var underlyingTypeCtor = GenerateUnderlyingTypeCtor(appContext, type);
                type.Methods.Add(underlyingTypeCtor);

                var implicitConversions = GenerateUnderlyingImplicitConversions(appContext, type);
                foreach (var implicitConversion in implicitConversions)
                {
                    type.Methods.Add(implicitConversion);
                }
            }
        }
    }

    private static InjectedMethodAnalysisContext GenerateUnderlyingTypeCtor(ApplicationAnalysisContext appContext, TypeAnalysisContext enumType)
    {
        var underlyingType = enumType.EnumUnderlyingType!;
        var ctor = new InjectedMethodAnalysisContext(
            enumType,
            ".ctor",
            appContext.SystemTypes.SystemVoidType,
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
            [underlyingType]);

        ctor.IsInjected = true;

        List<Instruction> instructions = [
            new Instruction(CilOpCodes.Ldarg_0),
            new Instruction(CilOpCodes.Ldarg_1),
            new Instruction(CilOpCodes.Stfld, enumType.Fields.First(f => f.Name == "value__")),
            new Instruction(CilOpCodes.Ret)
        ];

        ctor.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions
        });

        return ctor;
    }

    private static InjectedMethodAnalysisContext GenerateStaticCtor(ApplicationAnalysisContext appContext, TypeAnalysisContext enumType)
    {
        var underlyingType = enumType.EnumUnderlyingType!;
        var ctor = new InjectedMethodAnalysisContext(
            enumType,
            ".cctor",
            appContext.SystemTypes.SystemVoidType,
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static,
            []);

        ctor.IsInjected = true;

        var valueField = enumType.Fields.First(f => f.Name == "value__");

        List<Instruction> instructions = [];

        LocalVariable enumVar = new(enumType);
        foreach (var val in enumType.Fields.Where(f => f.IsStatic && f.FieldType == enumType))
        {
            // init the enum type
            instructions.Add(new(CilOpCodes.Ldloca_S, enumVar));
            instructions.Add(new(CilOpCodes.Initobj, enumType));

            // set the enum value field
            instructions.Add(new(CilOpCodes.Ldloca_S, enumVar));

            object value = val.ConstantValue!;

            switch (value)
            {
                case sbyte v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, (int)v));
                    break;
                case byte v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, (int)v));
                    break;
                case short v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, (int)v));
                    break;
                case ushort v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, (int)v));
                    break;
                case int v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, v));
                    break;
                case uint v:
                    instructions.Add(new(CilOpCodes.Ldc_I4, unchecked((int)v)));
                    break;
                case long v:
                    instructions.Add(new(CilOpCodes.Ldc_I8, v));
                    break;
                case ulong v:
                    instructions.Add(new(CilOpCodes.Ldc_I8, unchecked((long)v)));
                    break;
                default:
                    throw new NotSupportedException(
                        $"Unsupported enum underlying type {value.GetType().FullName}");
            }

            instructions.Add(new(CilOpCodes.Stfld, valueField));

            // set the static field to the created enum
            instructions.Add(new(CilOpCodes.Ldloc_0));
            instructions.Add(new(CilOpCodes.Stsfld, val));
        }

        instructions.Add(new(CilOpCodes.Ret));

        ctor.PutExtraData(new TranslatedMethodBody()
        {
            Instructions = instructions,
            LocalVariables = [enumVar]
        });

        return ctor;
    }

    private static IEnumerable<InjectedMethodAnalysisContext> GenerateUnderlyingImplicitConversions(ApplicationAnalysisContext appContext, TypeAnalysisContext enumType)
    {
        var underlyingType = enumType.EnumUnderlyingType!;
        // Underlying Type -> Enum
        {
            var implicitConversion = new InjectedMethodAnalysisContext(
                enumType,
                "op_Implicit",
                enumType,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static,
                [underlyingType]);
            implicitConversion.IsInjected = true;

            LocalVariable enumVar = new(enumType);
            implicitConversion.PutExtraData(new TranslatedMethodBody()
            {
                Instructions = [
                    new(CilOpCodes.Ldloca_S, enumVar),
                    new(CilOpCodes.Initobj, enumType),
                    new(CilOpCodes.Ldloca_S, enumVar),
                    new(CilOpCodes.Ldarg_0),
                    new(CilOpCodes.Stfld, enumType.Fields.First(f => f.Name == "value__")),
                    new(CilOpCodes.Ldloca_S, enumVar),
                    new(CilOpCodes.Ret)
                ],
                LocalVariables = [enumVar]
            });

            yield return implicitConversion;
        }

        // Enum -> Underlying Type
        {
            var implicitConversion = new InjectedMethodAnalysisContext(
                enumType,
                "op_Implicit",
                underlyingType,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static,
                [enumType]);
            implicitConversion.IsInjected = true;

            implicitConversion.PutExtraData(new TranslatedMethodBody()
            {
                Instructions = [
                    new(CilOpCodes.Ldarg_0),
                    new(CilOpCodes.Ldfld, enumType.Fields.First(f => f.Name == "value__")),
                    new(CilOpCodes.Ret)
                ]
            });

            yield return implicitConversion;
        }
    }
}