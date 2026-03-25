using System.Windows;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.ViewModels;

namespace WPF.PresentationLayer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        ApplySidebarVisibility();
    }

    private void ApplySidebarVisibility()
    {
        bool isAdmin   = SessionManager.IsAdmin;
        bool isManager = SessionManager.IsManager;
        bool isStaff   = SessionManager.IsStaff;

        // Admin menu
        var adminVis = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderAdmin.Visibility = adminVis;
        BtnUsers.Visibility        = adminVis;

        // Warehouse config (Admin + Manager)
        var whConfigVis = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderWarehouse.Visibility = whConfigVis;
        BtnWarehouseConfig.Visibility  = whConfigVis;

        // Inventory lookup (All except Supplier)
        var isSupplier = SessionManager.IsSupplier;
        var invVis = (!isSupplier) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderInventory.Visibility = invVis;
        BtnInventoryLookup.Visibility  = invVis;

        // Stock Count Approval (Admin + Manager + Accountant)
        bool isAccountant = SessionManager.CurrentUser?.Role?.RoleCode
            ?.Equals("ACCOUNTANT", StringComparison.OrdinalIgnoreCase) == true;
        var adjVis = (isAdmin || isManager || isAccountant) ? Visibility.Visible : Visibility.Collapsed;
        BtnStockCountApproval.Visibility = adjVis;

        // Execute Stock Count is STRICTLY FOR STAFF
        if (!SessionManager.IsStaff)
        {
            BtnStockCountExecution.Visibility = Visibility.Collapsed;
        }

        // Schedule menu (Admin + Manager)
        var scheduleVis = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderSchedule.Visibility = scheduleVis;
        BtnSchedule.Visibility        = scheduleVis;

        // My schedule (Staff)
        var myScheduleVis = isStaff ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderMySchedule.Visibility = myScheduleVis;
        BtnMySchedule.Visibility        = myScheduleVis;
    }
}
