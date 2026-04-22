#if ANDROID
using System.Runtime.InteropServices;
using System.Text;
using xarsu.Reference.Java;

namespace xarsu.Proxy.Android;

internal partial class AndroidBootstrap : IProxyBootstrap
{
    public string? PackageName { get; private set; }
    public string Il2CppAssemblyName { get; } = "libil2cpp.so";
    public int ApiLevel { get; private set; } = 0;

    public string? DataDirectory { get; }

    private string? _nativeLibraryDir = null;
    private readonly List<string> _loadedLibraryNames = [];

    private const string CONFIGURATION_FILE_NAME = "xarsu.toml";

    public static void TryInitCore()
    {
        // linux-bionic .NET logs everything to stdout/err, this allows us to see these logs in logcat with our logs
        StdRedirect.RedirectStdOut();
        StdRedirect.RedirectStdErr();

        AndroidBootstrap bootstrap = new();
        Core.Init(bootstrap);
    }

    public AndroidBootstrap()
    {
        CacheApplicationInfo();
        DataDirectory = $"/sdcard/xarsu/{PackageName}/";

        if (Directory.Exists(DataDirectory) && !EnsurePerms())
        {
            Core.ProxyLogger?.LogError("Failed to ensure permissions, aborting loader initialization.");
            return;    
        }
    }

    public bool TryLoadConfiguration()
    {
        // first, try loading it from our data directory
        string fsPath = Path.Combine(DataDirectory!, CONFIGURATION_FILE_NAME);
        if (File.Exists(fsPath))
        {
            Core.ProxyLogger?.Log($"Configuration file found at {fsPath}, attempting to load...");
            try
            {
                string configData = File.ReadAllText(fsPath);
                Configuration.Load(configData);
                Core.ProxyLogger?.Log("Successfully loaded configuration from data directory.");
                return true;
            }
            catch (Exception ex)
            {
                Core.ProxyLogger?.LogError($"Failed to load configuration from data directory: {ex.Message}");
            }
        }

        // if that fails, try loading it from the APK assets
        Stream? assetStream = APKAssetManager.GetAssetStream(CONFIGURATION_FILE_NAME);
        if (assetStream == null)
        {
            Core.ProxyLogger?.LogError("Failed to find configuration file in APK assets.");
            return false;
        }

        Core.ProxyLogger?.Log($"Configuration file found in APK assets, attempting to load...");
        try
        {
            StringBuilder sb = new();

            byte[] buffer = new byte[512];
            int bytesRead;
            while ((bytesRead = assetStream.Read(buffer, 0, buffer.Length)) > 0)
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            Configuration.Load(sb.ToString());
            Core.ProxyLogger?.Log("Successfully loaded configuration from APK assets.");
            return true;
        }
        catch (Exception ex)
        {
            Core.ProxyLogger?.LogError($"Failed to load configuration from APK assets: {ex.Message}");
        }

        return true;
    }

    public bool TryLoadRawLibrary(string path, out IntPtr handle)
    {
        handle = dlopen(path, RTLD_NOW);
        return handle != IntPtr.Zero;
    }

    public IEnumerable<Library> LoadLibraries()
    {
        // load from data directory first
        // we don't check with the configuration here because we are the only ones putting files there
        if (Directory.Exists(DataDirectory!))
        {
            Core.ProxyLogger?.Log($"Data directory found at {DataDirectory!}, attempting to load libraries...");
            string[] libraries = Directory.GetFiles(DataDirectory!, "*.so");
            foreach (string libraryPath in libraries)
            {
                Core.ProxyLogger?.Log($"Found library at {libraryPath}, attempting to load...");

                // android security workaround, can't load from arbitrary paths
                string loadPath = $"/data/data/{PackageName}/files/{Path.GetFileName(libraryPath)}";
                Directory.CreateDirectory($"/data/data/{PackageName}/files/");
                File.Copy(libraryPath, loadPath, true);

                Library? library = HandleLibraryLoad(loadPath);
                if (library != null)
                {
                    _loadedLibraryNames.Add(Path.GetFileNameWithoutExtension(libraryPath));
                    yield return library;
                }
            }
        }

        if (Configuration.Current == null)
        {
            Core.ProxyLogger?.LogError("Configuration not loaded, cannot load APK-embedded libraries.");
            yield break;
        }

        // now check APK-embedded libraries
        foreach (string libraryPath in Configuration.Current!.ModLibraryNames)
        {
            if (_loadedLibraryNames.Contains(libraryPath))
                continue; // already loaded by above (priorized) or not in config

            Library? library = HandleLibraryLoad(libraryPath);
            if (library != null)
                yield return library;
        }

        static Library? HandleLibraryLoad(string libraryPath)
        {
            Core.ProxyLogger?.Log($"Found library at {libraryPath}, attempting to load...");
            bool result = Library.TryLoad(libraryPath, out var library, JNI.JavaVMPtr);
            if (!result)
            {
                string errMsg = Marshal.PtrToStringAnsi(dlerror()) ?? "Unknown error";
                Core.ProxyLogger?.LogError($"Failed to load library {libraryPath}");
                Core.ProxyLogger?.LogError(errMsg);
                return null;
            }
            Core.ProxyLogger?.Log($"Successfully loaded library {library!.Name}");

            return library;
        }
    }

