using System.Collections.Generic;

namespace DentalCenter.Data;

/// <summary>یک ردیف اطلاعات تماس.</summary>
public sealed class ContactInfo
{
    public ContactInfo(string role, string icon, string name, string email, string phone)
    {
        Role = role;
        Icon = icon;
        Name = name;
        Email = email;
        Phone = phone;
    }

    public string Role { get; }

    public string Icon { get; }

    public string Name { get; }

    public string Email { get; }

    public string Phone { get; }
}

public static class ContactData
{
    public static IReadOnlyList<ContactInfo> All { get; } = new[]
    {
        new ContactInfo(
            role: "استاد راهنما",
            icon: "🎓",
            name: ContentData.SupervisorName,
            email: "supervisor@example.com",
            phone: "۰۲۱-۰۰۰۰۰۰۰۰"),

        new ContactInfo(
            role: "دانشجو",
            icon: "👨‍🎓",
            name: ContentData.StudentName,
            email: "student@example.com",
            phone: "۰۹۱۲-۰۰۰۰۰۰۰")
    };
}
