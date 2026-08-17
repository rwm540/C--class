using Avalonia.Controls;
using DentalCenter.Helpers;

namespace DentalCenter.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();

        try
        {
            var image = AssetsHelper.LoadImage("clinic.png");
            if (image != null)
                HeroImage.Source = image;
        }
        catch
        {
            // missing image must never prevent the window from opening
        }
    }
}
