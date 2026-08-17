using Avalonia.Controls;

namespace DentalCenter.Views;

public partial class EnergyView : UserControl
{
    public EnergyView()
    {
        InitializeComponent();

        btnLight.Click += (_, _) =>
        {
            EnergyTitle.Text = "روشنایی";

            EnergyDescription.Text =
            "استفاده از چراغ‌های LED، سنسور حضور، کنترل هوشمند و نور طبیعی برای کاهش مصرف انرژی.";
        };

        btnCooling.Click += (_, _) =>
        {
            EnergyTitle.Text = "سرمایش";

            EnergyDescription.Text =
            "استفاده از سیستم‌های VRF، چیلرهای کم‌مصرف و عایق‌بندی مناسب.";
        };

        btnHeating.Click += (_, _) =>
        {
            EnergyTitle.Text = "گرمایش";

            EnergyDescription.Text =
            "بویلرهای چگالشی، پمپ حرارتی و کنترل دمای هوشمند.";
        };

        btnVentilation.Click += (_, _) =>
        {
            EnergyTitle.Text = "تهویه";

            EnergyDescription.Text =
            "تهویه مناسب جهت حفظ کیفیت هوا و کاهش مصرف انرژی.";
        };

        btnWindow.Click += (_, _) =>
        {
            EnergyTitle.Text = "پنجره‌ها";

            EnergyDescription.Text =
            "شیشه‌های دوجداره Low-E و قاب‌های عایق برای کاهش اتلاف انرژی.";
        };

        btnSolar.Click += (_, _) =>
        {
            EnergyTitle.Text = "نور طبیعی";

            EnergyDescription.Text =
            "استفاده از نور روز جهت کاهش مصرف برق و افزایش آسایش کاربران.";
        };
    }
}