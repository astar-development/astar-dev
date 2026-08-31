using Avalonia.Controls;
using System.Globalization;

namespace AStarDev.FileApp;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void SetCountdown(int seconds) => CountdownText.Text = seconds.ToString(CultureInfo.InvariantCulture);
}