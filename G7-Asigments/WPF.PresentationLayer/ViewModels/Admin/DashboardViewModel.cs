using BLL.BusinessLogicLayer.Core;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class DashboardViewModel : BaseViewModel
{
    public int TotalUsers      { get; }
    public int ActiveUsers     { get; }
    public int TotalProducts   { get; }
    public int TotalWarehouses { get; }

    public int PendingPO  { get; }
    public int PendingSO  { get; }
    public int PendingGRN { get; }
    public int PendingGI  { get; }
    public int LowStockCount { get; }
    public string TotalInventoryValue { get; } = "0 ₫";

    public DashboardViewModel()
    {
        try
        {
            var uow = UnitOfWork.Instance;
            TotalUsers      = uow.Users.GetAll().Count();
            ActiveUsers     = uow.Users.Find(u => u.IsActive).Count();
            TotalProducts   = uow.Products.GetAll().Count();
            TotalWarehouses = uow.Warehouses.GetAll().Count();

            PendingPO  = uow.PurchaseOrders.Find(p => p.StatusId == 2).Count();
            PendingSO  = uow.SalesOrders.Find(s => s.StatusId == 2).Count();
            PendingGRN = uow.GoodsReceipts.Find(r => r.StatusId == 2).Count();
            PendingGI  = uow.GoodsIssues.Find(i => i.StatusId == 2).Count();

            var inventories = uow.Inventories.GetAll().ToList();
            LowStockCount = inventories.Count(i => i.QtyOnHand <= 10);
            var totalValue = inventories.Sum(i => i.QtyOnHand * i.AvgCost);
            TotalInventoryValue = totalValue.ToString("N0") + " ₫";
        }
        catch { /* DB chưa sẵn sàng — hiển thị 0 */ }
    }
}
