#if ANDROID
using System.Runtime.InteropServices;

namespace xarsu.Proxy.Android;

internal partial class AndroidLogger : IProxyLogger
{
    [LibraryImport("liblog", EntryPoint = "__android_log_print", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int LogNative(LogPriority prio, string tag, string fmt);

    internal static void LogInternal(string msg, LogPriority prio = LogPriority.INFO)
    {
        int res = LogNative(prio, "xarsu", msg);
        if (res < 0)
            throw new Exception("Logging failed: code " + res);
    }

    public void Log(object? message)
    {
        LogInternal(message?.ToString() ?? "", LogPriority.INFO);
    }

    public void LogWarning(object? message)
    {
        LogInternal(message?.ToString() ?? "", LogPriority.WARN);
    }

    public void LogError(object? message)
    {
        LogInternal(message?.ToString() ?? "", LogPriority.ERROR);
    }

    public void LogVerbose(object? message)
    {
        LogInternal(message?.ToString() ?? "", LogPriority.VERBOSE);
    }

    internal enum LogPriority
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