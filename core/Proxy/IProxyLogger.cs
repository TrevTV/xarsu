namespace xarsu.Proxy;

internal interface IPlatformLogger
{
    void Log(string message, ProxyLogger.LogLevel level);
}