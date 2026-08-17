using Avalonia.Controls;
using DentalCenter.Data;

namespace DentalCenter.Views;

public partial class EquipmentView : UserControl
{
    public EquipmentView()
    {
        InitializeComponent();
        Detail.Load("تجهیزات مرکز", ContentData.Equipment);
    }
}
