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

        // Admin menu - handled by binding in XAML
        // MenuHeaderAdmin.Visibility = adminVis;
        // BtnUsers.Visibility        = adminVis;

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

        // Schedule menu (Admin + Manager) - handled by binding in XAML
        // MenuHeaderSchedule.Visibility = scheduleVis;
        // BtnSchedule.Visibility        = scheduleVis;

        // My schedule (Staff) - handled by binding in XAML
        // MenuHeaderMySchedule.Visibility = myScheduleVis;
        // BtnMySchedule.Visibility        = myScheduleVis;

        // Import menu (Admin + Manager + Staff)
        var importVis = (isAdmin || isManager || isStaff) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderImport.Visibility = importVis;
        BtnPurchaseOrder.Visibility = importVis;
        BtnGoodsReceipt.Visibility  = importVis;

        // Export menu (Admin + Manager + Staff)
        var exportVis = (isAdmin || isManager || isStaff) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderExport.Visibility = exportVis;
        BtnSalesOrder.Visibility    = exportVis;
        BtnGoodsIssue.Visibility    = exportVis;
    }
}
