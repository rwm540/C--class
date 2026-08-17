using System.Collections.Generic;

namespace DentalCenter.Models;

/// <summary>
/// یک موضوع قابل نمایش در صفحات جزئیات (تجهیزات، فضای فیزیکی، بهره‌وری انرژی).
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
        IReadOnlyList<Spec> specs,
        string energyNote,
        string? pdf = null)
    {
        Id = id;
        Icon = icon;
        Title = title;
        Summary = summary;
        Image = image;
        Bullets = bullets;
        Specs = specs;
        EnergyNote = energyNote;
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

    /// <summary>مشخصات فنی به‌صورت کلید/مقدار.</summary>
    public IReadOnlyList<Spec> Specs { get; }

    /// <summary>نکتهٔ مربوط به بهره‌وری انرژی.</summary>
    public string EnergyNote { get; }

    /// <summary>نام فایل PDF نسبت به Assets/PDF.</summary>
    public string Pdf { get; }

    public string ButtonText => Icon + "  " + Title;
}

/// <summary>یک ردیف از جدول مشخصات فنی.</summary>
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
