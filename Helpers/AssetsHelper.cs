using System;
using System.Diagnostics;
using System.IO;

namespace DentalCenter.Helpers;

public static class AssetsHelper
{
    // مسیر ریشه پروژه
    public static string RootPath
    {
        get
        {
            string path = AppContext.BaseDirectory;

            while (!File.Exists(Path.Combine(path, "DentalCenter.csproj")))
            {
                DirectoryInfo? parent = Directory.GetParent(path);

                if (parent == null)
                    break;

                path = parent.FullName;
            }

            return path;
        }
    }

    public static string Image(string relative)
    {
        return Path.Combine(RootPath, "Assets", "Images", relative);
    }

    public static string Pdf(string relative)
    {
        return Path.Combine(RootPath, "Assets", "PDF", relative);
    }

    public static void OpenPdf(string relative)
    {
        string pdf = Pdf(relative);

        if (!File.Exists(pdf))
            return;

        Process.Start(new ProcessStartInfo
        {
            FileName = pdf,
            UseShellExecute = true
        });
    }
}