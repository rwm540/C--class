using System.Collections.Generic;

namespace DentalCenter.Models;

/// <summary>
/// یک موضوع قابل نمایش در صفحات جزئیات (تجهیزات، فضای فیزیکی، بهره‌وری انرژی).
/// جزئیات فنی (مشخصات فنی، واژگان کلیدی و نکتهٔ بهره‌وری انرژی) در نرم‌افزار نمایش
/// داده نمی‌شوند و فقط داخل فایل PDF همان موضوع آمده‌اند.
/// </summary>
public sealed class Topic
{
    public Topic(
        string id,
        string icon,
        string title,
        string summary,
        string image,
        IReadOnlyList<string> bullets,
        string? pdf = null)
    {
        Id = id;
        Icon = icon;
        Title = title;
        Summary = summary;
        Image = image;
        Bullets = bullets;
        Pdf = pdf ?? id + ".pdf";
    }

    public string Id { get; }

    public string Icon { get; }

    public string Title { get; }

    /// <summary>خلاصهٔ یک تا دو خطی موضوع.</summary>
    public string Summary { get; }

    /// <summary>مسیر تصویر نسبت به Assets/Images.</summary>
    public string Image { get; }

    /// <summary>نکات کلیدی طراحی.</summary>
    public IReadOnlyList<string> Bullets { get; }

    /// <summary>نام فایل PDF نسبت به Assets/PDF.</summary>
    public string Pdf { get; }

    public string ButtonText => Icon + "  " + Title;
}

/// <summary>یک جفت کلید/مقدار (برای آمار صفحهٔ نخست).</summary>
public sealed class Spec
{
    public Spec(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }

    public string Value { get; }
}
