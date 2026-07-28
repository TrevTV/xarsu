# xarsu
A NativeAOT-based Unity mod loader for IL2CPP, compatible with Android and Windows.

## Installation (Android)
1. Once compiled, put `libmain.so` and `libc++_shared.so` (from `_redist`) inside the APK's ARM64 library folder (replacing the original `libmain.so`).
2. Place any mods inside that directory as well.
3. Create a `xarsu.toml` configuration file inside the `assets` folder.
4. Inside of the config, you only need to define a list of mods, however you can also add extra information.

```toml
mods = [ # only applicable to APK-embedded mods
    "libfoo.so",
    "libbar.so"
]

# optional:
[logging]
log_level = 1 # 0 = Verbose; 1 = Info; 2 = Warning; 3 = Error
log_to_file = false
```

Mods can also be placed externally at `/sdcard/xarsu/<package name>/`. If two mods share the same name, the one located outside of the APK will take priority.

## Installation (Windows)
1. Once compiled, put `winhttp.dll` next to the application's executable.
2. Place any mods inside a folder named `xarsu` next to the executable.

On launch, a default configuration will be made.

## Proxy Assembly Generation
In order to create mods, you'll need proxy assemblies to reference the game code.

### Option A (Windows-only)
1. Clone the `xarsu` repository.
2. Compile `xarsu.Generator.CLI` in either Release or Debug mode.
3. Run `scripts/generate_reference_libs.ps1` and follow the prompts.

Once the script is finished, a `xarsu_out` folder will be available in the script's working directory.

### Option B
1. Clone the `xarsu` repository.
2. Compile `xarsu.Generator.CLI` in either Release or Debug mode.
3. Run `xarsu.Generator.CLI` with the parameters `--binary` (pointing at `GameAssembly.dll` or `libil2cpp.so`), `--metadata` (pointing at `global-metadata.dat`) and `--data` (pointing at the application's `_Data` folder).

Once the prgram is finished, a `xarsu_out` folder will be available in the script's working directory (unless a different output directory was specified).

```
Available Options for xarsu.Generator.CLI:
  -b, --binary <binary> (REQUIRED)      Path to the Il2Cpp binary file (ex. libil2cpp.so / GameAssembly.dll)
  -m, --metadata <metadata> (REQUIRED)  Path to the Il2Cpp metadata file (global-metadata.dat)
  -c, --corlib <corlib>                 Path to a reference mscorlib.dll
  -d, --data <data> (REQUIRED)          Path to the game's Data directory
  -o, --output <output>                 Path to write the generated reference assemblies [default: <working directory>/xarsu_out]
  -?, -h, --help                        Show help and usage information
  --version                             Show version information
```

## Example Mod

See the [xarsu.ExampleMod](https://github.com/TrevTV/xarsu.ExampleMod) repository for an example project, as well as a rough overview of the API.

## Console (Android)
Logs can be viewed with `adb logcat`. If you want a filtered version, the following works.

`adb logcat -v time main:D xarsu:D Dobby:D Zygote:D DEBUG:D Unity:D Binder:D AndroidRuntime:D *:S`

If enabled, text logs will be written to `/sdcard/xarsu/<package name>/logs`.

## Console (Windows)
xarsu will attach a console to the running application by default.<br>
If enabled, text logs will be written to `<executable directory>/xarsu/logs`.

## Licensing and Credits
- `xarsu.Generator` is based on [ds5678's Il2CppInterop Rewrite](https://github.com/ds5678/Il2CppInterop/tree/v2-rewrite), licensed under the GNU GPL v3.0 License. See [LICENSE](https://github.com/ds5678/Il2CppInterop/blob/v2-rewrite/LICENSE) for the full license.
- [MelonLoader](https://github.com/LavaGang/MelonLoader) is licensed under the Apache-2.0 License. See [LICENSE](https://github.com/LavaGang/MelonLoader/blob/master/LICENSE.md) for the full license.
- [NativeAOT-AndroidHelloJniLib](https://github.com/josephmoresena/NativeAOT-AndroidHelloJniLib) is licensed under the MIT License. See [LICENSE](https://github.com/josephmoresena/NativeAOT-AndroidHelloJniLib/blob/main/LICENSE) for the full license.
- [JNISharp](https://github.com/WarrenUlrich/JNISharp) is licensed under the MIT License. See [LICENSE](https://github.com/WarrenUlrich/JNISharp/blob/master/LICENSE) for the full license.
- [Il2CppInterop](https://github.com/BepInEx/Il2CppInterop) is licensed under the GNU GPL v3.0 License. See [LICENSE](https://github.com/BepInEx/Il2CppInterop/blob/master/LICENSE) for the full license.
- [beatsaber-hook](https://github.com/QuestPackageManager/beatsaber-hook) is licensed under the MIT License. See [LICENSE](https://github.com/QuestPackageManager/beatsaber-hook/blob/master/LICENSE) for the full license.