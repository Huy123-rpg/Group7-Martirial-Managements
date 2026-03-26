using System.Windows;
using WPF.PresentationLayer.Views.Auth;

namespace WPF.PresentationLayer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Không tự shutdown khi đóng LoginWindow — chỉ shutdown khi gọi tường minh
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        new LoginWindow().Show();
    }
}
