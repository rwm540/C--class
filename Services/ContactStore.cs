using System;
using System.Collections.Generic;
using DentalCenter.Models;

namespace DentalCenter.Services;

/// <summary>ذخیره و بازیابی پیام‌های فرم تماس در پایگاه دادهٔ SQLite.</summary>
public static class ContactStore
{
    /// <summary>ثبت یک پیام تماس جدید.</summary>
    public static bool Add(ContactMessage message)
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO ContactMessage (Name, Email, Phone, Subject, Message, CreatedAt)
                VALUES ($name, $email, $phone, $subject, $message, $createdAt);
                """;

            command.Parameters.AddWithValue("$name", message.Name);
            command.Parameters.AddWithValue("$email", message.Email ?? "");
            command.Parameters.AddWithValue("$phone", message.Phone ?? "");
            command.Parameters.AddWithValue("$subject", message.Subject ?? "");
            command.Parameters.AddWithValue("$message", message.Message);
            command.Parameters.AddWithValue("$createdAt", Database.ToDbDate(message.CreatedAt));

            command.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>خواندن پیام‌های ثبت‌شده، جدیدترین در ابتدا.</summary>
    public static List<ContactMessage> Load()
    {
        var result = new List<ContactMessage>();

        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT Id, Name, Email, Phone, Subject, Message, CreatedAt
                FROM ContactMessage
                ORDER BY datetime(CreatedAt) DESC, Id DESC;
                """;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ContactMessage
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.GetString(3),
                    Subject = reader.GetString(4),
                    Message = reader.GetString(5),
                    CreatedAt = Database.FromDbDate(reader.GetString(6))
                });
            }
        }
        catch
        {
            // نادیده
        }

        return result;
    }

    /// <summary>تعداد پیام‌های ثبت‌شده.</summary>
    public static int Count()
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM ContactMessage;";
            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>حذف همهٔ پیام‌ها.</summary>
    public static bool Clear()
    {
        try
        {
            using var connection = Database.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ContactMessage;";
            command.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
