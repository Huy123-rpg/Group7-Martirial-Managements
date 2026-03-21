using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Auth;

namespace WPF.PresentationLayer.ViewModels;

public class MainViewModel : BaseViewModel
{
    // ─── Navigation State ────────────────────────────────────────────────────
    private object? _currentView;
    private string _currentPageTitle = string.Empty;
    private string _activeMenu = "dashboard";

    public object? CurrentView        { get => _currentView;      set => SetField(ref _currentView, value); }
    public string  CurrentPageTitle   { get => _currentPageTitle; set => SetField(ref _currentPageTitle, value); }
    public string  ActiveMenu         { get => _activeMenu;       set => SetField(ref _activeMenu, value); }

    // ─── Session Info ────────────────────────────────────────────────────────
    public string CurrentUserName => SessionManager.CurrentUser?.FullName ?? string.Empty;
    public string CurrentUserRole => SessionManager.CurrentUser?.Role?.RoleName ?? string.Empty;

    // ─── Nav Commands ─────────────────────────────────────────────────────────
    public RelayCommand NavDashboardCommand       => new(() => Navigate(new Views.Admin.DashboardView(),       "Tổng quan",             "dashboard"));
    public RelayCommand NavUserManagementCommand  => new(() => Navigate(new Views.Admin.UserManagementView(),  "Quản lý người dùng",    "users"));

    // ─── Logout ───────────────────────────────────────────────────────────────
    public RelayCommand LogoutCommand => new(Logout);

    public MainViewModel()
    {
        Navigate(new Views.Admin.DashboardView(), "Tổng quan", "dashboard");
    }

    private void Navigate(UserControl view, string title, string menuKey)
    {
        CurrentView      = view;
        CurrentPageTitle = title;
        ActiveMenu       = menuKey;
    }

    private void Logout()
    {
        SessionManager.Logout();
        var login = new LoginWindow();
        login.Show();

        foreach (Window w in Application.Current.Windows)
        {
            if (w is MainWindow)
            {
                w.Close();
                return;
            }
        }
    }
}
