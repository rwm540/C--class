using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using DentalCenter.Models;

namespace DentalCenter.Services;

/// <summary>
/// ذخیره و بازیابی نظرات کاربران در یک فایل JSON کنار دادهٔ برنامه.
/// مسیر: %LOCALAPPDATA%\DentalCenter\feedback.json
/// </summary>
public static class FeedbackStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string FilePath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DentalCenter");

            return Path.Combine(dir, "feedback.json");
        }
    }

    /// <summary>خواندن همهٔ نظرات؛ در صورت خطا فهرست خالی برمی‌گرداند.</summary>
    public static List<Feedback> Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new List<Feedback>();

            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Feedback>();

            return JsonSerializer.Deserialize<List<Feedback>>(json, Options)
                   ?? new List<Feedback>();
        }
        catch
        {
            return new List<Feedback>();
        }
    }

    /// <summary>افزودن یک نظر جدید. در صورت موفقیت true برمی‌گرداند.</summary>
    public static bool Add(Feedback feedback)
    {
        var all = Load();
        all.Insert(0, feedback);
        return Save(all);
    }

    /// <summary>حذف همهٔ نظرات.</summary>
    public static bool Clear() => Save(new List<Feedback>());

    private static bool Save(List<Feedback> items)
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(FilePath, JsonSerializer.Serialize(items, Options));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
