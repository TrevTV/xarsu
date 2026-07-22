using AsmResolver.DotNet;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.Logging;
using Cpp2IL.Core.Model.Contexts;
using xarsu.Generator.Extensions;

namespace xarsu.Generator.ProcessingLayers;

public class MscorlibAssemblyInjectionProcessingLayer : Cpp2IlProcessingLayer
{
    public override string Name => "Inject a new mscorlib into the Cpp2IL context system";

    public override string Id => "mscorlib_injector";

    private const string MscorlibKey = "corlib";

    private static readonly string[] injectedAssemblies =
    [
        "mscorlib",
        "System.Collections",
    ];
    internal static ReadOnlySpan<string> InjectedAssemblies => injectedAssemblies;

    public override void Process(ApplicationAnalysisContext appContext, Action<int, int>? progressCallback = null)
    {
        var corlibPath = appContext.GetExtraData<string>(MscorlibKey);
        var mscorlib = corlibPath != null ? AssemblyDefinition.FromFile(corlibPath) : null;

        if (mscorlib is null)
        {
            Logger.WarnNewline("mscorlib not provided, processor will not run.", nameof(MscorlibAssemblyInjectionProcessingLayer));
            return;
        }

        if (appContext.AssembliesByName.ContainsKey("mscorlib"))
        {
            Logger.WarnNewline("mscorlib already injected, processor will not run.", nameof(MscorlibAssemblyInjectionProcessingLayer));
            return;
        }

        Logger.InfoNewline($"Injecting new mscorlib...", nameof(MscorlibAssemblyInjectionProcessingLayer));

        appContext.InjectAssemblies([mscorlib]);

        // Need to reset the system types context to use the new corlib
        appContext.SystemTypes = new SystemTypesContext(appContext);

        appContext.AssembliesByName["mscorlib"].IsReferenceAssembly = true;
    }
}
