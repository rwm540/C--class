using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DentalCenter.Models;
using DentalCenter.Services;

namespace DentalCenter.Views;

public partial class CommentsView : UserControl
{
    private readonly ObservableCollection<Feedback> _items = new();

    public CommentsView()
    {
        InitializeComponent();

        CommentList.ItemsSource = _items;

        btnRefresh.Click += (_, _) => Reload();
        btnClear.Click += (_, _) => ClearAll();
        SearchBox.TextChanged += (_, _) => Reload();

        Reload();
    }

    private void Reload()
    {
        var search = SearchBox.Text ?? "";

        _items.Clear();
        foreach (var item in FeedbackStore.Load(search, 0))
            _items.Add(item);

        var total = FeedbackStore.Count();

        var hasAny = _items.Count > 0;
        EmptyPanel.IsVisible = !hasAny;
        EmptyText.Text = total == 0
            ? "هنوز نظری ثبت نشده است.\nاز صفحهٔ «ثبت نظرات» اولین نظر را وارد کنید."
            : "نظری با این جست‌وجو پیدا نشد.";

        btnClear.IsEnabled = total > 0;
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