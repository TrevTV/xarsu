namespace xarsu.Proxy;

internal interface IProxyBootstrap
{
    public string? DataDirectory { get; }
    public string Il2CppAssemblyName { get; }

    // i would prefer to use NativeLibrary.Load but it doesn't supply useful error info on failure
    public bool TryLoadRawLibrary(string path, out IntPtr handle);
    public IEnumerable<Library> LoadLibraries();
}