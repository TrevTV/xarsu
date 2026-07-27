#if WINDOWS
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using xarsu.Proxy.Windows.Exports;

namespace xarsu.Proxy.Windows;

// based on https://github.com/NotNite/NativeProxy and slxdy's improvements
internal static partial class WindowsProxy
{
    private static readonly Proxy[] _proxies =
    [
        new() {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "version.dll"),
            ProxyFuncs = typeof(VersionExports)
        },
        new() {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winhttp.dll"),
            ProxyFuncs = typeof(WinHttpExports)
        },
        new() {
            OriginalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winmm.dll"),
            ProxyFuncs = typeof(WinMMExports)
        },
    ];

    [UnmanagedCallersOnly(EntryPoint = "DllProcessAttach")]
    internal static void DllMain(nint hModule)
    {
        InstallProxyRedirects(hModule);

        WindowsBootstrap.TryInitCore();
    }

    private static unsafe void InstallProxyRedirects(nint ourHandle)
    {
        var ourPathBdr = new StringBuilder(1024);
        if (GetModuleFileName(ourHandle, ourPathBdr, (uint)ourPathBdr.Capacity) == 0)
            return;

        var ourName = Path.GetFileName(ourPathBdr.ToString());

        var proxy = _proxies.FirstOrDefault(x => ourName.Equals(Path.GetFileName(x.OriginalPath), StringComparison.OrdinalIgnoreCase));
        if (proxy == null)
            return;

        if (!NativeLibrary.TryLoad(proxy.OriginalPath, out nint ogHandle))
            return;

        foreach (var exportMethod in proxy.ProxyFuncs.GetMethods())
        {
            var export = exportMethod.Name;
            var preTag = "Impl";
            if (!export.StartsWith(preTag))
                continue;

            export = export[preTag.Length..];

            if (!NativeLibrary.TryGetExport(ogHandle, export, out var theirExport)
                || !NativeLibrary.TryGetExport(ourHandle, export, out var ourExport))
            {
                Console.WriteLine($"Proxy export not found: '{export}'");
                continue;
            }

            var jump = AssembleJump(theirExport);
            if (VirtualProtect(ourExport, jump.Length, 0x40 /* PAGE_EXECUTE_READWRITE */, out var oldProtect) != 1)
                continue;

            var span = new Span<byte>((byte*)ourExport, jump.Length);
            jump.CopyTo(span);
            VirtualProtect(ourExport, jump.Length, oldProtect, out _);
        }
    }

    private static unsafe byte[] AssembleJump(nint addr)
    {
        byte[] shellcode;
        int offset;
        if (sizeof(nint) == 4)
        {
            shellcode = [
                // mov eax,
                0xB8,
                // addr
                0x00, 0x00, 0x00, 0x00,
                // jmp eax
                0xFF, 0xE0
            ];
            offset = 1;
        }
        else
        {
            shellcode = [
                // mov r11,
                0x49, 0xBB,
                // addr
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                // jmp r11
                0x41, 0xFF, 0xE3
            ];
            offset = 2;
        }

        var addrBytes = BitConverter.GetBytes(addr);
        Array.Copy(addrBytes, 0, shellcode, offset, sizeof(nint));

        return shellcode;
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetModuleFileName(nint module, StringBuilder filename, uint size);

    [LibraryImport("kernel32", SetLastError = true)]
    private static partial byte VirtualProtect(nint address, nint size, uint newProtect, out uint oldProtect);

    private class Proxy
    {
        public required string OriginalPath { get; init; }
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        public required Type ProxyFuncs { get; init; }
    }
}
#endif