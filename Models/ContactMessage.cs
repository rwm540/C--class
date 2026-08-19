using System;
using System.Globalization;

namespace DentalCenter.Models;

/// <summary>یک پیام ارسال‌شده از فرم تماس با ما.</summary>
public sealed class ContactMessage
{
    public long Id { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Subject { get; set; } = "";

    public string Message { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string CreatedAtText => CreatedAt.ToString("yyyy/MM/dd  HH:mm");

    public string Header => string.IsNullOrWhiteSpace(Subject)
        ? Name
        : Name + " — " + Subject;

    public string Initial => string.IsNullOrWhiteSpace(Name)
        ? "؟"
        : Name.Trim().Substring(0, 1);
}
