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

    // ─── Role Visibility (stored — set once sau khi session sẵn sàng) ───────────
    private bool _showAdminMenu;
    private bool _showManagerMenu;
    private bool _showScheduleMenu;
    private bool _showMySchedule;

    public bool ShowAdminMenu    { get => _showAdminMenu;    set => SetField(ref _showAdminMenu,    value); }
    public bool ShowManagerMenu  { get => _showManagerMenu;  set => SetField(ref _showManagerMenu,  value); }
    public bool ShowScheduleMenu { get => _showScheduleMenu; set => SetField(ref _showScheduleMenu, value); }
    public bool ShowMySchedule   { get => _showMySchedule;   set => SetField(ref _showMySchedule,   value); }

    public void RefreshSession()
    {
        ShowAdminMenu    = SessionManager.IsAdmin;
        ShowManagerMenu  = SessionManager.IsAdmin || SessionManager.IsManager;
        ShowScheduleMenu = SessionManager.IsAdmin || SessionManager.IsManager;
        ShowMySchedule   = SessionManager.IsStaff;
        OnPropertyChanged(nameof(CurrentUserName));
        OnPropertyChanged(nameof(CurrentUserRole));
    }

    // ─── Nav Commands ─────────────────────────────────────────────────────────
    public RelayCommand NavDashboardCommand       => new(() => Navigate(new Views.Admin.DashboardView(),             "Tổng quan",             "dashboard"));
    public RelayCommand NavUserManagementCommand  => new(() => Navigate(new Views.Admin.UserManagementView(),        "Quản lý người dùng",    "users"));
    public RelayCommand NavScheduleCommand        => new(() => Navigate(new Views.Scheduling.ScheduleListView(),     "Phân công nhiệm vụ",    "schedule"));
    public RelayCommand NavMyScheduleCommand      => new(() => Navigate(new Views.Scheduling.MyScheduleView(),       "Lịch của tôi",          "my-schedule"));

    // ─── Logout ───────────────────────────────────────────────────────────────
    public RelayCommand LogoutCommand => new(Logout);

    public MainViewModel()
    {
        RefreshSession();
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
