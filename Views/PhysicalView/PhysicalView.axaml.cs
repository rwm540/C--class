using Avalonia.Controls;
using DentalCenter.Data;

namespace DentalCenter.Views;

public partial class PhysicalView : UserControl
{
    public PhysicalView()
    {
        InitializeComponent();
        Detail.Load("فضای فیزیکی", ContentData.Spaces);
    }
}
