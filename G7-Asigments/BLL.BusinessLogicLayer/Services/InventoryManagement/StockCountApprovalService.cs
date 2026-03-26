using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public class StockCountApprovalService : IStockCountApprovalService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── Status IDs (from lkp_document_status) ────────────────────────────────
    public const byte StatusDraft     = 1;
    public const byte StatusPending   = 2;
    public const byte StatusApproved  = 3; // Manager approved
    public const byte StatusRejected  = 4; // Manager rejected
    public const byte StatusCancelled = 5;
    public const byte StatusCompleted = 6; // Staff completed count -> Pending Approval

    // ─── Queries ─────────────────────────────────────────────────────────────
    public IEnumerable<StockCountSession> GetAll()
    {
        // Admin or Accountant: View all sessions that are Completed (Waiting Approval), Approved, or Rejected
        var sessions = _uow.Context.Set<StockCountSession>()
            .Include(s => s.Status)
            .Include(s => s.Warehouse)
            .Include(s => s.AssignedToNavigation)
            .Include(s => s.ApprovedByNavigation)
            .Include(s => s.StockCountItems)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(s => s.StatusId == StatusCompleted || s.StatusId == StatusApproved || s.StatusId == StatusRejected)
            .OrderByDescending(s => s.CompletedAt ?? s.CreatedAt)
            .ToList();

        PopulateScheduleTitles(sessions);
        PopulateQtyAvailable(sessions);

        return sessions;
    }

    private void PopulateScheduleTitles(List<StockCountSession> sessions)
    {
        if (!sessions.Any()) return;
        var sessionIds = sessions.Select(s => s.Id).ToList();
        var schedules = _uow.Context.Set<Schedule>().AsNoTracking()
            .Where(sch => sch.RefType == "STOCK_COUNT" && sch.RefId.HasValue && sessionIds.Contains(sch.RefId.Value))
            .ToList();

        foreach (var session in sessions)
        {
            var sch = schedules.FirstOrDefault(s => s.RefId == session.Id);
            session.ScheduleTitle = sch?.Title ?? "Kiểm kho thủ công";
        }
    }

    private void PopulateQtyAvailable(List<StockCountSession> sessions)
    {
        foreach (var session in sessions)
        {
            var productIds = session.StockCountItems.Select(i => i.ProductId).Distinct().ToList();
            var inventories = _uow.Context.Set<Inventory>().AsNoTracking()
                .Where(inv => inv.WarehouseId == session.WarehouseId && productIds.Contains(inv.ProductId))
                .ToList();

            foreach (var item in session.StockCountItems)
            {
                var inv = inventories.FirstOrDefault(inv => inv.ProductId == item.ProductId && inv.ZoneId == item.ZoneId);
                var qtyReserved = inv?.QtyReserved ?? 0;
                item.QtyAvailable = item.QtySystem - qtyReserved;
            }
        }
    }

    public IEnumerable<StockCountSession> GetByManagerWarehouse(Guid managerId)
    {
        // Manager: View sessions for their warehouses that are Completed, Approved, or Rejected
        var warehouseIds = _uow.Context.Set<Warehouse>()
            .Where(w => w.ManagerId == managerId)
            .Select(w => w.Id)
            .ToList();

        var sessions = _uow.Context.Set<StockCountSession>()
            .Include(s => s.Status)
            .Include(s => s.Warehouse)
            .Include(s => s.AssignedToNavigation)
            .Include(s => s.ApprovedByNavigation)
            .Include(s => s.StockCountItems)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(s => warehouseIds.Contains(s.WarehouseId) && 
                        (s.StatusId == StatusCompleted || s.StatusId == StatusApproved || s.StatusId == StatusRejected))
            .OrderByDescending(s => s.CompletedAt ?? s.CreatedAt)
            .ToList();

        PopulateScheduleTitles(sessions);
        PopulateQtyAvailable(sessions);

        return sessions;
    }

    // ─── Approval Actions (Manager) ───────────────────────────────────────────
    public void Approve(Guid sessionId, Guid managerId)
    {
        var session = _uow.Context.Set<StockCountSession>()
            .Include(s => s.StockCountItems)
            .FirstOrDefault(s => s.Id == sessionId) 
            ?? throw new Exception("Không tìm thấy phiếu kiểm kho.");

        if (session.StatusId != StatusCompleted)
            throw new InvalidOperationException("Chỉ có thể duyệt phiếu ở trạng thái Hoàn thành.");

        // Adjust actual inventory entries based on QtyCounted
        var inventoryDb = _uow.Context.Set<Inventory>();
        foreach (var item in session.StockCountItems)
        {
            if (!item.QtyCounted.HasValue) continue; // Skip if it wasn't counted (shouldn't happen but safe-guard)

            var inv = inventoryDb.FirstOrDefault(i => 
                          i.WarehouseId == session.WarehouseId && 
                          i.ProductId == item.ProductId && 
                          i.ZoneId == item.ZoneId);

            if (inv != null)
            {
                inv.QtyOnHand = item.QtyCounted.Value;
                inv.AvgCost = item.UnitCost; // Update price/cost as requested
                inv.LastUpdated = DateTimeOffset.UtcNow;
                inventoryDb.Update(inv);
            }
            else
            {
                // Create new inventory record if it doesn't exist (e.g., found new product during count)
                var newInv = new Inventory
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = session.WarehouseId,
                    ProductId = item.ProductId,
                    ZoneId = item.ZoneId,
                    QtyOnHand = item.QtyCounted.Value,
                    QtyReserved = 0,
                    QtyIncoming = 0,
                    AvgCost = item.UnitCost,
                    LastUpdated = DateTimeOffset.UtcNow
                };
                inventoryDb.Add(newInv);
            }
        }

        // Ghi StockTransaction cho mỗi chênh lệch (variance)
        var txnDb = _uow.Context.Set<StockTransaction>();
        var now   = DateTimeOffset.UtcNow;
        foreach (var item in session.StockCountItems.Where(i => i.QtyCounted.HasValue))
        {
            var variance = item.QtyCounted!.Value - item.QtySystem;
            if (variance == 0) continue;

            txnDb.Add(new StockTransaction
            {
                Id          = Guid.NewGuid(),
                TxnCode     = $"TXN-VAR-{now:yyMMddHHmmss}-{item.ProductId.ToString()[..4].ToUpper()}",
                TxnTypeId   = 3, // stock_adjustment
                ProductId   = item.ProductId,
                WarehouseId = session.WarehouseId,
                ZoneId      = item.ZoneId,
                QtyChange   = variance,
                QtyBalance  = item.QtyCounted.Value,
                UnitCost    = item.UnitCost,
                RefType     = "STOCK_COUNT",
                RefId       = session.Id,
                RefItemId   = item.Id,
                PerformedBy = managerId,
                TxnAt       = now,
                Note        = $"Điều chỉnh kiểm kho {session.SessionCode}: chênh lệch {variance:+#;-#;0}",
            });
        }

        session.StatusId   = StatusApproved;
        session.ApprovedBy = managerId;
        session.ApprovedAt = now;

        _uow.Context.Set<StockCountSession>().Update(session);
        _uow.Save();
    }

    public void Reject(Guid sessionId, Guid managerId, string reason)
    {
        var session = _uow.Context.Set<StockCountSession>()
            .FirstOrDefault(s => s.Id == sessionId)
            ?? throw new Exception("Không tìm thấy phiếu kiểm kho.");

        if (session.StatusId != StatusCompleted)
            throw new InvalidOperationException("Chỉ có thể từ chối phiếu ở trạng thái Hoàn thành.");

        session.StatusId = StatusRejected;
        session.ApprovedBy = managerId;    // Record who rejected it
        session.ApprovedAt = DateTimeOffset.UtcNow;
        session.Notes = string.IsNullOrWhiteSpace(session.Notes) 
            ? $"[Từ chối] {reason}" 
            : $"{session.Notes}\n[Từ chối] {reason}";

        _uow.Context.Set<StockCountSession>().Update(session);
        _uow.Save();
    }
}
