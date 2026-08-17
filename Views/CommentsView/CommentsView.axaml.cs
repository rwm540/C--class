using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DentalCenter.Models;
using DentalCenter.Services;

namespace DentalCenter.Views;

/// <summary>یک ردیف از نمودار توزیع امتیازها.</summary>
public sealed class RatingBar
{
    public RatingBar(int stars, int count, int max)
    {
        Label = new string('★', stars);
        Count = count;
        BarWidth = max > 0 ? Math.Max(4, Math.Round((double)count / max * 260)) : 4;
    }

    public string Label { get; }

    public int Count { get; }

    public double BarWidth { get; }

    public string CountText => Count.ToString(CultureInfo.InvariantCulture);
}

public partial class CommentsView : UserControl
{
    private readonly ObservableCollection<Feedback> _items = new();
    private readonly ObservableCollection<ContactMessage> _messages = new();

    public CommentsView()
    {
        InitializeComponent();

        CommentList.ItemsSource = _items;

        btnRefresh.Click += (_, _) => Reload();
        btnClear.Click += (_, _) => ClearAll();
        SearchBox.TextChanged += (_, _) => Reload();
        FilterBox.SelectionChanged += (_, _) => Reload();

        MessageList.ItemsSource = _messages;
        btnRefreshMessages.Click += (_, _) => ReloadMessages();
        btnClearMessages.Click += (_, _) => ClearMessages();

        Reload();
        ReloadMessages();
    }

    /// <summary>بارگذاری پیام‌های فرم «تماس با ما» از پایگاه داده.</summary>
    private void ReloadMessages()
    {
        _messages.Clear();
        foreach (var message in ContactStore.Load())
            _messages.Add(message);

        MessageCountText.Text = _messages.Count == 0
            ? "پیام‌های تماس"
            : _messages.Count + " پیام دریافت شده";

        EmptyMessagesPanel.IsVisible = _messages.Count == 0;
        btnClearMessages.IsEnabled = _messages.Count > 0;
    }

    private void ClearMessages()
    {
        if (_messages.Count > 0 && ContactStore.Clear())
            ReloadMessages();
    }

    private void Reload()
    {
        var search = SearchBox.Text ?? "";

        var minRating = FilterBox.SelectedIndex switch
        {
            1 => 5,
            2 => 4,
            3 => 3,
            _ => 0
        };

        _items.Clear();
        foreach (var item in FeedbackStore.Load(search, minRating))
            _items.Add(item);

        var (total, average) = FeedbackStore.Summary();

        AverageText.Text = total > 0
            ? average.ToString("0.0", CultureInfo.InvariantCulture)
            : "—";

        var rounded = (int)Math.Round(average);
        AverageStars.Text = total > 0
            ? new string('★', Math.Clamp(rounded, 0, 5)) + new string('☆', 5 - Math.Clamp(rounded, 0, 5))
            : "☆☆☆☆☆";

        TotalText.Text = total switch
        {
            0 => "بدون نظر",
            _ => "از " + total + " نظر"
        };

        BuildHistogram();

        var hasAny = _items.Count > 0;
        EmptyPanel.IsVisible = !hasAny;
        EmptyText.Text = total == 0
            ? "هنوز نظری ثبت نشده است.\nاز صفحهٔ «ثبت نظرات» اولین نظر را وارد کنید."
            : "نظری با این جست‌وجو یا فیلتر پیدا نشد.";

        btnClear.IsEnabled = total > 0;
    }

    private void BuildHistogram()
    {
        var buckets = FeedbackStore.RatingHistogram();

        var max = 0;
        foreach (var value in buckets)
        {
            if (value > max)
                max = value;
        }

        var rows = new List<RatingBar>();
        for (var stars = 5; stars >= 1; stars--)
            rows.Add(new RatingBar(stars, buckets[stars - 1], max));

        HistogramList.ItemsSource = rows;
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: long id } && FeedbackStore.Delete(id))
            Reload();
    }

    private void ClearAll()
    {
        if (FeedbackStore.Clear())
            Reload();
    }
}
