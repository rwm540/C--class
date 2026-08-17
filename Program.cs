using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;

namespace DentalCenter;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            ReportCrash("UnhandledException", e.ExceptionObject as Exception);

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            ReportCrash("UnobservedTask", e.Exception);
            e.SetObserved();
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            ReportCrash("Main", ex);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    internal static void LogCrash(string source, Exception? ex)
    {
        var text =
            $"Dental Center crashed ({source}){Environment.NewLine}{Environment.NewLine}" +
            (ex?.ToString() ?? "(no exception details)");

        try
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DentalCenter");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "crash.log"), text);
        }
        catch
        {
            // ignore log failures
        }

        try
        {
            var exeDir = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrWhiteSpace(exeDir))
                File.WriteAllText(Path.Combine(exeDir, "DentalCenter-crash.log"), text);
        }
        catch
        {
            // Program Files may be read-only
        }
    }

    internal static void ReportCrash(string source, Exception? ex)
    {
        LogCrash(source, ex);
        ShowNativeError(
            "برنامه نتوانست اجرا شود.\n\n" +
            (ex?.Message ?? "خطای ناشناخته") +
            "\n\nجزئیات در فایل DentalCenter-crash.log ذخیره شد.",
            "Dental Center");
    }

    private static void ShowNativeError(string message, string caption)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                MessageBoxW(IntPtr.Zero, message, caption, 0x00000010);
        }
        catch
        {
            // not Windows / no UI
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);
}
