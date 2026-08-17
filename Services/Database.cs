using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace DentalCenter.Services;

/// <summary>
/// مدیریت پایگاه دادهٔ SQLite برنامه.
/// فایل دیتابیس در %LOCALAPPDATA%\DentalCenter\dentalcenter.db ساخته می‌شود.
/// </summary>
public static class Database
{
    private static bool _initialized;

    private static readonly object Gate = new();

    /// <summary>مسیر کامل فایل دیتابیس.</summary>
    public static string FilePath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DentalCenter");

            return Path.Combine(dir, "dentalcenter.db");
        }
    }

    /// <summary>رشتهٔ اتصال به دیتابیس.</summary>
    public static string ConnectionString =>
        new SqliteConnectionStringBuilder
        {
            DataSource = FilePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();

    /// <summary>
    /// باز کردن یک اتصال آماده‌به‌کار. در اولین فراخوانی، جدول‌ها ساخته می‌شوند.
    /// </summary>
    public static SqliteConnection Open()
    {
        EnsureInitialized();

        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    /// <summary>ساخت پوشه و جدول‌ها در صورت نبود.</summary>
    public static void EnsureInitialized()
    {
        if (_initialized)
            return;

        lock (Gate)
        {
            if (_initialized)
                return;

            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = """
                PRAGMA journal_mode = WAL;

                CREATE TABLE IF NOT EXISTS Feedback (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name      TEXT    NOT NULL,
                    Email     TEXT    NOT NULL DEFAULT '',
                    Subject   TEXT    NOT NULL DEFAULT '',
                    Rating    INTEGER NOT NULL DEFAULT 5,
                    Message   TEXT    NOT NULL,
                    CreatedAt TEXT    NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_Feedback_CreatedAt
                    ON Feedback (CreatedAt DESC);

                CREATE TABLE IF NOT EXISTS ContactMessage (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name      TEXT    NOT NULL,
                    Email     TEXT    NOT NULL DEFAULT '',
                    Phone     TEXT    NOT NULL DEFAULT '',
                    Subject   TEXT    NOT NULL DEFAULT '',
                    Message   TEXT    NOT NULL,
                    CreatedAt TEXT    NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_ContactMessage_CreatedAt
                    ON ContactMessage (CreatedAt DESC);
                """;
            command.ExecuteNonQuery();

            _initialized = true;
        }

        // انتقال یک‌بارهٔ نظرات از فایل JSON نسخه‌های قبلی به دیتابیس.
        JsonMigration.RunIfNeeded();
    }

    /// <summary>قالب ذخیرهٔ تاریخ در دیتابیس (قابل مرتب‌سازی به‌صورت متنی).</summary>
    public const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    public static string ToDbDate(DateTime value) =>
        value.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture);

    public static DateTime FromDbDate(string? value)
    {
        if (DateTime.TryParseExact(
                value,
                DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var parsed))
        {
            return parsed;
        }

        return DateTime.TryParse(value, out var loose) ? loose : DateTime.Now;
    }
}
