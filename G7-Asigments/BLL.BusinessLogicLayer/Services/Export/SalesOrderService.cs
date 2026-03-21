using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Export;

public class SalesOrderService : ISalesOrderService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<SalesOrder> GetAll() =>
        _uow.SalesOrders.GetAll().OrderByDescending(s => s.OrderDate);

    public IEnumerable<SalesOrder> Search(string keyword) =>
        _uow.SalesOrders.Find(s =>
            s.SoNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public SalesOrder? GetById(Guid id) => _uow.SalesOrders.GetById(id);

    public void Create(SalesOrder so)
    {
        so.Id = Guid.NewGuid();
        so.CreatedAt = DateTimeOffset.UtcNow;
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Add(so);
        _uow.Save();
    }

    public void Update(SalesOrder so)
    {
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        _uow.SalesOrders.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");
        so.StatusId = 3;
        so.ApprovedBy = approvedBy;
        so.ApprovedAt = DateTimeOffset.UtcNow;
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    public void Reject(Guid id, string reason)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");
        so.StatusId = 5;
        so.RejectionReason = reason;
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }
}
