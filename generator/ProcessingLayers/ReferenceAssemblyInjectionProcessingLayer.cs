using System.Reflection;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using xarsu.Generator.Extensions;

namespace xarsu.Generator.ProcessingLayers;

public class ReferenceAssemblyInjectionProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Id => "reference_assembly_injector";
    public override string Name => "Inject required references into the Cpp2IL context system";
    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        Type[] xarsuTypes =
        [
            typeof(xarsu.Reference.IL2CPP),
            typeof(xarsu.Reference.Il2CppObject),
            typeof(xarsu.Reference.ObjectPointer),
            typeof(xarsu.Reference.OriginalNameAttribute),
            typeof(xarsu.Reference.OriginalTypeNameAttribute),
        ];
        InjectTypes(appContext, typeof(xarsu.Reference.IL2CPP).Assembly, xarsuTypes);
    }

    /// <summary>
    /// Injects the given assembly and some of its types into the <see cref="ApplicationAnalysisContext"/>.
    /// </summary>
    /// <param name="appContext">The <see cref="ApplicationAnalysisContext"/></param>
    /// <param name="assembly">The assembly</param>
    /// <param name="types">The types to be injected from <paramref name="assembly"/>. Must be in order of inheritance</param>
    private static void InjectTypes(ApplicationAnalysisContext appContext, Assembly assembly, Type[] types)
    {
        var il2CppInteropRuntime = appContext.InjectAssembly(assembly);

        il2CppInteropRuntime.IsReferenceAssembly = true;

        var typeContextArray = new InjectedTypeAnalysisContext[types.Length];

        for (var i = 0; i < types.Length; i++)
        {
            typeContextArray[i] = il2CppInteropRuntime.InjectType(types[i]);
        }

        for (var index = 0; index < types.Length; index++)
        {
            typeContextArray[index].InjectContentFromSourceType();
        }
    }
}
