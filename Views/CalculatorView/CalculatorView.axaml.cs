using System;
using Avalonia.Controls;
using DentalCenter.Services;

namespace DentalCenter.Views;

public partial class CalculatorView : UserControl
{
    public CalculatorView()
    {
        InitializeComponent();

        btnCalculate.Click += (_, _) => Recalculate();

        // با هر تغییر ورودی، نتیجه به‌روز شود.
        foreach (var box in new[] { AreaBox, UnitsBox, HoursBox, DaysBox, TariffBox, SolarBox })
            box.ValueChanged += (_, _) => Recalculate();

        foreach (var chk in new[] { ChkLed, ChkSensor, ChkVrf, ChkInsulation, ChkHrv, ChkVsd, ChkSolar })
            chk.IsCheckedChanged += (_, _) => Recalculate();

        Recalculate();
    }

    private void Recalculate()
    {
        var input = new EnergyInput
        {
            AreaM2 = Value(AreaBox, 200),
            Units = (int)Value(UnitsBox, 4),
            HoursPerDay = Value(HoursBox, 10),
            DaysPerYear = (int)Value(DaysBox, 290),
            TariffPerKwh = Value(TariffBox, 1500),
            Led = ChkLed.IsChecked == true,
            Sensors = ChkSensor.IsChecked == true,
            Vrf = ChkVrf.IsChecked == true,
            Insulation = ChkInsulation.IsChecked == true,
            HeatRecovery = ChkHrv.IsChecked == true,
            VsdCompressor = ChkVsd.IsChecked == true,
            Solar = ChkSolar.IsChecked == true,
            SolarKw = Value(SolarBox, 10)
        };

        SolarBox.IsEnabled = input.Solar;

        var result = EnergyCalculator.Calculate(input);

        BaseText.Text = result.BaseKwh.ToString("N0");
        OptimizedText.Text = result.OptimizedKwh.ToString("N0");
        SavingPercentText.Text = result.SavingPercent.ToString("N1");
        SavingMoneyText.Text = result.SavingMoney.ToString("N0");
        BreakdownList.ItemsSource = result.Breakdown;
    }

    private static double Value(NumericUpDown box, double fallback)
        => box.Value.HasValue ? (double)box.Value.Value : fallback;
}
