using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Services.Import;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── TxnTypeId constants ─────────────────────────────────────────────────
    private const byte TxnTypeReceipt = 1; // goods_receipt
    private const byte TxnTypeAdjust  = 3; // stock_adjustment (reversal)

    public IEnumerable<GoodsReceipt> GetAll() =>
        _uow.GoodsReceipts.GetAll().OrderByDescending(x => x.ReceiptDate);

    public GoodsReceipt? GetById(Guid id) =>
        _uow.GoodsReceipts.GetById(id);

    public void Create(GoodsReceipt receipt)
    {
        _uow.GoodsReceipts.Add(receipt);
        _uow.Save();
    }

    public void Update(GoodsReceipt receipt)
    {
        _uow.GoodsReceipts.Update(receipt);
        _uow.Save();
    }

    public IEnumerable<GoodsReceipt> Search(string keyword) =>
        _uow.GoodsReceipts.Find(g =>
            g.GrnNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public void Delete(Guid id)
    {
        // 1. Xoá hết các dòng chi tiết trước để tránh lỗi khoá ngoại (FK)
        var items = _uow.GoodsReceiptItems.GetAll().Where(x => x.GrnId == id).ToList();
        foreach (var item in items)
        {
            _uow.GoodsReceiptItems.DeleteById(item.Id);
        }

        // 2. Sau đó mới xoá phiếu nhập
        _uow.GoodsReceipts.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var gr = _uow.GoodsReceipts.GetById(id) ?? throw new Exception("GR not found");

        var items = _uow.GoodsReceiptItems.Find(i => i.GrnId == id)
                        .Where(i => i.QtyAccepted > 0).ToList();

        var invDb = _uow.Context.Set<Inventory>();
        var txnDb = _uow.Context.Set<StockTransaction>();
        var now   = DateTimeOffset.UtcNow;

        foreach (var item in items)
        {
            // 1. Tìm hoặc tạo mới bản ghi tồn kho
            var inv = invDb.FirstOrDefault(i =>
                i.WarehouseId == gr.WarehouseId &&
                i.ProductId   == item.ProductId &&
                i.ZoneId      == item.ZoneId);

            if (inv == null)
            {
                inv = new Inventory
                {
                    Id          = Guid.NewGuid(),
                    WarehouseId = gr.WarehouseId,
                    ProductId   = item.ProductId,
                    ZoneId      = item.ZoneId,
                    QtyOnHand   = 0,
                    QtyReserved = 0,
                    QtyIncoming = 0,
                    AvgCost     = item.UnitCost,
                    LastUpdated = now,
                };
                invDb.Add(inv);
            }

            // 2. Tính giá trung bình có trọng số
            var newQty     = inv.QtyOnHand + item.QtyAccepted;
            var newAvgCost = newQty > 0
                ? (inv.QtyOnHand * inv.AvgCost + item.QtyAccepted * item.UnitCost) / newQty
                : item.UnitCost;

            inv.QtyOnHand   = newQty;
            inv.AvgCost     = Math.Round(newAvgCost, 4);
            inv.QtyIncoming = Math.Max(0, inv.QtyIncoming - item.QtyAccepted);
            inv.LastUpdated = now;
            invDb.Update(inv);

            // 3. Tạo StockTransaction
            txnDb.Add(new StockTransaction
            {
                Id          = Guid.NewGuid(),
                TxnCode     = $"TXN-GR-{now:yyMMddHHmmss}-{item.ProductId.ToString()[..4].ToUpper()}",
                TxnTypeId   = TxnTypeReceipt,
                ProductId   = item.ProductId,
                WarehouseId = gr.WarehouseId,
                ZoneId      = item.ZoneId,
                QtyChange   = item.QtyAccepted,
                QtyBalance  = newQty,
                UnitCost    = item.UnitCost,
                BatchNumber = item.BatchNumber,
                ExpiryDate  = item.ExpiryDate,
                RefType     = "GRN",
                RefId       = gr.Id,
                RefItemId   = item.Id,
                PerformedBy = approvedBy,
                TxnAt       = now,
                Note        = $"Nhập kho từ phiếu {gr.GrnNumber}",
            });
        }

        gr.StatusId  = 3;
        gr.ApprovedBy = approvedBy;
        gr.ApprovedAt = now;
        gr.UpdatedAt  = now;
        _uow.GoodsReceipts.Update(gr);
        _uow.Save();
    }

    public void Cancel(Guid id, Guid cancelledBy, string reason)
    {
        var gr = _uow.GoodsReceipts.GetById(id) ?? throw new Exception("GR not found");

        // Nếu đã duyệt thì đảo ngược tồn kho
        if (gr.StatusId == 3)
        {
            var items = _uow.GoodsReceiptItems.Find(i => i.GrnId == id)
                            .Where(i => i.QtyAccepted > 0).ToList();
            var invDb = _uow.Context.Set<Inventory>();
            var txnDb = _uow.Context.Set<StockTransaction>();
            var now   = DateTimeOffset.UtcNow;

            foreach (var item in items)
            {
                var inv = invDb.FirstOrDefault(i =>
                    i.WarehouseId == gr.WarehouseId &&
                    i.ProductId   == item.ProductId &&
                    i.ZoneId      == item.ZoneId);
                if (inv == null) continue;

                inv.QtyOnHand   = Math.Max(0, inv.QtyOnHand - item.QtyAccepted);
                inv.LastUpdated = now;
                invDb.Update(inv);

                txnDb.Add(new StockTransaction
                {
                    Id          = Guid.NewGuid(),
                    TxnCode     = $"TXN-REV-GR-{now:yyMMddHHmmss}",
                    TxnTypeId   = TxnTypeAdjust,
                    ProductId   = item.ProductId,
                    WarehouseId = gr.WarehouseId,
                    ZoneId      = item.ZoneId,
                    QtyChange   = -item.QtyAccepted,
                    QtyBalance  = inv.QtyOnHand,
                    UnitCost    = item.UnitCost,
                    RefType     = "GRN",
                    RefId       = gr.Id,
                    RefItemId   = item.Id,
                    PerformedBy = cancelledBy,
                    TxnAt       = now,
                    Note        = $"Hủy phiếu {gr.GrnNumber}: {reason}",
                });
            }
        }

        gr.StatusId       = 4;
        gr.RejectionReason = reason;
        gr.UpdatedAt      = DateTimeOffset.UtcNow;
        _uow.GoodsReceipts.Update(gr);
        _uow.Save();
    }
}