namespace xarsu.Proxy;

internal class ProxyLogger
{
    private readonly string _logFilePath;
    private readonly StreamWriter? _logWriter;
    private readonly IPlatformLogger _platformLogger;

    public ProxyLogger(IPlatformLogger platformLogger, string dataDirectory)
    {
        _platformLogger = platformLogger;

        _logFilePath = Path.Combine(dataDirectory, "logs", $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss.fff}.log");

        if (Configuration.Current!.Logging?.LogToFile ?? false)
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);

        _logWriter = (Configuration.Current!.Logging?.LogToFile ?? false) ? File.CreateText(_logFilePath) : null;
    }

    private void LogInternal(object? message, LogLevel level)
    {
        if ((int)level < (int)(Configuration.Current!.Logging?.LogLevel ?? LogLevel.Info))
            return;

        string logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
        _platformLogger.Log(logMessage, level);
        _logWriter?.WriteLine(logMessage);
        _logWriter?.Flush();
    }

    public void Log(object? message) => LogInternal(message?.ToString() ?? "<null>", LogLevel.Info);

    public void LogWarning(object? message) => LogInternal(message?.ToString() ?? "<null>", LogLevel.Warning);

    public void LogError(object? message) => LogInternal(message?.ToString() ?? "<null>", LogLevel.Error);

    public void LogVerbose(object? message) => LogInternal(message?.ToString() ?? "<null>", LogLevel.Verbose);

    public enum LogLevel
    {
        Verbose,
        Info,
        Warning,
        Error
    }
}