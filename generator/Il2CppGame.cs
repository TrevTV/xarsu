using Cpp2IL.Core;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.InstructionSets;
using Cpp2IL.Core.Logging;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.ProcessingLayers;
using Il2CppInterop.Generator;
using LibCpp2IL;
using xarsu.Generator.ProcessingLayers;

namespace xarsu.Generator;

public static class XarsuIl2CppGame
{
    public static void Process(string binaryPath, string metadataPath, string gameDataPath, string outputFolder, KeyValuePair<string, string>[] extraData)
    {
        Il2CppGame.Process(binaryPath,
            metadataPath,
            gameDataPath,
            outputFolder,
            new XarsuReferenceOutputFormat(), [
                new AttributeAnalysisProcessingLayer(),
                new Il2CppRenamingProcessingLayer(),
                new CleanRenamingProcessingLayer(),
                new ConflictRenamingProcessingLayer(),
                new MscorlibAssemblyInjectionProcessingLayer(),
                new ReferenceAssemblyInjectionProcessingLayer(),
                new KnownTypeAssignmentProcessingLayer(),
                new ReferenceReplacementProcessingLayer(),
                new AttributeRemovalProcessingLayer(),
                new AttributesOverrideProcessingLayer(),
                new PublicizerProcessingLayer(),
                new PointerCtorInjectionProcessingLayer(),
                new PrimitiveImplicitConversionProcessingLayer(),
                new ManagedTypeRemappingProcessingLayer(),
                new ArrayRemappingProcessingLayer(),
                new FieldAccessorProcessingLayer(),
                new StructInterfaceInjectorProcessingLayer(),
                new OriginalNameInjectorProcessingLayer(),
                new MethodTokenInjectionProcessingLayer(),
            ],
            extraData);
    }
}

public static class Il2CppGame
{
    static Il2CppGame()
    {
        Logger.InfoLog += Console.WriteLine;
        Logger.WarningLog += Console.WriteLine;
        Logger.ErrorLog += Console.WriteLine;
        Logger.VerboseLog += Console.WriteLine;

        InstructionSetRegistry.RegisterInstructionSet<X86InstructionSet>(DefaultInstructionSets.X86_32);
        InstructionSetRegistry.RegisterInstructionSet<X86InstructionSet>(DefaultInstructionSets.X86_64);
        InstructionSetRegistry.RegisterInstructionSet<WasmInstructionSet>(DefaultInstructionSets.WASM);
        InstructionSetRegistry.RegisterInstructionSet<ArmV7InstructionSet>(DefaultInstructionSets.ARM_V7);
        InstructionSetRegistry.RegisterInstructionSet<NewArmV8InstructionSet>(DefaultInstructionSets.ARM_V8);

        LibCpp2IlBinaryRegistry.RegisterBuiltInBinarySupport();
    }

    public static void Process(string binaryPath, string metadataPath, string gameDataPath, string outputFolder, Cpp2IlOutputFormat outputFormat, List<Cpp2IlProcessingLayer> processingLayers, KeyValuePair<string, string>[] extraData)
    {
        Process(binaryPath, metadataPath, gameDataPath, processingLayers, extraData);

        outputFormat.DoOutput(GetCurrentAppContext(), outputFolder);
    }

    public static void Process(string binaryPath, string metadataPath, string gameDataPath, List<Cpp2IlProcessingLayer> processingLayers, KeyValuePair<string, string>[] extraData)
    {
        var unityVersion = Cpp2IlApi.DetermineUnityVersion(null, gameDataPath);

        Console.WriteLine($"Detected Unity version {unityVersion}");

        Cpp2IlApi.InitializeLibCpp2Il(binaryPath, metadataPath, unityVersion, false);

        foreach ((var key, var value) in extraData)
        {
            Cpp2IlApi.CurrentAppContext.PutExtraData(key, value);
        }

        foreach (var cpp2IlProcessingLayer in processingLayers)
        {
            cpp2IlProcessingLayer.PreProcess(GetCurrentAppContext(), processingLayers);
        }

        foreach (var cpp2IlProcessingLayer in processingLayers)
        {
            cpp2IlProcessingLayer.Process(GetCurrentAppContext());
        }
    }

    private static ApplicationAnalysisContext GetCurrentAppContext()
    {
        return Cpp2IlApi.CurrentAppContext ?? throw new NullReferenceException();
    }
}