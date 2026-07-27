namespace xarsu.Proxy.Windows;

internal partial class WindowsLogger : IPlatformLogger
{
    public void Log(string message, ProxyLogger.LogLevel level)
    {
        Console.WriteLine(message);
    }
}