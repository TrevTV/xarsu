using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using xarsu.Generator.Extensions;
using MethodAttributes = System.Reflection.MethodAttributes;
using AsmResolver.PE.DotNet.Cil;

namespace xarsu.Generator.ProcessingLayers;

public class PointerCtorInjectionProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Id => "pointer_ctor_injection";
    public override string Name => "Pointer contructor injection";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var allTypes = appContext.Assemblies
            .Where(a => !a.IsReferenceAssembly && !a.IsInjected)
            .SelectMany(a => a.Types)
            .Where(t => !t.IsInjected && !t.IsValueType)
            .ToList();

        // pass 1: inject the ctor on every type that needs it
        foreach (var type in allTypes)
        {
            if (type.Methods.Any(m => m.IsConstructor && !m.IsStatic
                && m.Parameters.Count == 1
                && m.Parameters[0].ParameterType.FullName == "xarsu.Reference.ObjectPointer"))
                continue;

            var methodContext = new InjectedMethodAnalysisContext(
                type, ".ctor",
                type.AppContext.SystemTypes.SystemObjectType,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                Enumerable.Repeat(type.AppContext.SystemTypes.SystemObjectType, 1).ToArray(),
                ["ptr"],
                [System.Reflection.ParameterAttributes.None],
                System.Reflection.MethodImplAttributes.IL);

            type.Methods.Add(methodContext);
            methodContext.SetDefaultReturnType(type.AppContext.SystemTypes.SystemVoidType);
            var parameter = (InjectedParameterAnalysisContext)methodContext.Parameters[0];
            parameter.SetDefaultParameterType(type.AppContext.ResolveTypeOrThrow(typeof(xarsu.Reference.ObjectPointer)));
            methodContext.IsInjected = true;
        }

        // pass 2: build body data now that all types have the ctor
        foreach (var type in allTypes)
        {
            var ctor = type.Methods.FirstOrDefault(m => m.IsConstructor && !m.IsStatic
                && m.Parameters.Count == 1
                && m.Parameters[0].ParameterType.FullName == "xarsu.Reference.ObjectPointer"
                && m.IsInjected);
            if (ctor == null) continue;

            var baseCtor = type.FindBasePtrCtor();
            if (baseCtor == null)
                continue;

            ctor.PutExtraData(new TranslatedMethodBody()
            {
                Instructions = [
                    new(CilOpCodes.Ldarg_0),
                    new(CilOpCodes.Ldarg_1),
                    new(CilOpCodes.Call, baseCtor),
                    new(CilOpCodes.Ret),
                ]
            });
        }
    }
}
