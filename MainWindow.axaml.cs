using Avalonia.Controls;
using DentalCenter.Views;

namespace DentalCenter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainContent.Content = new HomeView();

        btnHome.Click += (_, _) =>
            MainContent.Content = new HomeView();

        btnEquipment.Click += (_, _) =>
            MainContent.Content = new EquipmentView();

        btnPhysical.Click += (_, _) =>
            MainContent.Content = new PhysicalView();

        btnEnergy.Click += (_, _) =>
            MainContent.Content = new EnergyView();

        btnChildren.Click += (_, _) =>
            MainContent.Content = new ChildrenView();

        btnContact.Click += (_, _) =>
            MainContent.Content = new ContactView();

        btnFeedback.Click += (_, _) =>
            MainContent.Content = new FeedbackView();
    }
}