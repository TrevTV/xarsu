using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using xarsu.Generator.Extensions;
using xarsu.Generator.Visitors;

namespace xarsu.Generator.ProcessingLayers;

internal class ManagedTypeRemappingProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Managed Type Remapping";

    public override string Id => "managed_type_remapping";

    private readonly Dictionary<string, string> typeMappings = new() {
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
        { "Il2CppSystem.String", "System.String" },
        { "Il2CppSystem.ValueType", "System.ValueType" },
    };

    private TypeReplacementVisitor? _visitor;

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        Dictionary<TypeAnalysisContext, TypeAnalysisContext> remapTypes = [];
        foreach (var remap in typeMappings)
        {
            var monoType = appContext.Mscorlib.GetTypeByFullNameOrThrow(remap.Value);
            remapTypes.Add(appContext.Il2CppMscorlib.GetTypeByFullNameOrThrow(remap.Key), monoType);
        }
        _visitor = new(remapTypes);

        foreach (var assembly in appContext.Assemblies)
        {
            foreach (var type in assembly.Types)
            {
                type.BaseType = _visitor.Replace(type.BaseType);
                _visitor.Modify(type.InterfaceContexts);

                if (type.IsEnumType)
                    type.OverrideEnumUnderlyingType = _visitor.Replace(type.EnumUnderlyingType);

                foreach (var genericParameter in type.GenericParameters)
                {
                    _visitor.Modify(genericParameter.ConstraintTypes);
                }

                foreach (var field in type.Fields)
                {
                    field.FieldType = _visitor.Replace(field.FieldType);
                }

                foreach (var property in type.Properties)
                {
                    property.PropertyType = _visitor.Replace(property.PropertyType);
                }

                foreach (var evnt in type.Events)
                {
                    evnt.EventType = _visitor.Replace(evnt.EventType);
                }

                foreach (var method in type.Methods)
                {
                    method.ReturnType = _visitor.Replace(method.ReturnType);

                    foreach (var param in method.Parameters)
                    {
                        param.ParameterType = _visitor.Replace(param.ParameterType);
                    }

                    foreach (var genericParam in method.GenericParameters)
                    {
                        _visitor.Modify(genericParam.ConstraintTypes);
                    }

                    for (var i = 0; i < method.Overrides.Count; i++)
                    {
                        method.Overrides[i] = _visitor.Replace(method.Overrides[i]);
                    }
                }
            }
        }
    }
}
