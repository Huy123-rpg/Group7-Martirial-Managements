using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPF.PresentationLayer.Views.Auth;
using WPF.PresentationLayer.Views.Scheduling;
using WPF.PresentationLayer.ViewModels.Auth;
using WPF.PresentationLayer.ViewModels.Scheduling;
using WPF.PresentationLayer.ViewModels;
using BLL.BusinessLogicLayer;
using System;

namespace WPF.PresentationLayer
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Global exception handlers để bắt lỗi crash
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show($"Lỗi UI:\n{ex.Exception.Message}\n\n{ex.Exception.InnerException?.Message}\n\nStack:\n{ex.Exception.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                var err = ex.ExceptionObject as Exception;
                MessageBox.Show($"Lỗi nghiêm trọng:\n{err?.Message}\n\n{err?.InnerException?.Message}\n\nStack:\n{err?.StackTrace}",
                    "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
            };

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
}
