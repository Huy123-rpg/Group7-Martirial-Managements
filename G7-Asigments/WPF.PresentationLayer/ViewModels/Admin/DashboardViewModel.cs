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

    public int TodayReceipts { get; }
    public int TodayIssues { get; }

    public decimal TotalReceiptAmount { get; }
    public decimal TotalIssueAmount { get; }

    public DashboardViewModel()
    {
        var uow = UnitOfWork.Instance;
        var today = DateOnly.FromDateTime(DateTime.Now);

        TotalUsers = uow.Users.GetAll().Count();
        ActiveUsers = uow.Users.Find(u => u.IsActive).Count();
        TotalProducts = uow.Products.GetAll().Count();
        TotalWarehouses = uow.Warehouses.GetAll().Count();

        TodayReceipts = uow.GoodsReceipts.GetAll().Count(x => x.ReceiptDate == today);
        TodayIssues = uow.GoodsIssues.GetAll().Count(x => x.IssueDate == today);

        TotalReceiptAmount = uow.GoodsReceipts.GetAll().Any()
            ? uow.GoodsReceipts.GetAll().Sum(x => x.TotalAmount)
            : 0;

        TotalIssueAmount = uow.GoodsIssues.GetAll().Any()
            ? uow.GoodsIssues.GetAll().Sum(x => x.TotalAmount)
            : 0;
    }
}