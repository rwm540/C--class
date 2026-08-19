using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using DentalCenter.Models;
using DentalCenter.Services;

namespace DentalCenter.Views;

public partial class FeedbackView : UserControl
{
    private readonly ObservableCollection<Feedback> _items = new();

    public FeedbackView()
    {
        InitializeComponent();

        FeedbackList.ItemsSource = _items;

        btnSubmit.Click += (_, _) => Submit();
        btnReset.Click += (_, _) => ResetForm();
        btnClear.Click += (_, _) => ClearAll();

        Reload();
    }

    private void Submit()
    {
        var name = (NameBox.Text ?? "").Trim();
        var message = (MessageBox.Text ?? "").Trim();

        if (name.Length == 0)
        {
            ShowStatus("لطفاً نام خود را وارد کنید.", false);
            NameBox.Focus();
            return;
        }

        if (message.Length < 3)
        {
            ShowStatus("متن نظر خیلی کوتاه است.", false);
            MessageBox.Focus();
            return;
        }

        var email = (EmailBox.Text ?? "").Trim();
        if (email.Length > 0 && (!email.Contains('@') || !email.Contains('.')))
        {
            ShowStatus("قالب ایمیل درست نیست.", false);
            EmailBox.Focus();
            return;
        }

        var feedback = new Feedback
        {
            Name = name,
            Email = email,
            Subject = (SubjectBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "نظر کلی",
            Rating = 5,
            Message = message,
            CreatedAt = DateTime.Now
        };

        if (FeedbackStore.Add(feedback))
        {
            ResetForm();
            Reload();
            ShowStatus("نظر شما با موفقیت ثبت شد. ✔", true);
        }
        else
        {
            ShowStatus("ذخیرهٔ نظر ممکن نشد؛ دسترسی نوشتن روی دیسک را بررسی کنید.", false);
        }
    }

    private void ClearAll()
    {
        if (_items.Count == 0)
            return;

        if (FeedbackStore.Clear())
        {
            Reload();
            ShowStatus("همهٔ نظرات حذف شد.", true);
        }
    }

    private void ResetForm()
    {
        NameBox.Text = "";
        EmailBox.Text = "";
        MessageBox.Text = "";
        SubjectBox.SelectedIndex = 0;
    }

    private void Reload()
    {
        _items.Clear();
        foreach (var item in FeedbackStore.Load())
            _items.Add(item);

        var total = FeedbackStore.Count();

        CountText.Text = total == 0
            ? "بدون نظر"
            : total + " نظر ثبت شده";

        EmptyText.IsVisible = _items.Count == 0;
    }

    private void ShowStatus(string text, bool success)
    {
        StatusText.Text = text;
        StatusText.IsVisible = true;
        StatusText.Foreground = new SolidColorBrush(
            Color.Parse(success ? "#2E7D32" : "#C62828"));
    }
}
