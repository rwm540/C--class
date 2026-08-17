using Avalonia.Controls;

namespace DentalCenter.Views;

public partial class PhysicalView : UserControl
{
    public PhysicalView()
    {
        InitializeComponent();

        btnRoom.Click += (_, _) =>
        {
            RoomTitle.Text = "اتاق یونیت";

            RoomDescription.Text =
            "حداقل فضای لازم برای نصب یونیت، فاصله مناسب تجهیزات، تهویه، نور طبیعی، سرمایش و گرمایش در این قسمت توضیح داده می‌شود.";
        };

        btnWaiting.Click += (_, _) =>
        {
            RoomTitle.Text = "اتاق انتظار";

            RoomDescription.Text =
            "طراحی فضای انتظار، مبلمان، نورپردازی، تهویه و آسایش بیماران.";
        };

        btnReception.Click += (_, _) =>
        {
            RoomTitle.Text = "پذیرش";

            RoomDescription.Text =
            "طراحی کانتر پذیرش، کامپیوتر، پرونده الکترونیکی و ارگونومی کارکنان.";
        };

        btnImaging.Click += (_, _) =>
        {
            RoomTitle.Text = "اتاق تصویربرداری";

            RoomDescription.Text =
            "الزامات نصب تجهیزات تصویربرداری و حفاظت در برابر اشعه.";
        };

        btnService.Click += (_, _) =>
        {
            RoomTitle.Text = "سرویس بهداشتی";

            RoomDescription.Text =
            "محل قرارگیری، تهویه و استانداردهای طراحی سرویس بهداشتی.";
        };
    }
}