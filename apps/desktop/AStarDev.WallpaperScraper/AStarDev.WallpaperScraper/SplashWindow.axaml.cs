using Avalonia.Controls;

namespace AStarDev.WallpaperScraper;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void SetCountdown(int seconds) => CountdownText.Text = seconds.ToString();
}