using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using xarsu.Generator.Extensions;
using xarsu.Generator.Visitors;

namespace xarsu.Generator;

public class ReferenceReplacementProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Id => "reference_replacement";
    public override string Name => "Reference Replacement";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var il2CppMscorlib = appContext.AssembliesByName["Il2Cppmscorlib"];
        var xarsuReference = appContext.AssembliesByName["xarsu.Reference"];
        var mscorlib = appContext.AssembliesByName["mscorlib"];

        var monoSystemObject = mscorlib.GetTypeByFullNameOrThrow("System.Object");
        var monoSystemValueType = mscorlib.GetTypeByFullNameOrThrow("System.ValueType");
        var monoSystemVoid = mscorlib.GetTypeByFullNameOrThrow("System.Void");

        var il2CppSystemObject = il2CppMscorlib.GetTypeByFullNameOrThrow("Il2CppSystem.Object");
        var il2CppSystemVoid = il2CppMscorlib.GetTypeByFullNameOrThrow("Il2CppSystem.Void");
        var il2CppSystemEnum = il2CppMscorlib.GetTypeByFullNameOrThrow("Il2CppSystem.Enum");
        var il2CppSystemValueType = il2CppMscorlib.GetTypeByFullNameOrThrow("Il2CppSystem.ValueType");

        var xarsuIl2CppObject = xarsuReference.GetTypeByFullNameOrThrow("xarsu.Reference.Il2CppObject");

        il2CppSystemObject.OverrideBaseType = monoSystemObject;

        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInterface)
                {
                    type.OverrideBaseType = null;
                }
                else if (type.IsStatic)
                {
                    type.OverrideBaseType = monoSystemObject;
                }
                else if (type == il2CppSystemObject)
                {
                    type.OverrideBaseType = xarsuIl2CppObject;
                }
                else if (type == il2CppSystemEnum || type == il2CppSystemValueType)
                {
                }
                else if (type.BaseType is null || type.BaseType == il2CppSystemObject)
                {
                    type.OverrideBaseType = il2CppSystemObject;
                }
                else if (type.BaseType == il2CppSystemValueType)
                {
                    type.OverrideBaseType = monoSystemValueType;
                }
                else if (type.BaseType == il2CppSystemEnum)
                {
                    type.OverrideBaseType = monoSystemValueType;
                }

                foreach (var method in type.Methods)
                {
                    if (method.ReturnType == il2CppSystemVoid)
                    {
                        // Special case for void return type.
                        method.OverrideReturnType = monoSystemVoid;
                    }
                }
            }
        }
    }
}