    private void CacheApplicationInfo()
    {
        using JObject currentActivityObj = GetCurrentActivity();
        JClass activityClass = JNI.GetObjectClass(currentActivityObj);

        // package name
        JMethodID getPackageNameMethodId = JNI.GetMethodID(activityClass, "getPackageName", "()Ljava/lang/String;");
        using JString jPackageName = JNI.CallObjectMethod<JString>(currentActivityObj, getPackageNameMethodId);
        PackageName = jPackageName.GetString();

        // native library dir
        JMethodID getApplicationInfoMethodId = JNI.GetMethodID(activityClass, "getApplicationInfo", "()Landroid/content/pm/ApplicationInfo;");
        using JObject applicationInfoObj = JNI.CallObjectMethod<JObject>(currentActivityObj, getApplicationInfoMethodId);
        JClass applicationInfoClass = JNI.GetObjectClass(applicationInfoObj);

        JFieldID nativeLibraryDirFieldId = JNI.GetFieldID(applicationInfoClass, "nativeLibraryDir", "Ljava/lang/String;");
        using JString jNativeLibraryDir = JNI.GetObjectField<JString>(applicationInfoObj, nativeLibraryDirFieldId);
        _nativeLibraryDir = jNativeLibraryDir.GetString();
    }

    private bool EnsurePerms()
    {
        JClass versionClass = JNI.FindClass("android/os/Build$VERSION");
        JFieldID sdkIntField = JNI.GetStaticFieldID(versionClass, "SDK_INT", "I");
        ApiLevel = JNI.GetStaticField<int>(versionClass, sdkIntField);
        Core.ProxyLogger?.Log($"Android API Level: {ApiLevel}");

        using JObject currentActivityObj = GetCurrentActivity();

        if (!CheckManageAllFilesPermission(currentActivityObj))
        {
            Core.ProxyLogger?.Log("Failed to get MANAGE_ALL_FILES permission.");
            return false;
        }
        
        // TODO: causes the app to freeze for an unknown reason but is required for Android 10 and below
        /*if (!EnsurePermsWithUnity(currentActivityObj))
        {
            Core.ProxyLogger?.Log("Failed to ensure permissions with Unity.");
            return false;
        }*/

        return true;
    }

