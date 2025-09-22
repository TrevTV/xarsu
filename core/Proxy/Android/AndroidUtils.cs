#if ANDROID
using System.Runtime.InteropServices;

namespace xarsu.Proxy.Android;

internal static partial class AndroidUtils
{
    [LibraryImport("liblog", EntryPoint = "__android_log_print", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int LogInternal(LogPriority prio, string tag, string fmt);

    public static void Log(string msg, LogPriority prio = LogPriority.INFO)
    {
        int res = LogInternal(prio, "xarsu", msg);
        if (res < 0)
            throw new Exception("Logging failed: code " + res);
    }

    public static void LogError(string msg)
    {
        Log(msg, LogPriority.ERROR);
    }

    public enum LogPriority
    {
        UNKNOWN = 0,
        DEFAULT = 1,
        VERBOSE = 2,
        DEBUG = 3,
        INFO = 4,
        WARN = 5,
        ERROR = 6,
        FATAL = 7,
        SILENT = 8,
    }
}
#endif