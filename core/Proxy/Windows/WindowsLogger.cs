namespace xarsu.Proxy.Windows;

// TODO: Implement a proper logger for Windows that writes to a log file and lets you toggle the log level. For now, just log to the console.
internal partial class WindowsLogger : IProxyLogger
{
    public void Log(object? message)
    {
        Console.WriteLine(message);
    }

    public void LogError(object? message)
    {
        Console.WriteLine(message);
    }

    public void LogVerbose(object? message)
    {
        Console.WriteLine(message);
    }

    public void LogWarning(object? message)
    {
        Console.WriteLine(message);
    }
}