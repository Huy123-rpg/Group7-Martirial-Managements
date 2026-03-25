using System;
using System.Linq;
using BLL.BusinessLogicLayer.Core;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class DashboardViewModel : BaseViewModel
{
    public int TotalUsers { get; }
    public int ActiveUsers { get; }
    public int TotalProducts { get; }
    public int TotalWarehouses { get; }

    // Chỉ đếm phiếu đã được Admin DUYỆT (StatusId = 2)
    public int TodayApprovedReceipts { get; }
    public int TodayApprovedIssues { get; }

    // Phiếu đang chờ duyệt (StatusId = 1)
    public int PendingReceipts { get; }
    public int PendingIssues { get; }

    // Tổng tiền chỉ từ phiếu đã duyệt
    public decimal TotalReceiptAmount { get; }
    public decimal TotalIssueAmount { get; }

    public DashboardViewModel()
    {
        try
        {
            var uow = UnitOfWork.Instance;
            var today = DateOnly.FromDateTime(DateTime.Now);

            TotalUsers     = uow.Users.GetAll().Count();
            ActiveUsers    = uow.Users.Find(u => u.IsActive).Count();
            TotalProducts  = uow.Products.GetAll().Count();
            TotalWarehouses = uow.Warehouses.GetAll().Count();

            var allReceipts = uow.GoodsReceipts.GetAll().ToList();
            var allIssues   = uow.GoodsIssues.GetAll().ToList();

            // Phiếu đã duyệt hôm nay (StatusId = 2)
            TodayApprovedReceipts = allReceipts.Count(x => x.ReceiptDate == today && x.StatusId == 2);
            TodayApprovedIssues   = allIssues.Count(x => x.IssueDate == today && x.StatusId == 2);

            // Phiếu đang chờ duyệt (StatusId = 1)
            PendingReceipts = allReceipts.Count(x => x.StatusId == 1);
            PendingIssues   = allIssues.Count(x => x.StatusId == 1);

            // Tổng tiền chỉ tính phiếu đã duyệt
            TotalReceiptAmount = allReceipts.Where(x => x.StatusId == 2).Sum(x => x.TotalAmount);
            TotalIssueAmount   = allIssues.Where(x => x.StatusId == 2).Sum(x => x.TotalAmount);
        }
        catch { /* DB chưa sẵn sàng — hiển thị 0 */ }
    }
}