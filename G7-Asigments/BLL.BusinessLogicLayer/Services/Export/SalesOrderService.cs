using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore;

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

    public void Submit(Guid id)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");
        so.StatusId = 2;
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");

        var items  = _uow.SalesOrderItems.Find(i => i.SoId == id).ToList();
        var invDb  = _uow.Context.Set<Inventory>();

        // Kiểm tra tồn kho khả dụng trước khi duyệt
        foreach (var item in items)
        {
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == so.WarehouseId && i.ProductId == item.ProductId);

            var available = (inv?.QtyOnHand ?? 0) - (inv?.QtyReserved ?? 0);
            if (available < item.QtyOrdered)
                throw new InvalidOperationException(
                    $"Tồn kho khả dụng không đủ cho sản phẩm {item.ProductId}: " +
                    $"cần {item.QtyOrdered}, còn lại {available}.");
        }

        // Đặt chỗ tồn kho
        var now = DateTimeOffset.UtcNow;
        foreach (var item in items)
        {
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == so.WarehouseId && i.ProductId == item.ProductId);
            if (inv == null) continue;

            inv.QtyReserved += item.QtyOrdered;
            inv.LastUpdated  = now;
            invDb.Update(inv);
        }

        so.StatusId  = 3;
        so.ApprovedBy = approvedBy;
        so.ApprovedAt = now;
        so.UpdatedAt  = now;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    public void Reject(Guid id, string reason)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");

        // Nếu đã duyệt thì giải phóng QtyReserved
        if (so.StatusId == 3)
            ReleaseReservation(so);

        so.StatusId        = 5;
        so.RejectionReason = reason;
        so.UpdatedAt       = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    public void Cancel(Guid id, Guid cancelledBy)
    {
        var so = _uow.SalesOrders.GetById(id) ?? throw new Exception("SO not found");

        // If already approved, release reserved stock
        if (so.StatusId == 3)
            ReleaseReservation(so);

        so.StatusId  = 5;
        so.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.SalesOrders.Update(so);
        _uow.Save();
    }

    private void ReleaseReservation(SalesOrder so)
    {
        var items = _uow.SalesOrderItems.Find(i => i.SoId == so.Id).ToList();
        var invDb = _uow.Context.Set<Inventory>();
        var now   = DateTimeOffset.UtcNow;

        foreach (var item in items)
        {
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == so.WarehouseId && i.ProductId == item.ProductId);
            if (inv == null) continue;

            inv.QtyReserved = Math.Max(0, inv.QtyReserved - item.QtyOrdered);
            inv.LastUpdated = now;
            invDb.Update(inv);
        }
    }
}
