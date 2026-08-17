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

    private readonly Button[] _stars = new Button[5];

    private int _rating = 5;

    public FeedbackView()
    {
        InitializeComponent();

        BuildStars();
        FeedbackList.ItemsSource = _items;

        btnSubmit.Click += (_, _) => Submit();
        btnReset.Click += (_, _) => ResetForm();
        btnClear.Click += (_, _) => ClearAll();

        Reload();
    }

    /// <summary>ساخت پنج دکمهٔ ستاره برای امتیازدهی.</summary>
    private void BuildStars()
    {
        for (var i = 0; i < 5; i++)
        {
            var value = i + 1;

            var button = new Button
            {
                Content = "★",
                FontSize = 24,
                Width = 40,
                Height = 40,
                Padding = new Avalonia.Thickness(0),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = Brushes.Transparent,
                BorderThickness = new Avalonia.Thickness(0),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            button.Click += (_, _) => SetRating(value);

            _stars[i] = button;
            StarPanel.Children.Add(button);
        }

        SetRating(_rating);
    }

    private void SetRating(int value)
    {
        _rating = Math.Clamp(value, 1, 5);

        for (var i = 0; i < _stars.Length; i++)
        {
            _stars[i].Foreground = i < _rating
                ? new SolidColorBrush(Color.Parse("#F9A825"))
                : new SolidColorBrush(Color.Parse("#C6CFD6"));
        }
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
            Rating = _rating,
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
        SetRating(5);
    }

    private void Reload()
    {
        _items.Clear();
        foreach (var item in FeedbackStore.Load())
            _items.Add(item);

        var (total, average) = FeedbackStore.Summary();

        CountText.Text = total == 0
            ? "بدون نظر"
            : total + " نظر — میانگین " +
              average.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " از ۵";

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
