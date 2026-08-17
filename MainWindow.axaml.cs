using System;
using Avalonia.Controls;
using DentalCenter.Views;

namespace DentalCenter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ShowView(() => new HomeView());

        btnHome.Click += (_, _) => ShowView(() => new HomeView());
        btnEquipment.Click += (_, _) => ShowView(() => new EquipmentView());
        btnPhysical.Click += (_, _) => ShowView(() => new PhysicalView());
        btnEnergy.Click += (_, _) => ShowView(() => new EnergyView());
        btnChildren.Click += (_, _) => ShowView(() => new ChildrenView());
        btnContact.Click += (_, _) => ShowView(() => new ContactView());
        btnFeedback.Click += (_, _) => ShowView(() => new FeedbackView());
    }

    private void ShowView(Func<Control> factory)
    {
        try
        {
            MainContent.Content = factory();
        }
        catch (Exception ex)
        {
            MainContent.Content = new TextBlock
            {
                Text = "این صفحه الان قابل نمایش نیست.\n\n" + ex.Message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(24)
            };
        }
    }
}
