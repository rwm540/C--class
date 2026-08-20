using Avalonia.Controls;
using DentalCenter.Data;
using DentalCenter.Helpers;

namespace DentalCenter.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();

        HeroTitle.Text = ContentData.ProjectTitle;
        HeroSubtitle.Text = ContentData.ProjectSubtitle;
        HeroSubtitle.IsVisible = !string.IsNullOrEmpty(ContentData.ProjectSubtitle);
        IntroText.Text = ContentData.HomeIntro;

        StudentText.Text = ContentData.StudentName;
        SupervisorText.Text = ContentData.SupervisorName;
        UniversityText.Text = ContentData.University;
        YearText.Text = ContentData.AcademicYear;

        StatList.ItemsSource = ContentData.HomeStats;

        ImagingNote.Text = ContentData.HomeImagingNote;
        AiNote.Text = ContentData.HomeAiNote;

        // نبود تصویر نباید برنامه را متوقف کند.
        HeroImage.Source = AssetsHelper.LoadImage("clinic.jpg")
                           ?? AssetsHelper.LoadImage("Energy/building.jpg");
        ImagingImage.Source = AssetsHelper.LoadImage("Physical/imaging.jpg")
                              ?? AssetsHelper.LoadImage("Equipment/radio.jpg");
        AiImage.Source = AssetsHelper.LoadImage("Energy/ai.jpg")
                         ?? AssetsHelper.LoadImage("Energy/lighting.jpg");
    }
}
