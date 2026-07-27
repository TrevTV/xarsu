#if ANDROID
using System.Runtime.InteropServices;

namespace xarsu.Proxy.Android;

internal partial class AndroidLogger : IPlatformLogger
{
    [LibraryImport("liblog", EntryPoint = "__android_log_print", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int LogNative(LogPriority prio, string tag, string fmt);

    internal static void LogInternal(string msg, LogPriority prio = LogPriority.INFO)
    {
        int res = LogNative(prio, "xarsu", msg);
        if (res < 0)
            throw new Exception("Logging failed: code " + res);
    }

    public void Log(string message, ProxyLogger.LogLevel level)
    {
        switch (level)
        {
            case ProxyLogger.LogLevel.Verbose:
                LogInternal(message, LogPriority.VERBOSE);
                break;
            case ProxyLogger.LogLevel.Info:
                LogInternal(message, LogPriority.INFO);
                break;
            case ProxyLogger.LogLevel.Warning:
                LogInternal(message, LogPriority.WARN);
                break;
            case ProxyLogger.LogLevel.Error:
                LogInternal(message, LogPriority.ERROR);
                break;
            default:
                LogInternal(message, LogPriority.UNKNOWN);
                break;
        }
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