using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DentalCenter.Data;

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

            CopyStatus.Text = "کپی شد: " + value;
            CopyStatus.IsVisible = true;
        }
        catch (Exception)
        {
            CopyStatus.Text = "کپی کردن در این سیستم ممکن نیست.";
            CopyStatus.IsVisible = true;
        }
    }
}
