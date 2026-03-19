using Cpp2IL.Core.OutputFormats;
using Cpp2IL.Core.ProcessingLayers;
using System.CommandLine;
using xarsu.Generator;

RootCommand rootCommand = new()
{
    new Option<FileInfo>("--binary", "-b") { Required = true, Description = "Path to the Il2Cpp binary file (ex. libil2cpp.so / GameAssembly.dll)" }.AcceptExistingOnly(),
    new Option<FileInfo>("--metadata", "-m") { Required = true, Description = "Path to the Il2Cpp metadata file (global-metadata.dat)" }.AcceptExistingOnly(),
    new Option<FileInfo>("--corlib", "-c") { Required = true, Description = "Path to a reference mscorlib.dll" }.AcceptExistingOnly(),
    new Option<DirectoryInfo>("--data", "-d") { Required = true, Description = "Path to the game's Data directory" }.AcceptExistingOnly(),
    new Option<DirectoryInfo>("--output", "-o") { Required = true, Description = "Path to write the generated reference assemblies" },
};

rootCommand.SetAction(res =>
{
    FileInfo? binaryPath = res.GetValue<FileInfo>("--binary");
    FileInfo? metadataPath = res.GetValue<FileInfo>("--metadata");
    FileInfo? corlibPath = res.GetValue<FileInfo>("--corlib");
    DirectoryInfo? dataPath = res.GetValue<DirectoryInfo>("--data");
    DirectoryInfo? outputPath = res.GetValue<DirectoryInfo>("--output");

    if (binaryPath == null || metadataPath == null || dataPath == null || outputPath == null || corlibPath == null)
        throw new ArgumentException("Missing required argument.");

    Il2CppGame.Process(binaryPath.FullName,
        metadataPath.FullName,
        dataPath.FullName,
        outputPath.FullName,
        new AsmResolverDllOutputFormatThrowNull(), [
            new AttributeAnalysisProcessingLayer(),
            // TODO: Add more processing layers here as needed
        ],
        [new("corlib", corlibPath.FullName)]);
});

ParseResult result = rootCommand.Parse(args);
return result.Invoke();