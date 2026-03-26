using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

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

    public void Submit(Guid id)
    {
        var po = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        po.StatusId = 2; // waiting for approval
        po.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var po    = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        var items = _uow.PurchaseOrderItems.Find(i => i.PoId == id).ToList();
        var invDb = _uow.Context.Set<Inventory>();
        var now   = DateTimeOffset.UtcNow;

        foreach (var item in items.Where(i => i.QtyOrdered > 0))
        {
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == po.WarehouseId && i.ProductId == item.ProductId);

            if (inv == null)
            {
                inv = new Inventory
                {
                    Id          = Guid.NewGuid(),
                    WarehouseId = po.WarehouseId,
                    ProductId   = item.ProductId,
                    QtyOnHand   = 0,
                    QtyReserved = 0,
                    QtyIncoming = item.QtyOrdered,
                    AvgCost     = item.UnitCost,
                    LastUpdated = now,
                };
                invDb.Add(inv);
            }
            else
            {
                inv.QtyIncoming += item.QtyOrdered;
                inv.LastUpdated  = now;
                invDb.Update(inv);
            }
        }

        po.StatusId  = 3;
        po.ApprovedBy = approvedBy;
        po.ApprovedAt = now;
        po.UpdatedAt  = now;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }

    public void Reject(Guid id, string reason)
    {
        var po = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        po.StatusId        = 5;
        po.RejectionReason = reason;
        po.UpdatedAt       = DateTimeOffset.UtcNow;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }

    public void Cancel(Guid id, Guid cancelledBy)
    {
        var po  = _uow.PurchaseOrders.GetById(id) ?? throw new Exception("PO not found");
        var now = DateTimeOffset.UtcNow;

        // If already approved, reverse QtyIncoming
        if (po.StatusId == 3)
        {
            var items = _uow.PurchaseOrderItems.Find(i => i.PoId == id).ToList();
            var invDb = _uow.Context.Set<Inventory>();
            foreach (var item in items.Where(i => i.QtyOrdered > 0))
            {
                var inv = invDb.FirstOrDefault(i =>
                    i.WarehouseId == po.WarehouseId && i.ProductId == item.ProductId);
                if (inv == null) continue;
                inv.QtyIncoming = Math.Max(0, inv.QtyIncoming - item.QtyOrdered);
                inv.LastUpdated = now;
                invDb.Update(inv);
            }
        }

        po.StatusId  = 5;
        po.UpdatedAt = now;
        _uow.PurchaseOrders.Update(po);
        _uow.Save();
    }
}
