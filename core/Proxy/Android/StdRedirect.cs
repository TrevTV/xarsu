#if ANDROID
using System.Runtime.InteropServices;
using System.Text;

namespace xarsu.Proxy.Android;

internal static partial class StdRedirect
{
    private const int STDERR_FILENO = 2;
    private const int STDOUT_FILENO = 1;

    [LibraryImport("libc.so", SetLastError = true)]
    private static partial int pipe(int[] pipefd);

    [LibraryImport("libc.so", SetLastError = true)]
    private static partial int dup2(int oldfd, int newfd);

    [LibraryImport("libc.so", SetLastError = true)]
    private static partial IntPtr fdopen(int fd, [MarshalAs(UnmanagedType.LPUTF8Str)] string mode);

    [LibraryImport("libc.so", SetLastError = true)]
    private static partial IntPtr fgets(byte[] buffer, int size, IntPtr stream);

    [LibraryImport("liblog.so", SetLastError = true)]
    private static partial int __android_log_write(int prio, [MarshalAs(UnmanagedType.LPUTF8Str)] string tag, [MarshalAs(UnmanagedType.LPUTF8Str)] string text);

    public static void RedirectStdErr()
    {
        Environment.SetEnvironmentVariable("COREHOST_TRACE", "1");
        Environment.SetEnvironmentVariable("COREHOST_TRACE_VERBOSITY", "3");

        RedirectStream(STDERR_FILENO, "xarsu");
    }

    public static void RedirectStdOut()
    {
        Environment.SetEnvironmentVariable("COREHOST_TRACE", "1");
        Environment.SetEnvironmentVariable("COREHOST_TRACE_VERBOSITY", "3");

        RedirectStream(STDOUT_FILENO, "xarsu");
    }

    private static void RedirectStream(int fileno, string tag)
    {
        int[] pipes = new int[2];
        if (pipe(pipes) != 0)
        {
            Console.WriteLine("Failed to create pipe");
            return;
        }

        dup2(pipes[1], fileno);
        IntPtr inputFile = fdopen(pipes[0], "r");

        Thread logThread = new Thread(() =>
        {
            byte[] buffer = new byte[512];
            while (true)
            {
                IntPtr result = fgets(buffer, buffer.Length, inputFile);
                if (result == IntPtr.Zero)
                    break;

                string logMsg = Encoding.UTF8.GetString(buffer).TrimEnd('\0', '\n', '\r');
                __android_log_write(3, tag, logMsg); // debug
            }
        });

        logThread.IsBackground = true;
        logThread.Start();
    }
}
#endif