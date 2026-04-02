using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.Model.CustomAttributes;
using xarsu.Generator.Extensions;
using xarsu.Reference;

namespace xarsu.Generator.ProcessingLayers;

public sealed class MethodTokenInjectionProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Method Token Attribute Injection";
    public override string Id => "method_token_injection";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var methodTokenAttribute = appContext.ResolveTypeOrThrow(typeof(MethodTokenAttribute));
        var methodTokenAttributeCtor = methodTokenAttribute.GetMethodByName(".ctor");

        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected)
                    continue;

                foreach (var method in type.Methods)
                {
                    if (method.IsInjected)
                        continue;
                    AddAttribute(method, methodTokenAttributeCtor, method.Token);
                }
            }
        }
    }

    private static void AddAttribute(HasCustomAttributesAndName item, MethodAnalysisContext ctor, uint token)
    {
        item.CustomAttributes ??= [];

        var customAttribute = new AnalyzedCustomAttribute(ctor);
        customAttribute.ConstructorParameters.Add(
            new CustomAttributePrimitiveParameter(
                token,
                customAttribute,
                CustomAttributeParameterKind.ConstructorParam,
                0)
        );

        item.CustomAttributes.Add(customAttribute);
    }
}
