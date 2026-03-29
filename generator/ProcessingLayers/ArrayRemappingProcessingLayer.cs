using AsmResolver.PE.DotNet.Metadata.Tables;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Logging;
using Cpp2IL.Core.Model.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using xarsu.Generator.Extensions;

namespace xarsu.Generator.ProcessingLayers;

internal class ArrayRemappingProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Array Remapping";

    public override string Id => "array_remapping";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var mscorlib = appContext.Mscorlib;

        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected)
                    continue;

                foreach (var field in type.Fields)
                {
                    if (field.IsInjected)
                        continue;

                    if (TryReplaceArrayType(appContext, field.FieldType, out var newType))
                        field.OverrideFieldType = newType;
                }

                foreach (var property in type.Properties)
                {
                    if (property.IsInjected)
                        continue;

                    if (TryReplaceArrayType(appContext, property.PropertyType, out var newType))
                        property.OverridePropertyType = newType;
                }

                foreach (var evnt in type.Events)
                {
                    if (evnt.IsInjected)
                        continue;

                    if (TryReplaceArrayType(appContext, evnt.EventType, out var newType))
                        evnt.OverrideEventType = newType;
                }

                foreach (var method in type.Methods)
                {
                    if (method.IsInjected)
                        continue;

                    if (TryReplaceArrayType(appContext, method.ReturnType, out var newType))
                        method.OverrideReturnType = newType;

                    foreach (var param in method.Parameters)
                    {
                        if (param.IsInjected)
                            continue;

                        if (TryReplaceArrayType(appContext, param.ParameterType, out var newParamType))
                            param.OverrideParameterType = newParamType;
                    }
                }
            }
        }
    }

    private bool TryReplaceArrayType(ApplicationAnalysisContext appContext, TypeAnalysisContext type, out TypeAnalysisContext? newType)
    {
        var il2ArrayType = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.Il2CppArray<>));
        var il2ValueArrayType = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.Il2CppValueArray<>));

        if (type is ArrayTypeAnalysisContext arrayType)
        {
            // TODO: non-single dimension arrays
        }

        if (type is SzArrayTypeAnalysisContext szArrayType)
        {
            var realType = szArrayType.ElementType;
            if (realType.IsValueType)
            {
                var genericArrayType = il2ValueArrayType.MakeGenericInstanceType(realType);
                newType = genericArrayType;
            }
            else
            {
                var genericArrayType = il2ArrayType.MakeGenericInstanceType(realType);
                newType = genericArrayType;
            }

            return true;
        }

        newType = null;
        return false;
    }
}
