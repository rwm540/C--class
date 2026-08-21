using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DentalCenter.Helpers;
using DentalCenter.Models;

namespace DentalCenter.Controls;

/// <summary>
/// کنترل مشترکِ نمایش جزئیات؛ صفحات تجهیزات، فضای فیزیکی، انرژی و کودکان
/// همگی از همین کنترل با داده‌های متفاوت استفاده می‌کنند.
/// </summary>
public partial class DetailPageView : UserControl
{
    private Topic? _current;

    public DetailPageView()
    {
        InitializeComponent();
        btnPdf.Click += OnPdfClick;
    }

    /// <summary>تنظیم عنوان بخش و فهرست موضوع‌ها.</summary>
    public void Load(string sectionTitle, IReadOnlyList<Topic> topics)
    {
        SectionTitle.Text = sectionTitle;
        TopicList.ItemsSource = topics;
        TopicList.SelectedIndex = topics.Count > 0 ? 0 : -1;
    }

    private void OnTopicChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TopicList.SelectedItem is Topic topic)
            Show(topic);
    }

    private void Show(Topic topic)
    {
        _current = topic;

        TopicTitle.Text = topic.Icon + "  " + topic.Title;
        TopicSummary.Text = topic.Summary;

        var bitmap = AssetsHelper.LoadImage(topic.Image);
        TopicImage.Source = bitmap;
        TopicImage.IsVisible = bitmap != null;
        ImageFallback.IsVisible = bitmap == null;

        BulletsList.ItemsSource = topic.Bullets;
        BulletsSection.IsVisible = topic.Bullets.Count > 0;

        // مشخصات فنی، واژگان کلیدی و نکتهٔ بهره‌وری انرژی در نرم‌افزار نمایش داده
        // نمی‌شوند؛ این موارد فقط داخل فایل PDF مربوط به همین موضوع آمده‌اند.
        var hasPdf = AssetsHelper.PdfExists(topic.Pdf);
        btnPdf.IsEnabled = hasPdf;
        PdfHint.Text = hasPdf
            ? ""
            : "فایل " + topic.Pdf + " در پوشهٔ Assets/PDF قرار داده نشده است.";
    }

    private void OnPdfClick(object? sender, RoutedEventArgs e)
    {
        if (_current != null)
            AssetsHelper.OpenPdf(_current.Pdf);
    }
}
