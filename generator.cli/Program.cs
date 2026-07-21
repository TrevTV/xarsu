using Cpp2IL.Core;
using System.CommandLine;
using System.IO.Compression;
using xarsu.Generator;

RootCommand rootCommand =
[
    new Option<FileInfo>("--binary", "-b") { Required = true, Description = "Path to the Il2Cpp binary file (ex. libil2cpp.so / GameAssembly.dll)" }.AcceptExistingOnly(),
    new Option<FileInfo>("--metadata", "-m") { Required = true, Description = "Path to the Il2Cpp metadata file (global-metadata.dat)" }.AcceptExistingOnly(),
    new Option<FileInfo>("--corlib", "-c") { Required = false, Description = "Path to a reference mscorlib.dll" }.AcceptExistingOnly(),
    new Option<DirectoryInfo>("--data", "-d") { Required = true, Description = "Path to the game's Data directory" }.AcceptExistingOnly(),
    new Option<DirectoryInfo>("--output", "-o") { Required = false, Description = "Path to write the generated reference assemblies", DefaultValueFactory = result => new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "xarsu_out")) },
];

rootCommand.SetAction(async res =>
{
    FileInfo? binaryPath = res.GetValue<FileInfo>("--binary");
    FileInfo? metadataPath = res.GetValue<FileInfo>("--metadata");
    FileInfo? corlibPath = res.GetValue<FileInfo>("--corlib");
    DirectoryInfo? dataPath = res.GetValue<DirectoryInfo>("--data");
    DirectoryInfo? outputPath = res.GetValue<DirectoryInfo>("--output");

    if (binaryPath == null || metadataPath == null || dataPath == null || outputPath == null)
        throw new ArgumentException("Missing required argument.");

    bool cleanUpCorlib = false;

    if (corlibPath == null || !corlibPath.Exists)
    {
        const string CORLIB_URL_TEMPLATE = "https://unity.bepinex.dev/corlibs/{0}.zip";

        // download corlib from bepin if not provided
        var unityVersion = Cpp2IlApi.DetermineUnityVersion(null, dataPath.FullName);
        string corlibUrl = string.Format(CORLIB_URL_TEMPLATE, unityVersion.ToStringWithoutType());
        HttpClient client = new();
        await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, corlibUrl)).ContinueWith(async responseTask =>
        {
            var response = await responseTask;
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to download corlib from {corlibUrl}: {response.StatusCode}");

            using var stream = await response.Content.ReadAsStreamAsync();
            using var archive = new ZipArchive(stream);
            var entry = archive.GetEntry("mscorlib.dll") ?? throw new Exception($"Corlib zip from {corlibUrl} does not contain mscorlib.dll");

            string tempCorlibPath = Path.Combine(Path.GetTempPath(), "mscorlib.dll");
            entry.ExtractToFile(tempCorlibPath, true);
            corlibPath = new FileInfo(tempCorlibPath);
            cleanUpCorlib = true;
        });
    }

    XarsuIl2CppGame.Process(binaryPath.FullName, metadataPath.FullName, dataPath.FullName, outputPath.FullName, [new("corlib", corlibPath!.FullName)]);

    // clean up temporary corlib if we downloaded it
    if (cleanUpCorlib && corlibPath.Exists)
        File.Delete(corlibPath.FullName);
});

ParseResult result = rootCommand.Parse(args);
return result.Invoke();