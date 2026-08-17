using Avalonia.Controls;
using DentalCenter.Helpers;

namespace DentalCenter.Views;

public partial class EquipmentView : UserControl
{
    private string currentPdf = "";

    public EquipmentView()
    {
        InitializeComponent();

        btnUnit.Click += (_, _) =>
            Show(
                "یونیت دندانپزشکی",
                "یونیت دندانپزشکی اصلی‌ترین تجهیز هر کلینیک است.",
                "Equipment/unit.png",
                "Equipment/unit.pdf");

        btnRadio.Click += (_, _) =>
            Show(
                "دستگاه رادیوگرافی",
                "این قسمت مربوط به دستگاه‌های تصویربرداری است.",
                "Equipment/radio.png",
                "Equipment/radio.pdf");

        btnMaterial.Click += (_, _) =>
            Show(
                "قسمت تهیه مواد",
                "در این بخش مواد مصرفی آماده می‌شوند.",
                "Equipment/material.png",
                "Equipment/material.pdf");

        btnCompressor.Click += (_, _) =>
            Show(
                "کمپرسور",
                "کمپرسور هوای فشرده تجهیزات را تامین می‌کند.",
                "Equipment/compressor.png",
                "Equipment/compressor.pdf");

        btnAutoclave.Click += (_, _) =>
            Show(
                "اتوکلاو",
                "اتوکلاو برای استریل تجهیزات استفاده می‌شود.",
                "Equipment/autoclave.png",
                "Equipment/autoclave.pdf");

        Show(
            "یونیت دندانپزشکی",
            "یونیت دندانپزشکی اصلی‌ترین تجهیز هر کلینیک است.",
            "Equipment/unit.png",
            "Equipment/unit.pdf");
    }

    private void Show(
        string title,
        string description,
        string image,
        string pdf)
    {
        EquipmentTitle.Text = title;
        EquipmentDescription.Text = description;
        EquipmentImage.Source = AssetsHelper.LoadImage(image);

        btnPdf.Click -= OpenPdf;
        currentPdf = pdf;
        btnPdf.Click += OpenPdf;
    }

    private void OpenPdf(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AssetsHelper.OpenPdf(currentPdf);
    }
}
