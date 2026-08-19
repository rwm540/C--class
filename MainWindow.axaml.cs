using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using DentalCenter.Data;
using DentalCenter.Views;

namespace DentalCenter;

public partial class MainWindow : Window
{
    private sealed record NavItem(string Icon, string Title, Func<Control> Factory);

    private readonly List<Button> _navButtons = new();

    private static readonly NavItem[] Items =
    {
        new("🏠", "صفحه اصلی", () => new HomeView()),
        new("🦷", "تجهیزات", () => new EquipmentView()),
        new("🏢", "فضای فیزیکی", () => new PhysicalView()),
        new("💡", "بهره‌وری انرژی", () => new EnergyView()),
        new("👶", "بخش کودکان", () => new ChildrenView()),
        new("🧮", "محاسبهٔ انرژی", () => new CalculatorView()),
        new("☎", "تماس با ما", () => new ContactView()),
        new("📝", "ثبت نظرات", () => new FeedbackView()),
        new("💬", "نظرات و پیام‌ها", () => new CommentsView())
    };

    public MainWindow()
    {
        InitializeComponent();

        HeaderTitle.Text = ContentData.ProjectTitle;
        HeaderSubtitle.Text = ContentData.ProjectSubtitle;
        HeaderSubtitle.IsVisible = !string.IsNullOrEmpty(ContentData.ProjectSubtitle);
        FooterName.Text = ContentData.StudentName + " — " + ContentData.AcademicYear;
        StatusRight.Text = "نسخهٔ ۲٫۰";

        BuildNav();
        Select(0);

        btnAbout.Click += (_, _) => ShowAbout();
    }

    private void BuildNav()
    {
        for (var i = 0; i < Items.Length; i++)
        {
            var index = i;
            var item = Items[i];

            var button = new Button
            {
                Classes = { "nav" },
                Content = item.Icon + "   " + item.Title,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            button.Click += (_, _) => Select(index);

            _navButtons.Add(button);
            NavPanel.Children.Add(button);
        }
    }

    private void Select(int index)
    {
        for (var i = 0; i < _navButtons.Count; i++)
        {
            if (i == index)
                _navButtons[i].Classes.Add("selected");
            else
                _navButtons[i].Classes.Remove("selected");
        }

        var item = Items[index];
        StatusLeft.Text = item.Icon + "  " + item.Title;
        ShowView(item.Factory);
    }

    /// <summary>ساخت و نمایش صفحه؛ خطای یک صفحه نباید کل برنامه را از کار بیندازد.</summary>
    private void ShowView(Func<Control> factory)
    {
        try
        {
            MainContent.Content = factory();
        }
        catch (Exception ex)
        {
            Program.LogCrash("View", ex);

            MainContent.Content = new StackPanel
            {
                Spacing = 10,
                Margin = new Thickness(24),
                Children =
                {
                    new TextBlock
                    {
                        Text = "این صفحه الان قابل نمایش نیست.",
                        FontSize = 18,
                        FontWeight = FontWeight.Bold
                    },
                    new TextBlock
                    {
                        Text = ex.Message,
                        TextWrapping = TextWrapping.Wrap,
                        Opacity = 0.75
                    }
                }
            };
        }
    }

    private void ShowAbout()
    {
        var lines = new[]
        {
            ContentData.ProjectTitle,
            "",
            "دانشجو: " + ContentData.StudentName,
            "استاد راهنما: " + ContentData.SupervisorName,
            ContentData.University,
            ContentData.AcademicYear
        };

        var dialog = new Window
        {
            Title = "دربارهٔ برنامه",
            Width = 520,
            Height = 320,
            CanResize = false,
            FlowDirection = FlowDirection.RightToLeft,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new ScrollViewer
            {
                Padding = new Thickness(28),
                Content = new TextBlock
                {
                    Text = string.Join(Environment.NewLine, lines),
                    FontSize = 15,
                    LineHeight = 30,
                    TextWrapping = TextWrapping.Wrap
                }
            }
        };

        dialog.ShowDialog(this);
    }
}
