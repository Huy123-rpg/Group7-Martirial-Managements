using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views;
using WPF.PresentationLayer.Views.Auth;
using WPF.PresentationLayer.Views.Export;
using WPF.PresentationLayer.Views.Import;
using Microsoft.Extensions.DependencyInjection;
using System;

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

    // ─── Role Visibility ─────────────────────────────────────────────────────
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
        OnPropertyChanged(nameof(CanViewDashboard));
        OnPropertyChanged(nameof(CanViewUsers));
        OnPropertyChanged(nameof(CanViewPurchaseOrder));
        OnPropertyChanged(nameof(CanViewGoodsReceipt));
        OnPropertyChanged(nameof(CanViewGoodsIssue));
    }

    // ─── Permission Props (HA) ────────────────────────────────────────────────
    public bool CanViewDashboard      => PermissionHelper.CanViewDashboard;
    public bool CanViewUsers          => PermissionHelper.CanViewUsers;
    public bool CanViewPurchaseOrder  => PermissionHelper.CanViewPurchaseOrder;
    public bool CanViewGoodsReceipt   => PermissionHelper.CanViewGoodsReceipt;
    public bool CanViewGoodsIssue     => PermissionHelper.CanViewGoodsIssue;

    // ─── Nav Commands ─────────────────────────────────────────────────────────
    public RelayCommand NavDashboardCommand      { get; }
    public RelayCommand NavUserManagementCommand { get; }
    public RelayCommand NavPurchaseOrderCommand  { get; }
    public RelayCommand NavGoodsReceiptCommand   { get; }
    public RelayCommand NavGoodsIssueCommand     { get; }
    public RelayCommand NavScheduleCommand       { get; }
    public RelayCommand NavMyScheduleCommand     { get; }
    public RelayCommand LogoutCommand            { get; }

    public MainViewModel()
    {
        NavDashboardCommand = new RelayCommand(() =>
            Navigate(new Views.Admin.DashboardView(), "Tổng quan", "dashboard"));

        NavUserManagementCommand = new RelayCommand(() =>
            Navigate(new Views.Admin.UserManagementView(), "Quản lý người dùng", "users"));

        NavPurchaseOrderCommand = new RelayCommand(() =>
            Navigate(new PurchaseOrderListView(), "Đặt hàng", "purchaseorder"));

        NavGoodsReceiptCommand = new RelayCommand(() =>
            Navigate(new GoodsReceiptListView(), "Nhập kho", "goodsreceipt"));

        NavGoodsIssueCommand = new RelayCommand(() =>
            Navigate(new GoodsIssueListView(), "Xuất kho", "goodsissue"));

        NavScheduleCommand = new RelayCommand(() =>
            Navigate(new Views.Scheduling.ScheduleListView(), "Phân công nhiệm vụ", "schedule"));

        NavMyScheduleCommand = new RelayCommand(() =>
            Navigate(new Views.Scheduling.MyScheduleView(), "Lịch của tôi", "my-schedule"));

        LogoutCommand = new RelayCommand(Logout);

        RefreshSession();
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

        var login = App.ServiceProvider.GetRequiredService<LoginWindow>();
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