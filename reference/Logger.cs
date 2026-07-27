namespace xarsu.Reference;

public class Logger(string tag)
{
    private readonly string _tag = tag;

    public void Log(object? message) => XarsuExports.Log($"[{_tag}] {message?.ToString() ?? "<null>"}");
    public void LogWarning(object? message) => XarsuExports.LogWarning($"[{_tag}] {message?.ToString() ?? "<null>"}");
    public void LogError(object? message) => XarsuExports.LogError($"[{_tag}] {message?.ToString() ?? "<null>"}");
    public void LogVerbose(object? message) => XarsuExports.LogVerbose($"[{_tag}] {message?.ToString() ?? "<null>"}");
}