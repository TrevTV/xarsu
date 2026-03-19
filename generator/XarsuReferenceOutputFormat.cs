using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Native;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.OutputFormats;

namespace xarsu.Generator;

internal class XarsuReferenceOutputFormat : AsmResolverDllOutputFormatThrowNull
{
    public override string OutputFormatId => "xarsureference";

    public override string OutputFormatName => "Xarsu Reference DLLs";

    protected override void FillMethodBody(MethodDefinition methodDefinition, MethodAnalysisContext methodContext)
    {
        base.FillMethodBody(methodDefinition, methodContext);

        // TODO: method body handling
    }
}