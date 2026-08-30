using Avalonia.Controls;

namespace AStarDev.FileApp;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void SetCountdown(int seconds) => CountdownText.Text = seconds.ToString();
}