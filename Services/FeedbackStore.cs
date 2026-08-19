using System;
using System.Collections.Generic;
using DentalCenter.Models;
using Microsoft.Data.Sqlite;

namespace DentalCenter.Services;

/// <summary>ذخیره و بازیابی نظرات کاربران در پایگاه دادهٔ SQLite.</summary>
public static class FeedbackStore
{
    /// <summary>خواندن نظرات، جدیدترین در ابتدا. در صورت خطا فهرست خالی برمی‌گرداند.</summary>
    /// <param name="search">جست‌وجوی اختیاری در نام، موضوع و متن نظر.</param>
    /// <param name="minRating">حداقل امتیاز (۰ یعنی بدون فیلتر).</param>
    public static List<Feedback> Load(string? search = null, int minRating = 0)
    {
        var result = new List<Feedback>();

        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();

            var where = " WHERE Rating >= $min";
            if (!string.IsNullOrWhiteSpace(search))
                where += " AND (Name LIKE $q OR Subject LIKE $q OR Message LIKE $q)";

            command.CommandText =
                "SELECT Id, Name, Email, Subject, Rating, Message, CreatedAt FROM Feedback"
                + where
                + " ORDER BY datetime(CreatedAt) DESC, Id DESC;";

            command.Parameters.AddWithValue("$min", minRating);
            if (!string.IsNullOrWhiteSpace(search))
                command.Parameters.AddWithValue("$q", "%" + search.Trim() + "%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Feedback
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Subject = reader.GetString(3),
                    Rating = reader.GetInt32(4),
                    Message = reader.GetString(5),
                    CreatedAt = Database.FromDbDate(reader.GetString(6))
                });
            }
        }
        catch
        {
            // در صورت مشکل دیتابیس، رابط کاربری نباید از کار بیفتد.
        }

        return result;
    }

    /// <summary>افزودن یک نظر جدید. در صورت موفقیت true برمی‌گرداند.</summary>
    public static bool Add(Feedback feedback) => Add(feedback, skipInit: false);

    internal static bool Add(Feedback feedback, bool skipInit)
    {
        try
        {
            if (!skipInit)
                Database.EnsureInitialized();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Feedback (Name, Email, Subject, Rating, Message, CreatedAt)
                VALUES ($name, $email, $subject, $rating, $message, $createdAt);
                """;

            command.Parameters.AddWithValue("$name", feedback.Name);
            command.Parameters.AddWithValue("$email", feedback.Email ?? "");
            command.Parameters.AddWithValue("$subject", feedback.Subject ?? "");
            command.Parameters.AddWithValue("$rating", feedback.Rating);
            command.Parameters.AddWithValue("$message", feedback.Message);
            command.Parameters.AddWithValue("$createdAt", Database.ToDbDate(feedback.CreatedAt));

            command.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>حذف یک نظر بر اساس شناسه.</summary>
    public static bool Delete(long id)
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Feedback WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            return command.ExecuteNonQuery() > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>حذف همهٔ نظرات.</summary>
    public static bool Clear()
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Feedback;";
            command.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>تعداد کل نظرات.</summary>
    public static int Count()
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Feedback;";
            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>تعداد کل نظرات و میانگین امتیاز.</summary>
    public static (int Count, double Average) Summary()
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*), IFNULL(AVG(Rating), 0) FROM Feedback;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
                return (reader.GetInt32(0), reader.GetDouble(1));
        }
        catch
        {
            // نادیده
        }

        return (0, 0);
    }

    /// <summary>تعداد نظرها به تفکیک امتیاز ۱ تا ۵ (اندیس ۰ برای امتیاز ۱).</summary>
    public static int[] RatingHistogram()
    {
        var buckets = new int[5];

        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT Rating, COUNT(*) FROM Feedback GROUP BY Rating;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var rating = reader.GetInt32(0);
                if (rating >= 1 && rating <= 5)
                    buckets[rating - 1] = reader.GetInt32(1);
            }
        }
        catch
        {
            // نادیده
        }

        return buckets;
    }
}