    private bool CheckManageAllFilesPermission(JObject currentActivityObj)
    {
        if (ApiLevel < 30)
            return true; // This part of the API does not exist on Android versions below 11 (API level 30)

        const int MAX_WAIT = 30000; // in milliseconds

        JClass environment = JNI.FindClass("android/os/Environment");
        JClass uri = JNI.FindClass("android/net/Uri");
        JClass intent = JNI.FindClass("android/content/Intent");

        JMethodID isExternalStorageManagerMethodId = JNI.GetStaticMethodID(environment, "isExternalStorageManager", "()Z");
        bool isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
        if (JNI.ExceptionCheck())
            return false;

        if (isExternalStorageManager)
            return true;

        using JString actionName = JNI.NewString("android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");

        using JString packageName = JNI.NewString($"package:{PackageName}");

        using JObject callStaticObjectMethod = JNI.CallStaticObjectMethod<JObject>(uri, JNI.GetStaticMethodID(uri, "parse", "(Ljava/lang/String;)Landroid/net/Uri;"), packageName);

        JMethodID intentConstructor = JNI.GetMethodID(intent, "<init>", "(Ljava/lang/String;Landroid/net/Uri;)V");
        using JObject initialIntent = JNI.NewObject<JObject>(intent, intentConstructor, actionName, callStaticObjectMethod);

        JMethodID addFlagsMethodId = JNI.GetMethodID(intent, "addFlags", "(I)Landroid/content/Intent;");
        int flag = 0x10000000; // FLAG_ACTIVITY_NEW_TASK
        using JObject flaggedIntent = JNI.CallObjectMethod<JObject>(initialIntent, addFlagsMethodId, new JValue(flag));

        JClass activityClass = JNI.GetObjectClass(currentActivityObj);
        JMethodID startActivityMethod = JNI.GetMethodID(activityClass, "startActivity", "(Landroid/content/Intent;)V");
        JNI.CallVoidMethod(currentActivityObj, startActivityMethod, new JValue(flaggedIntent));

        JNI.CheckExceptionAndThrow();

        // TODO: this shouldn't sleep in the main thread; not sure if there is a better method
        int totalWaitTime = 0;
        while (totalWaitTime < MAX_WAIT)
        {
            isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
            if (JNI.ExceptionCheck())
                return false;

            if (isExternalStorageManager)
                return true;

            Thread.Sleep(250);
            totalWaitTime += 250;
        }

        Core.ProxyLogger?.Log("Timed out waiting for MANAGE_ALL_FILES permission, final check...");
        isExternalStorageManager = JNI.CallStaticMethod<bool>(environment, isExternalStorageManagerMethodId);
        if (JNI.ExceptionCheck())
            return false;

        if (isExternalStorageManager)
            return true;

        Core.ProxyLogger?.Log("Failed to get MANAGE_ALL_FILES permission after waiting.");
        return false;
    }

    private bool EnsurePermsWithUnity(JObject currentActivityObj)
    {
        if (ApiLevel >= 30)
            return true; // Not necessary on Android 11+ as you need MANAGE_ALL_FILES_ACCESS_PERMISSION instead.

        string[] permissions =
        [
            "android.permission.WRITE_EXTERNAL_STORAGE",
            "android.permission.MANAGE_EXTERNAL_STORAGE"
        ];

        JClass unityPermissionsClass = JNI.FindClass("com/unity3d/player/UnityPermissions");
        JClass unityWaitPermissionsClass = JNI.FindClass("com/unity3d/player/UnityPermissions$ModalWaitForPermissionResponse");

        using JObject waitPermission = JNI.NewObject<JObject>(unityWaitPermissionsClass, JNI.GetMethodID(unityWaitPermissionsClass, "<init>", "()V"));

        JClass stringClass = JNI.FindClass("java/lang/String");
        using JObjectArray<JString> permissionArray = JNI.NewObjectArray<JString>(permissions.Length, stringClass);
        for (int i = 0; i < permissions.Length; i++)
        {
            permissionArray[i] = JNI.NewString(permissions[i]);
        }

        JMethodID requestUserPermissionsId = JNI.GetStaticMethodID(unityPermissionsClass, "requestUserPermissions", "(Landroid/app/Activity;[Ljava/lang/String;Lcom/unity3d/player/IPermissionRequestCallbacks;)V");
        JNI.CallStaticVoidMethod(unityPermissionsClass, requestUserPermissionsId, new JValue(currentActivityObj), new JValue(permissionArray), new JValue(waitPermission));

        if (JNI.ExceptionCheck())
        {
            Core.ProxyLogger?.Log("Failed to request permissions.");
            return false;
        }

        JMethodID waitForResponseId = JNI.GetMethodID(unityWaitPermissionsClass, "waitForResponse", "()V");
        JNI.CallVoidMethod(waitPermission, waitForResponseId);

        if (JNI.ExceptionCheck())
        {
            Core.ProxyLogger?.Log("Failed to wait for permission response.");
            return false;
        }

        return true;
    }

    private static JObject GetCurrentActivity()
    {
        JClass unityPlayer = JNI.FindClass("com/unity3d/player/UnityPlayer");
        JFieldID activityFieldId = JNI.GetStaticFieldID(unityPlayer, "currentActivity", "Landroid/app/Activity;");
        return JNI.GetStaticObjectField<JObject>(unityPlayer, activityFieldId);
    }

    #region Imports

    const int RTLD_NOW = 2;

    [LibraryImport("libdl.so", StringMarshalling = StringMarshalling.Utf8)]
    protected static partial IntPtr dlopen(string filename, int flags);

    [LibraryImport("libdl.so")]
    protected static partial IntPtr dlerror();

    #endregion
}
#endif