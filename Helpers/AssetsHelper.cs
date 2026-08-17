using System;
using System.IO;
using System.Diagnostics;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace DentalCenter.Helpers;

public static class AssetsHelper
{
    private const string AssemblyName = "DentalCenter";

    public static string AppDirectory
    {
        get
        {
            var fromProcess = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrWhiteSpace(fromProcess) && Directory.Exists(fromProcess))
                return fromProcess;

            return AppContext.BaseDirectory;
        }
    }

    public static string RootPath
    {
        get
        {
            var path = AppDirectory;

            for (var i = 0; i < 8; i++)
            {
                if (File.Exists(Path.Combine(path, "DentalCenter.csproj")))
                    return path;

                var parent = Directory.GetParent(path);
                if (parent == null)
                    break;

                path = parent.FullName;
            }

            return AppDirectory;
        }
    }

    public static string Image(string relative)
        => FirstExisting(
               Path.Combine(AppDirectory, "Assets", "Images", relative),
               Path.Combine(RootPath, "Assets", "Images", relative))
           ?? Path.Combine(AppDirectory, "Assets", "Images", relative);

    public static string Pdf(string relative)
        => FirstExisting(
               Path.Combine(AppDirectory, "Assets", "PDF", relative),
               Path.Combine(RootPath, "Assets", "PDF", relative))
           ?? Path.Combine(AppDirectory, "Assets", "PDF", relative);

    public static Bitmap? LoadImage(string relative)
    {
        relative = relative.Replace('\\', '/').TrimStart('/');

        try
        {
            var uri = new Uri($"avares://{AssemblyName}/Assets/Images/{relative}");
            if (AssetLoader.Exists(uri))
                return new Bitmap(AssetLoader.Open(uri));
        }
        catch
        {
            // fall through to files on disk
        }

        foreach (var candidate in new[]
                 {
                     Path.Combine(AppDirectory, "Assets", "Images", relative),
                     Path.Combine(AppContext.BaseDirectory, "Assets", "Images", relative),
                     Path.Combine(RootPath, "Assets", "Images", relative),
                 })
        {
            try
            {
                if (File.Exists(candidate))
                    return new Bitmap(candidate);
            }
            catch
            {
                // try next location
            }
        }

        return null;
    }

    /// <summary>آیا فایل PDF موردنظر روی دیسک یا داخل اسمبلی موجود است؟</summary>
    public static bool PdfExists(string relative)
    {
        if (string.IsNullOrWhiteSpace(relative))
            return false;

        relative = relative.Replace('\\', '/').TrimStart('/');

        var onDisk = FirstExisting(
            Path.Combine(AppDirectory, "Assets", "PDF", relative),
            Path.Combine(AppContext.BaseDirectory, "Assets", "PDF", relative),
            Path.Combine(RootPath, "Assets", "PDF", relative));

        if (onDisk != null)
            return true;

        try
        {
            return AssetLoader.Exists(new Uri($"avares://{AssemblyName}/Assets/PDF/{relative}"));
        }
        catch
        {
            return false;
        }
    }

    public static void OpenPdf(string relative)
    {
        if (string.IsNullOrWhiteSpace(relative))
            return;

        relative = relative.Replace('\\', '/').TrimStart('/');

        var onDisk = FirstExisting(
            Path.Combine(AppDirectory, "Assets", "PDF", relative),
            Path.Combine(AppContext.BaseDirectory, "Assets", "PDF", relative),
            Path.Combine(RootPath, "Assets", "PDF", relative));

        if (onDisk == null)
            onDisk = ExtractEmbeddedPdf(relative);

        if (onDisk == null || !File.Exists(onDisk))
            return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = onDisk,
                UseShellExecute = true
            });
        }
        catch
        {
            // no PDF viewer / not Windows
        }
    }

    private static string? ExtractEmbeddedPdf(string relative)
    {
        try
        {
            var uri = new Uri($"avares://{AssemblyName}/Assets/PDF/{relative}");
            if (!AssetLoader.Exists(uri))
                return null;

            var dest = Path.Combine(
                Path.GetTempPath(),
                "DentalCenter",
                "PDF",
                relative.Replace('/', Path.DirectorySeparatorChar));

            var destDir = Path.GetDirectoryName(dest);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            using var src = AssetLoader.Open(uri);
            using var dst = File.Create(dest);
            src.CopyTo(dst);
            return dest;
        }
        catch
        {
            return null;
        }
    }

    private static string? FirstExisting(params string[] paths)
    {
        foreach (var path in paths)
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }
}
