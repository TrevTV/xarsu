using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.OutputFormats;
using xarsu.Generator.Extensions;

namespace xarsu.Generator;

internal class XarsuReferenceOutputFormat : AsmResolverDllOutputFormatThrowNull
{
    public override string OutputFormatId => "xarsureference";

    public override string OutputFormatName => "Xarsu Reference DLLs";

    protected override void FillMethodBody(MethodDefinition methodDefinition, MethodAnalysisContext methodContext)
    {
        if (methodContext.TryGetExtraData(out TranslatedMethodBody? translatedBody))
        {
            translatedBody.FillMethodBody(methodDefinition);
            methodContext.RemoveExtraData<TranslatedMethodBody>(); // Free up memory
        }
        else
        {
            if (!FillMethodBodyCore(methodDefinition, methodContext))
                base.FillMethodBody(methodDefinition, methodContext); // if filling fails/is ignored, throw null instead
        }
    }

    private static bool FillMethodBodyCore(MethodDefinition methodDef, MethodAnalysisContext methodCtx)
    {
        if (methodDef.IsAbstract || methodDef.IsPInvokeImpl)
            return true; // no body needed for abstract or P/Invoke methods

        var module = methodDef.DeclaringModule!;
        methodDef.CilMethodBody = new CilMethodBody();
        var il = methodDef.CilMethodBody.Instructions;

        if (methodCtx is ConcreteGenericMethodAnalysisContext || methodDef.GenericParameters.Count > 0)
        {
            EmitNotSupported(il, module,
                $"Generic method {methodDef.DeclaringType?.FullName}.{methodDef.Name} is not supported.");
            return true;
        }

        return false;
    }

    private static void EmitNotSupported(CilInstructionCollection il, ModuleDefinition module, string message)
    {
        var notSupportedCtor = module.CorLibTypeFactory.CorLibScope
            .CreateTypeReference("System", "NotSupportedException")
            .CreateMemberReference(".ctor",
                MethodSignature.CreateInstance(module.CorLibTypeFactory.Void,
                    module.CorLibTypeFactory.String))
            .ImportWith(module.DefaultImporter);

        il.Add(new CilInstruction(CilOpCodes.Ldstr, message));
        il.Add(new CilInstruction(CilOpCodes.Newobj, notSupportedCtor));
        il.Add(new CilInstruction(CilOpCodes.Throw));
    }

    public override List<AssemblyDefinition> BuildAssemblies(ApplicationAnalysisContext context)
    {
        var list = base.BuildAssemblies(context);

        var referenceAssemblies = context.Assemblies.Where(a => a.IsReferenceAssembly).Select(a => a.Name).ToHashSet();

        // Remove injected reference assemblies from the output
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (referenceAssemblies.Contains(list[i].Name ?? ""))
                list.RemoveAt(i);
        }

        // Replace mscorlib references with .NET Core references
        var dotNetCorLib = KnownCorLibs.SystemRuntime_v10_0_0_0;
        foreach (var assembly in list)
        {
            foreach (var module in assembly.Modules)
            {
                foreach (var reference in module.AssemblyReferences)
                {
                    if (reference.Name == "mscorlib")
                    {
                        reference.Name = dotNetCorLib.Name;
                        reference.Version = dotNetCorLib.Version;
                        reference.Attributes = dotNetCorLib.Attributes;
                        reference.PublicKeyOrToken = dotNetCorLib.PublicKeyOrToken;
                        reference.HashValue = dotNetCorLib.HashValue;
                        reference.Culture = dotNetCorLib.Culture;
                    }
                }
            }
        }

        return list;
    }
}