using Cpp2IL.Core.Api;
using Cpp2IL.Core.Logging;
using Cpp2IL.Core.Model.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using xarsu.Generator.Extensions;

namespace xarsu.Generator.ProcessingLayers;

internal class ManagedTypeRemappingProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Managed Type Remapping";

    public override string Id => "managed_type_remapping";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var mscorlib = appContext.Mscorlib;

        Dictionary<string, string> typeMappings = new() {
            { "Il2CppSystem.SByte", "System.SByte"},
            { "Il2CppSystem.Byte", "System.Byte"},
            { "Il2CppSystem.Int16", "System.Int16"},
            { "Il2CppSystem.UInt16", "System.UInt16"},
            { "Il2CppSystem.Int32", "System.Int32"},
            { "Il2CppSystem.UInt32", "System.UInt32"},
            { "Il2CppSystem.Int64", "System.Int64"},
            { "Il2CppSystem.UInt64", "System.UInt64"},
            { "Il2CppSystem.Single", "System.Single"},
            { "Il2CppSystem.Double", "System.Double"},
            { "Il2CppSystem.Char", "System.Char"},
            { "Il2CppSystem.Boolean", "System.Boolean"},
            { "Il2CppSystem.IntPtr", "System.IntPtr"},
            { "Il2CppSystem.UIntPtr", "System.UIntPtr"},
            { "Il2CppSystem.String", "System.String" }
        };

        foreach (var assembly in appContext.Assemblies)
        {
            foreach (var type in assembly.Types)
            {
                foreach (var field in type.Fields)
                {
                    if (typeMappings.TryGetValue(field.FieldType.FullName, out string? monoTypeName) && monoTypeName != null)
                    {
                        var monoType = mscorlib.GetTypeByFullNameOrThrow(monoTypeName);
                        field.FieldType = monoType;
                    }
                }

                foreach (var property in type.Properties)
                {
                    if (typeMappings.TryGetValue(property.PropertyType.FullName, out string? monoTypeName) && monoTypeName != null)
                    {
                        var monoType = mscorlib.GetTypeByFullNameOrThrow(monoTypeName);
                        property.PropertyType = monoType;
                    }
                }

                foreach (var evnt in type.Events)
                {
                    if (typeMappings.TryGetValue(evnt.EventType.FullName, out string? monoTypeName) && monoTypeName != null)
                    {
                        var monoType = mscorlib.GetTypeByFullNameOrThrow(monoTypeName);
                        evnt.EventType = monoType;
                    }
                }

                foreach (var method in type.Methods)
                {
                    if (typeMappings.TryGetValue(method.ReturnType.FullName, out string? monoTypeName) && monoTypeName != null)
                    {
                        var monoType = mscorlib.GetTypeByFullNameOrThrow(monoTypeName);
                        method.ReturnType = monoType;
                    }

                    foreach (var param in method.Parameters)
                    {
                        if (typeMappings.TryGetValue(param.ParameterType.FullName, out monoTypeName) && monoTypeName != null)
                        {
                            var monoType = mscorlib.GetTypeByFullNameOrThrow(monoTypeName);
                            param.ParameterType = monoType;
                        }
                    }
                }
            }
        }
    }
}
