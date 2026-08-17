using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DentalCenter.Models;

namespace DentalCenter.Services;

/// <summary>
/// نسخه‌های قبلی برنامه نظرات را در feedback.json ذخیره می‌کردند.
/// این کلاس یک‌بار آن داده‌ها را به دیتابیس SQLite منتقل می‌کند تا نظر کسی از دست نرود.
/// </summary>
internal static class JsonMigration
{
    private static bool _done;

    public static void RunIfNeeded()
    {
        if (_done)
            return;

        _done = true;

        try
        {
            var jsonPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DentalCenter",
                "feedback.json");

            if (!File.Exists(jsonPath))
                return;

            var json = File.ReadAllText(jsonPath);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var items = JsonSerializer.Deserialize<List<Feedback>>(json);
            if (items == null || items.Count == 0)
            {
                ArchiveFile(jsonPath);
                return;
            }

            foreach (var item in items)
                FeedbackStore.Add(item, skipInit: true);

            // فایل قدیمی را کنار می‌گذاریم تا دفعهٔ بعد دوباره وارد نشود.
            ArchiveFile(jsonPath);
        }
        catch
        {
            // اگر انتقال ممکن نشد، برنامه باید عادی کار کند.
        }
    }

    private static void ArchiveFile(string path)
    {
        try
        {
            var target = path + ".migrated";
            if (File.Exists(target))
                File.Delete(target);

            File.Move(path, target);
        }
        catch
        {
            // نادیده می‌گیریم
        }
    }
}
