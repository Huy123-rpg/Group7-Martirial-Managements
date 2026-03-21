using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Import;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<PurchaseOrder> GetAll() =>
        _uow.PurchaseOrders.GetAll().OrderByDescending(p => p.OrderDate);

    public IEnumerable<PurchaseOrder> Search(string keyword) =>
        _uow.PurchaseOrders.Find(p =>
            p.PoNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public PurchaseOrder? GetById(Guid id) => _uow.PurchaseOrders.GetById(id);

    public void Create(PurchaseOrder po)
    {
        po.Id = Guid.NewGuid();
        po.CreatedAt = DateTimeOffset.UtcNow;
        po.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Add(po);
        _uow.Save();
    }

    public void Update(PurchaseOrder po)
    {
        po.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        _uow.PurchaseOrders.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var po = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        po.StatusId = 3; // approved
        po.ApprovedBy = approvedBy;
        po.ApprovedAt = DateTimeOffset.UtcNow;
        po.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }

    public void Reject(Guid id, string reason)
    {
        var po = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        po.StatusId = 5; // rejected
        po.RejectionReason = reason;
        po.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }
}
