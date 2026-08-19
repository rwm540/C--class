using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DentalCenter.Models;

/// <summary>یک نظر ثبت‌شده توسط کاربر.</summary>
public sealed class Feedback
{
    /// <summary>شناسهٔ ردیف در دیتابیس (۰ یعنی هنوز ذخیره نشده).</summary>
    [JsonIgnore]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = "";

    [JsonPropertyName("rating")]
    public int Rating { get; set; } = 5;

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string CreatedAtText
    {
        get
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(CreatedAt):0000}/{pc.GetMonth(CreatedAt):00}/{pc.GetDayOfMonth(CreatedAt):00}  {pc.GetHour(CreatedAt):00}:{pc.GetMinute(CreatedAt):00}";
        }
    }

    [JsonIgnore]
    public string Header => string.IsNullOrWhiteSpace(Subject)
        ? Name
        : Name + " — " + Subject;

    /// <summary>حرف اول نام برای نمایش در آواتار دایره‌ای.</summary>
    [JsonIgnore]
    public string Initial => string.IsNullOrWhiteSpace(Name)
        ? "؟"
        : Name.Trim().Substring(0, 1);
}
