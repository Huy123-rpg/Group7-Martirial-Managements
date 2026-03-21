using System.Windows;
using WPF.PresentationLayer.Views.Auth;

namespace WPF.PresentationLayer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        new LoginWindow().Show();
    }
}
