using BLL.BusinessLogicLayer.Core;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class DashboardViewModel : BaseViewModel
{
    public int TotalUsers      { get; }
    public int ActiveUsers     { get; }
    public int TotalProducts   { get; }
    public int TotalWarehouses { get; }

    public DashboardViewModel()
    {
        var uow = UnitOfWork.Instance;
        TotalUsers      = uow.Users.GetAll().Count();
        ActiveUsers     = uow.Users.Find(u => u.IsActive).Count();
        TotalProducts   = uow.Products.GetAll().Count();
        TotalWarehouses = uow.Warehouses.GetAll().Count();
    }
}
