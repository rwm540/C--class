using System;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using DentalCenter.Data;
using DentalCenter.Models;
using DentalCenter.Services;

namespace DentalCenter.Views;

public partial class ContactView : UserControl
{
    public ContactView()
    {
        InitializeComponent();

        ContactList.ItemsSource = ContactData.All;

        TitleText.Text = ContentData.ProjectTitle;
        UniversityText.Text = ContentData.University;
        YearText.Text = ContentData.AcademicYear;

        btnSend.Click += (_, _) => Send();
        btnResetForm.Click += (_, _) => ResetForm();

        UpdateCount();
    }

    private void Send()
    {
        var name = (NameBox.Text ?? "").Trim();
        var email = (EmailBox.Text ?? "").Trim();
        var message = (MessageBox.Text ?? "").Trim();

        if (name.Length == 0)
        {
            ShowStatus("لطفاً نام خود را وارد کنید.", false);
            NameBox.Focus();
            return;
        }

        if (email.Length == 0 || !email.Contains('@') || !email.Contains('.'))
        {
            ShowStatus("یک ایمیل معتبر وارد کنید.", false);
            EmailBox.Focus();
            return;
        }

        if (message.Length < 5)
        {
            ShowStatus("متن پیام خیلی کوتاه است.", false);
            MessageBox.Focus();
            return;
        }

        var contact = new ContactMessage
        {
            Name = name,
            Email = email,
            Phone = (PhoneBox.Text ?? "").Trim(),
            Subject = (SubjectBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "پرسش عمومی",
            Message = message,
            CreatedAt = DateTime.Now
        };

        if (ContactStore.Add(contact))
        {
            ResetForm();
            UpdateCount();
            ShowStatus("پیام شما ثبت شد. ✔", true);
        }
        else
        {
            ShowStatus("ثبت پیام ممکن نشد؛ دسترسی نوشتن روی دیسک را بررسی کنید.", false);
        }
    }

    private void ResetForm()
    {
        NameBox.Text = "";
        EmailBox.Text = "";
        PhoneBox.Text = "";
        MessageBox.Text = "";
        SubjectBox.SelectedIndex = 0;
    }

    private void UpdateCount()
    {
        var count = ContactStore.Count();
        SentCountText.Text = count == 0
            ? "هنوز پیامی ثبت نشده است."
            : count + " پیام در پایگاه داده ذخیره شده است.";
    }

    private void ShowStatus(string text, bool success)
    {
        StatusText.Text = text;
        StatusText.IsVisible = true;
        StatusText.Foreground = new SolidColorBrush(
            Color.Parse(success ? "#2E7D32" : "#C62828"));
    }

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string value } || string.IsNullOrWhiteSpace(value))
            return;

        try
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard == null)
                return;

            await clipboard.SetTextAsync(value);
            ShowStatus("کپی شد: " + value, true);
        }
        catch (Exception)
        {
            ShowStatus("کپی کردن در این سیستم ممکن نیست.", false);
        }
    }
}
