using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace DentalCenter;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                desktop.MainWindow = new MainWindow();
            }
            catch (Exception ex)
            {
                Program.LogCrash("MainWindow", ex);
                desktop.MainWindow = CreateFallbackWindow(ex);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Window CreateFallbackWindow(Exception ex)
    {
        return new Window
        {
            Title = "Dental Center",
            Width = 720,
            Height = 420,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new TextBlock
            {
                Text = "برنامه باز شد، اما صفحه اصلی بارگذاری نشد.\n\n" + ex.Message,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(24),
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }
}
