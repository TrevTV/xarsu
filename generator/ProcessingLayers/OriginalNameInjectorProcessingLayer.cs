using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.Model.CustomAttributes;
using xarsu.Generator.Extensions;
using xarsu.Reference;

namespace xarsu.Generator.ProcessingLayers;

public sealed class OriginalNameInjectorProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Original Name Attribute Injector";
    public override string Id => "original_name_injector";

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var typeNameAttribute = appContext.ResolveTypeOrThrow(typeof(OriginalTypeNameAttribute));
        var typeNameAttributeCtor = typeNameAttribute.GetMethodByName(".ctor");
        var nameAttribute = appContext.ResolveTypeOrThrow(typeof(OriginalNameAttribute));
        var nameAttributeCtor = nameAttribute.GetMethodByName(".ctor");

        foreach (var assembly in appContext.Assemblies)
        {
            if (assembly.IsReferenceAssembly || assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected)
                    continue;

                AddAttributeIfOverridden(
                    type,
                    typeNameAttributeCtor,
                    [type.DeclaringAssembly!.OverrideName, type.OverrideNamespace, type.OverrideName],
                    type.DeclaringAssembly!.DefaultName, type.DefaultNamespace, type.DefaultName
                );

                foreach (var field in type.Fields)
                {
                    if (field.IsInjected)
                        continue;
                    AddAttributeIfOverridden(field, nameAttributeCtor, [field.OverrideName], field.DefaultName);
                }

                foreach (var property in type.Properties)
                {
                    if (property.IsInjected)
                        continue;
                    AddAttributeIfOverridden(property, nameAttributeCtor, [property.OverrideName], property.DefaultName);
                }

                foreach (var evnt in type.Events)
                {
                    if (evnt.IsInjected)
                        continue;
                    AddAttributeIfOverridden(evnt, nameAttributeCtor, [evnt.OverrideName], evnt.DefaultName);
                }

                foreach (var method in type.Methods)
                {
                    if (method.IsInjected)
                        continue;
                    AddAttributeIfOverridden(method, nameAttributeCtor, [method.OverrideName], method.DefaultName);
                }
            }
        }
    }

    private static void AddAttributeIfOverridden(HasCustomAttributesAndName item, MethodAnalysisContext ctor, string?[] overrideDatas, params string[] datas)
    {
        // skip if no override data is present, as the attribute would be redundant
        if (!overrideDatas.Any(d => d != null))
            return;

        // skip if all override datas are the same as the originals
        if (overrideDatas.Zip(datas, (o, d) => (o, d)).All(t => t.o == t.d))
            return;

        item.CustomAttributes ??= [];

        var customAttribute = new AnalyzedCustomAttribute(ctor);
        for (int i = 0; i < datas.Length; i++)
        {
            customAttribute.ConstructorParameters.Add(
                new CustomAttributePrimitiveParameter(
                    datas[i],
                    customAttribute,
                    CustomAttributeParameterKind.ConstructorParam,
                    i)
            );
        }

        item.CustomAttributes.Add(customAttribute);
    }
}
