using Avalonia.Controls;
using DentalCenter.Data;

namespace DentalCenter.Views;

public partial class ChildrenView : UserControl
{
    public ChildrenView()
    {
        InitializeComponent();
        Detail.Load("بخش کودکان", ContentData.Children);
    }
}
