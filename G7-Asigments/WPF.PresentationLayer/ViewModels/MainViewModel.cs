using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views;
using WPF.PresentationLayer.Views.Auth;
using WPF.PresentationLayer.Views.Export;
using WPF.PresentationLayer.Views.Import;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels;



public class MainViewModel : BaseViewModel
{
    private object? _currentView;
    private string _currentPageTitle = string.Empty;
    private string _activeMenu = "dashboard";

    public object? CurrentView
    {
        get => _currentView;
        set => SetField(ref _currentView, value);
    }

    public string CurrentPageTitle
    {
        get => _currentPageTitle;
        set => SetField(ref _currentPageTitle, value);
    }

    public string ActiveMenu
    {
        get => _activeMenu;
        set => SetField(ref _activeMenu, value);
    }

    public string CurrentUserName => SessionManager.CurrentUser?.FullName ?? string.Empty;
    public string CurrentUserRole => SessionManager.CurrentUser?.RoleId switch
    {
        1 => "Admin",
        2 => "Warehouse Manager",
        3 => "Warehouse Staff",
        4 => "Accountant",
        _ => "Unknown"
    };

    public RelayCommand NavDashboardCommand { get; }
    public RelayCommand NavUserManagementCommand { get; }
    public RelayCommand NavGoodsReceiptCommand { get; }
    public RelayCommand NavGoodsIssueCommand { get; }
    public RelayCommand LogoutCommand { get; }
    public bool CanViewUsers => PermissionHelper.CanViewUsers;
    public string DebugRole => $"RoleId = {SessionManager.CurrentUser?.RoleId}";
    public bool CanViewGoodsReceipt => PermissionHelper.CanViewGoodsReceipt;
    public bool CanViewGoodsIssue => PermissionHelper.CanViewGoodsIssue;

    public bool CanViewDashboard => PermissionHelper.CanViewDashboard;
    public bool CanCreateGoodsReceipt => PermissionHelper.CanCreateGoodsReceipt;
    public bool CanCreateGoodsIssue => PermissionHelper.CanCreateGoodsIssue;

    public MainViewModel()
    {
        NavDashboardCommand = new RelayCommand(() =>
            Navigate(new Views.Admin.DashboardView(), "Tổng quan", "dashboard"));

        NavUserManagementCommand = new RelayCommand(() =>
            Navigate(new Views.Admin.UserManagementView(), "Quản lý người dùng", "users"));

        NavGoodsReceiptCommand = new RelayCommand(() =>
            Navigate(new GoodsReceiptListView(), "Nhập kho", "goodsreceipt"));

        NavGoodsIssueCommand = new RelayCommand(() =>
            Navigate(new GoodsIssueListView(), "Xuất kho", "goodsissue"));

        LogoutCommand = new RelayCommand(Logout);

        Navigate(new Views.Admin.DashboardView(), "Tổng quan", "dashboard");
    }

    private void Navigate(UserControl view, string title, string menuKey)
    {
        CurrentView = view;
        CurrentPageTitle = title;
        ActiveMenu = menuKey;
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