using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Services.Export;

public class GoodsIssueService : IGoodsIssueService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    private const byte TxnTypeIssue  = 2; // goods_issue
    private const byte TxnTypeAdjust = 3; // stock_adjustment (reversal)

    public IEnumerable<GoodsIssue> GetAll() =>
        _uow.GoodsIssues.GetAll().OrderByDescending(g => g.IssueDate);

    public IEnumerable<GoodsIssue> Search(string keyword) =>
        _uow.GoodsIssues.Find(g =>
            g.GiNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public GoodsIssue? GetById(Guid id) => _uow.GoodsIssues.GetById(id);

    public void Create(GoodsIssue gi)
    {
        gi.Id = Guid.NewGuid();
        gi.CreatedAt = DateTimeOffset.UtcNow;
        gi.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Add(gi);
        _uow.Save();
    }

    public void Update(GoodsIssue gi)
    {
        gi.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Update(gi);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        var items = _uow.GoodsIssueItems.GetAll().Where(x => x.GiId == id).ToList();
        foreach (var item in items)
            _uow.GoodsIssueItems.DeleteById(item.Id);
        _uow.GoodsIssues.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var gi = _uow.GoodsIssues.GetById(id) ?? throw new Exception("GI not found");

        var items = _uow.GoodsIssueItems.Find(i => i.GiId == id)
                        .Where(i => i.QtyIssued > 0).ToList();

        var invDb = _uow.Context.Set<Inventory>();
        var txnDb = _uow.Context.Set<StockTransaction>();
        var now   = DateTimeOffset.UtcNow;

        foreach (var item in items)
        {
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == gi.WarehouseId &&
                i.ProductId   == item.ProductId &&
                i.ZoneId      == item.ZoneId);

            if (inv == null)
                throw new InvalidOperationException(
                    $"Không tìm thấy tồn kho cho sản phẩm {item.ProductId} tại kho này.");

            if (inv.QtyOnHand < item.QtyIssued)
                throw new InvalidOperationException(
                    $"Tồn kho không đủ: cần {item.QtyIssued}, hiện có {inv.QtyOnHand}.");

            var newQty = inv.QtyOnHand - item.QtyIssued;
            inv.QtyOnHand   = newQty;
            inv.QtyReserved = Math.Max(0, inv.QtyReserved - item.QtyIssued);
            inv.LastUpdated = now;
            invDb.Update(inv);

            txnDb.Add(new StockTransaction
            {
                Id          = Guid.NewGuid(),
                TxnCode     = $"TXN-GI-{now:yyMMddHHmmss}-{item.ProductId.ToString()[..4].ToUpper()}",
                TxnTypeId   = TxnTypeIssue,
                ProductId   = item.ProductId,
                WarehouseId = gi.WarehouseId,
                ZoneId      = item.ZoneId,
                QtyChange   = -item.QtyIssued,
                QtyBalance  = newQty,
                UnitCost    = item.UnitCost,
                BatchNumber = item.BatchNumber,
                ExpiryDate  = item.ExpiryDate,
                RefType     = "GI",
                RefId       = gi.Id,
                RefItemId   = item.Id,
                PerformedBy = approvedBy,
                TxnAt       = now,
                Note        = $"Xuất kho theo phiếu {gi.GiNumber}",
            });
        }

        gi.StatusId  = 3;
        gi.ApprovedBy = approvedBy;
        gi.ApprovedAt = now;
        gi.UpdatedAt  = now;
        _uow.GoodsIssues.Update(gi);
        _uow.Save();
    }

    public void Cancel(Guid id, Guid cancelledBy, string reason)
    {
        var gi = _uow.GoodsIssues.GetById(id) ?? throw new Exception("GI not found");

        // Đảo ngược tồn kho nếu đã được duyệt
        if (gi.StatusId == 3)
        {
            var items = _uow.GoodsIssueItems.Find(i => i.GiId == id)
                            .Where(i => i.QtyIssued > 0).ToList();
            var invDb = _uow.Context.Set<Inventory>();
            var txnDb = _uow.Context.Set<StockTransaction>();
            var now   = DateTimeOffset.UtcNow;

            foreach (var item in items)
            {
                var inv = invDb.FirstOrDefault(i =>
                    i.WarehouseId == gi.WarehouseId &&
                    i.ProductId   == item.ProductId &&
                    i.ZoneId      == item.ZoneId);
                if (inv == null) continue;

                inv.QtyOnHand   += item.QtyIssued;
                inv.LastUpdated  = now;
                invDb.Update(inv);

                txnDb.Add(new StockTransaction
                {
                    Id          = Guid.NewGuid(),
                    TxnCode     = $"TXN-REV-GI-{now:yyMMddHHmmss}",
                    TxnTypeId   = TxnTypeAdjust,
                    ProductId   = item.ProductId,
                    WarehouseId = gi.WarehouseId,
                    ZoneId      = item.ZoneId,
                    QtyChange   = item.QtyIssued,
                    QtyBalance  = inv.QtyOnHand,
                    UnitCost    = item.UnitCost,
                    RefType     = "GI",
                    RefId       = gi.Id,
                    RefItemId   = item.Id,
                    PerformedBy = cancelledBy,
                    TxnAt       = now,
                    Note        = $"Hủy phiếu {gi.GiNumber}: {reason}",
                });
            }
        }

        gi.StatusId        = 4;
        gi.RejectionReason = reason;
        gi.UpdatedAt       = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Update(gi);
        _uow.Save();
    }
}
