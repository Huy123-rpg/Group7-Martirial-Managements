using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPF.PresentationLayer.Views.Auth;
using WPF.PresentationLayer.Views.Scheduling;
using WPF.PresentationLayer.ViewModels.Auth;
using WPF.PresentationLayer.ViewModels.Scheduling;
using WPF.PresentationLayer.ViewModels;
using BLL.BusinessLogicLayer;
using System;

namespace WPF.PresentationLayer;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        // Không tự shutdown khi đóng LoginWindow — chỉ shutdown khi gọi tường minh
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 1. Business Logic & Data Access (via BLL extension)
        services.AddBusinessLogic();

        // 2. ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<ChangePasswordViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<MyScheduleViewModel>();
        services.AddTransient<ScheduleFormViewModel>();
        services.AddTransient<ScheduleCalendarViewModel>();
        services.AddTransient<ViewModels.Admin.UserManagementViewModel>();

        // 3. Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<RegisterWindow>();
        services.AddTransient<ChangePasswordWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<ScheduleFormWindow>();
    }
}
