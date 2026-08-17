using Avalonia.Controls;
using DentalCenter.Data;

namespace DentalCenter.Views;

public partial class EnergyView : UserControl
{
    public EnergyView()
    {
        InitializeComponent();
        Detail.Load("بهره‌وری انرژی", ContentData.Energy);
    }
}
