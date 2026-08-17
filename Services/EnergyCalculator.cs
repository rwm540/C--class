using System;
using System.Collections.Generic;

namespace DentalCenter.Services;

/// <summary>ورودی‌های محاسبهٔ مصرف انرژی.</summary>
public sealed class EnergyInput
{
    public double AreaM2 { get; set; } = 200;

    public int Units { get; set; } = 4;

    public double HoursPerDay { get; set; } = 10;

    public int DaysPerYear { get; set; } = 290;

    public double TariffPerKwh { get; set; } = 1500;

    public bool Led { get; set; }

    public bool Sensors { get; set; }

    public bool Vrf { get; set; }

    public bool Insulation { get; set; }

    public bool HeatRecovery { get; set; }

    public bool VsdCompressor { get; set; }

    public bool Solar { get; set; }

    public double SolarKw { get; set; } = 10;
}

/// <summary>یک ردیف از تفکیک مصرف.</summary>
public sealed class EnergyLine
{
    public EnergyLine(string label, double kwh, double maxKwh)
    {
        Label = label;
        Kwh = kwh;
        BarWidth = maxKwh > 0 ? Math.Max(6, Math.Round(kwh / maxKwh * 320)) : 6;
    }

    public string Label { get; }

    public double Kwh { get; }

    public double BarWidth { get; }

    public string ValueText => Kwh.ToString("N0") + " kWh";
}

/// <summary>نتیجهٔ محاسبه.</summary>
public sealed class EnergyResult
{
    public double BaseKwh { get; init; }

    public double OptimizedKwh { get; init; }

    public double SolarKwh { get; init; }

    public double SavingKwh => Math.Max(0, BaseKwh - OptimizedKwh);

    public double SavingPercent => BaseKwh > 0 ? SavingKwh / BaseKwh * 100 : 0;

    public double SavingMoney { get; init; }

    public IReadOnlyList<EnergyLine> Breakdown { get; init; } = Array.Empty<EnergyLine>();
}

/// <summary>
/// برآورد تقریبی مصرف سالانهٔ برق یک مرکز دندانپزشکی و اثر راهکارهای بهره‌وری.
/// شاخص‌ها تجربی و برای مقایسهٔ نسبی سناریوها هستند.
/// </summary>
public static class EnergyCalculator
{
    // شاخص‌های پایه بر حسب کیلووات‌ساعت
    private const double LightingWattPerM2 = 12;      // توان روشنایی نصب‌شده بر مترمربع
    private const double HvacKwhPerM2Year = 110;      // سرمایش و گرمایش سالانه بر مترمربع
    private const double VentilationKwhPerM2Year = 25;
    private const double UnitKwhPerHour = 0.9;        // مصرف متوسط هر یونیت در ساعت کار
    private const double CompressorKwhPerHour = 1.2;  // کمپرسور مشترک
    private const double SterilizerKwhPerDay = 4.5;
    private const double OtherKwhPerM2Year = 18;      // پذیرش، رایانه، سرور، متفرقه
    private const double SolarKwhPerKwYear = 1600;

    public static EnergyResult Calculate(EnergyInput input)
    {
        var workHours = input.HoursPerDay * input.DaysPerYear;

        // --- مصرف پایه (بدون هیچ راهکاری) ---
        var lightingBase = LightingWattPerM2 * input.AreaM2 / 1000.0 * workHours;
        var hvacBase = HvacKwhPerM2Year * input.AreaM2;
        var ventBase = VentilationKwhPerM2Year * input.AreaM2;
        var unitsBase = UnitKwhPerHour * input.Units * workHours * 0.45; // ضریب هم‌زمانی
        var compressorBase = CompressorKwhPerHour * workHours * 0.35;
        var sterilizerBase = SterilizerKwhPerDay * input.DaysPerYear;
        var otherBase = OtherKwhPerM2Year * input.AreaM2;

        var baseTotal = lightingBase + hvacBase + ventBase
                        + unitsBase + compressorBase + sterilizerBase + otherBase;

        // --- اعمال راهکارها ---
        var lighting = lightingBase;
        if (input.Led) lighting *= 0.40;        // LED
        if (input.Sensors) lighting *= 0.75;    // سنسور حضور و نور روز

        var hvac = hvacBase;
        if (input.Vrf) hvac *= 0.72;            // VRF / اینورتر
        if (input.Insulation) hvac *= 0.80;     // عایق و پنجرهٔ دوجداره

        var vent = ventBase;
        if (input.HeatRecovery) vent *= 0.60;   // بازیافت حرارت

        var compressor = compressorBase;
        if (input.VsdCompressor) compressor *= 0.68;

        var optimizedBeforeSolar = lighting + hvac + vent
                                   + unitsBase + compressor + sterilizerBase + otherBase;

        var solarKwh = input.Solar
            ? Math.Min(optimizedBeforeSolar, Math.Max(0, input.SolarKw) * SolarKwhPerKwYear)
            : 0;

        var optimized = Math.Max(0, optimizedBeforeSolar - solarKwh);

        var lines = new List<EnergyLine>();
        var max = Math.Max(hvac, Math.Max(lighting, Math.Max(unitsBase, otherBase)));

        lines.Add(new EnergyLine("سرمایش و گرمایش", hvac, max));
        lines.Add(new EnergyLine("روشنایی", lighting, max));
        lines.Add(new EnergyLine("یونیت‌های درمان", unitsBase, max));
        lines.Add(new EnergyLine("تهویه", vent, max));
        lines.Add(new EnergyLine("کمپرسور و ساکشن", compressor, max));
        lines.Add(new EnergyLine("استریلیزاسیون", sterilizerBase, max));
        lines.Add(new EnergyLine("تجهیزات اداری و متفرقه", otherBase, max));

        if (solarKwh > 0)
            lines.Add(new EnergyLine("تولید خورشیدی (کسر می‌شود)", solarKwh, max));

        return new EnergyResult
        {
            BaseKwh = Math.Round(baseTotal),
            OptimizedKwh = Math.Round(optimized),
            SolarKwh = Math.Round(solarKwh),
            SavingMoney = Math.Round((baseTotal - optimized) * input.TariffPerKwh),
            Breakdown = lines
        };
    }
}
