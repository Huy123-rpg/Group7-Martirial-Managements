using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Auth;
using DAL.DataAccessLayer.Model;
using BLL.BusinessLogicLayer.Core;
using Microsoft.Extensions.DependencyInjection;

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
        LoadNotifications();
    }

    // ─── Notifications ────────────────────────────────────────────────────────
    private ObservableCollection<Notification> _unreadNotifications = [];
    public ObservableCollection<Notification> UnreadNotifications
    {
        get => _unreadNotifications;
        set { SetField(ref _unreadNotifications, value); OnPropertyChanged(nameof(HasUnreadNotifications)); }
    }

    public bool HasUnreadNotifications => UnreadNotifications.Count > 0;

    private void LoadNotifications()
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return;
        
        var uow = UnitOfWork.Instance;
        var list = uow.Notifications.Find(n => n.UserId == user.Id && !n.IsRead)
                                    .OrderByDescending(n => n.CreatedAt)
                                    .ToList();
        UnreadNotifications = new ObservableCollection<Notification>(list);
    }

    public RelayCommand<Notification> MarkNotificationReadCommand => new(MarkAsRead);
    public RelayCommand<Notification> ViewNotificationDetailCommand => new(ViewDetail);

    private void ViewDetail(Notification? n)
    {
        if (n == null) return;
        
        if (n.RefType == "WAREHOUSE_PROPOSAL")
        {
            var result = MessageBox.Show(
                $"{n.Body}\n\nBạn có CHẤP NHẬN đề xuất này không?", 
                $"Chi tiết đề xuất: {n.Title}", 
                MessageBoxButton.YesNoCancel, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes || result == MessageBoxResult.No)
            {
                var uow = UnitOfWork.Instance;
                var adminName = SessionManager.CurrentUser?.FullName ?? "Admin";
                string statusText = result == MessageBoxResult.Yes ? "CHẤP NHẬN" : "TỪ CHỐI";

                // Generate response notification if RefId (manager's UserId) is valid
                if (n.RefId.HasValue && n.RefId != Guid.Empty)
                {
                    uow.Notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = n.RefId.Value, // Send back to manager
                        Title = $"Kết quả đề xuất: {statusText}",
                        Body = $"Admin {adminName} đã {statusText} đề xuất của bạn:\n\n{n.Body}",
                        Channel = "IN_APP",
                        IsRead = false,
                        SentAt = DateTimeOffset.UtcNow,
                        CreatedAt = DateTimeOffset.UtcNow,
                    });
                    uow.Save();
                }

                // Delete the original request since it has been decided
                var existing = uow.Notifications.GetById(n.Id);
                if (existing != null)
                {
                    uow.Notifications.DeleteById(n.Id);
                    uow.Save();
                    LoadNotifications();
                    return; // Return early since deleted
                }
            }
        }
        else if (n.RefType == "STOCK_COUNT_APPROVAL")
        {
            NavStockCountApprovalCommand.Execute(null);
        }
        else
        {
            MessageBox.Show($"{n.Body}", $"Chi tiết thông báo: {n.Title}", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        // Auto mark as read when viewed if not decided
        if (!n.IsRead) MarkAsRead(n);
    }

    private void MarkAsRead(Notification? n)
    {
        if (n == null) return;
        var uow = UnitOfWork.Instance;
        var entity = uow.Notifications.GetById(n.Id);
        if (entity != null)
        {
            entity.IsRead = true;
            entity.ReadAt = DateTimeOffset.UtcNow;
            uow.Notifications.Update(entity);
            uow.Save();
            LoadNotifications();
        }
    }

    // ─── Nav Commands ─────────────────────────────────────────────────────────
    public RelayCommand NavDashboardCommand       => new(() => Navigate(new Views.Admin.DashboardView(),             "Tổng quan",             "dashboard"));
    public RelayCommand NavUserManagementCommand  => new(() => Navigate(new Views.Admin.UserManagementView(),        "Quản lý người dùng",    "users"));
    public RelayCommand NavWarehouseConfigCommand => new(() => Navigate(new Views.Admin.WarehouseConfigView(),       "Cấu hình kho & zone",   "warehouse-config"));
    public RelayCommand NavInventoryLookupCommand => new(() => Navigate(new Views.Inventory.InventoryLookupView(),   "Tra cứu tồn kho",       "inventory-lookup"));
    public RelayCommand NavStockCountExecutionCommand => new(() => Navigate(new Views.Inventory.StockCountExecutionListView(), "Kiểm kê kho", "stock-count-execution"));
    public RelayCommand NavStockCountApprovalCommand  => new(() => Navigate(new Views.Inventory.StockCountApprovalView(),  "Duyệt kiểm kê kho", "stock-count-approval"));
    public RelayCommand NavScheduleCommand        => new(() => Navigate(new Views.Scheduling.ScheduleListView(),     "Phân công nhiệm vụ",    "schedule"));
    public RelayCommand NavMyScheduleCommand      => new(() => Navigate(new Views.Scheduling.MyScheduleView(),       "Lịch của tôi",          "my-schedule"));
    public RelayCommand NavPurchaseOrderCommand   => new(() => Navigate(new Views.Import.PurchaseOrderListView(),    "Đơn đặt hàng",          "purchase-order"));
    public RelayCommand NavGoodsReceiptCommand    => new(() => Navigate(new Views.Import.GoodsReceiptListView(),     "Phiếu nhập kho",        "goods-receipt"));
    public RelayCommand NavSalesOrderCommand      => new(() => Navigate(new Views.Export.SalesOrderListView(),       "Đơn bán hàng",          "sales-order"));
    public RelayCommand NavGoodsIssueCommand      => new(() => Navigate(new Views.Export.GoodsIssueListView(),       "Phiếu xuất kho",        "goods-issue"));

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
