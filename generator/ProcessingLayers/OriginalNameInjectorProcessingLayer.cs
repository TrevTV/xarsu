using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.Model.CustomAttributes;
using System.Runtime.InteropServices;
using xarsu.Generator.Extensions;
using xarsu.Reference;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (assembly.IsInjected)
                continue;

            foreach (var type in assembly.Types)
            {
                if (type.IsInjected)
                    continue;

                AddAttribute(type, typeNameAttributeCtor, type.DeclaringAssembly!.DefaultName, type.DefaultNamespace, type.DefaultName);

                foreach (var field in type.Fields)
                {
                    if (field.IsInjected)
                        continue;
                    AddAttribute(field, nameAttributeCtor, field.DefaultName);
                }

                foreach (var property in type.Properties)
                {
                    if (property.IsInjected)
                        continue;
                    AddAttribute(property, nameAttributeCtor, property.DefaultName);
                }

                foreach (var evnt in type.Events)
                {
                    if (evnt.IsInjected)
                        continue;
                    AddAttribute(evnt, nameAttributeCtor, evnt.DefaultName);
                }

                foreach (var method in type.Methods)
                {
                    if (method.IsInjected)
                        continue;
                    AddAttribute(method, nameAttributeCtor, method.DefaultName);
                }
            }
        }
    }

    private static void AddAttribute(HasCustomAttributesAndName item, MethodAnalysisContext ctor, params string[] datas)
    {
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
