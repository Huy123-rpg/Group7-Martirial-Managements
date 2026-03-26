using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public class StockCountExecutionService : IStockCountExecutionService
{
    private readonly UnitOfWork _uow;

    public StockCountExecutionService()
    {
        _uow = UnitOfWork.Instance;
    }

    public IEnumerable<StockCountSession> GetAssignedSessions(Guid staffId)
    {
        // Get sessions assigned to this staff member
        // Staff should see Pending (2), In_Progress (3), Approved (4), Completed (6).
        // Exclude Draft (1) and Cancelled (5).
        return _uow.Context.Set<StockCountSession>()
            .Include(s => s.Warehouse)
            .Include(s => s.Zone)
            .Include(s => s.Status)
            .AsNoTracking()
            .Where(s => s.AssignedTo == staffId && s.StatusId != 1 && s.StatusId != 5)
            .OrderByDescending(s => s.PlannedDate)
            .ThenByDescending(s => s.CreatedAt)
            .ToList();
    }

    public IEnumerable<StockCountItem> GetSessionItems(Guid sessionId)
    {
        var items = _uow.Context.Set<StockCountItem>()
            .Include(i => i.Product)
            .Include(i => i.Zone)
            .AsNoTracking()
            .Where(i => i.SessionId == sessionId)
            .OrderBy(i => i.Product.ProductName)
            .ToList();

        // Get QtyReserved from Inventory for these items to calculate QtyAvailable
        var session = _uow.Context.Set<StockCountSession>().AsNoTracking().FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var inventories = _uow.Context.Set<Inventory>().AsNoTracking()
                .Where(inv => inv.WarehouseId == session.WarehouseId && productIds.Contains(inv.ProductId))
                .ToList();

            foreach (var item in items)
            {
                var inv = inventories.FirstOrDefault(inv => inv.ProductId == item.ProductId && inv.ZoneId == item.ZoneId);
                var qtyReserved = inv?.QtyReserved ?? 0;
                item.QtyAvailable = item.QtySystem - qtyReserved;
            }
        }

        return items;
    }

    public void SaveCount(Guid sessionId, IEnumerable<StockCountItem> items, bool isComplete, Guid staffId, string? notes = null)
    {
        var session = _uow.Context.Set<StockCountSession>()
            .Include(s => s.Warehouse)
            .FirstOrDefault(s => s.Id == sessionId);
        if (session == null) throw new Exception("Session not found");

        // Allowed to edit if status is 2 (Pending)
        if (session.StatusId == 3 || session.StatusId == 4 || session.StatusId == 5 || session.StatusId == 6)
        {
            throw new Exception("Phiên kiểm kho này đã chốt hoặc bị huỷ, không thể chỉnh sửa.");
        }

        var dbItems = _uow.Context.Set<StockCountItem>().Where(i => i.SessionId == sessionId).ToList();
        
        foreach (var item in items)
        {
            var dbItem = dbItems.FirstOrDefault(d => d.Id == item.Id);
            if (dbItem != null)
            {
                dbItem.QtyCounted = item.QtyCounted;
                dbItem.CountedBy = staffId;
                dbItem.CountedAt = DateTimeOffset.UtcNow;
                
                // Variance is calculated in the database via PERSISTED computed column.
                // QtyVariance = QtyCounted - QtySystem, VarianceValue = QtyVariance * UnitCost
            }
        }

        session.Notes = notes; // Save Staff Notes

        if (isComplete)
        {
            session.StatusId = 6; // 6 = COMPLETED (Awaiting manager approval)
            session.CompletedAt = DateTimeOffset.UtcNow;
            
            // Send IN_APP Notification to Manager
            if (session.Warehouse?.ManagerId != null)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = session.Warehouse.ManagerId.Value,
                    Title = "Có phiếu kiểm kho chờ duyệt",
                    Body = $"Khu vực/Kho {session.Warehouse.Name} đã đếm xong (Phiếu {session.SessionCode}). Vui lòng kiểm tra và duyệt.",
                    Channel = "IN_APP",
                    RefType = "STOCK_COUNT_APPROVAL",
                    RefId = session.Id,
                    IsRead = false,
                    SentAt = DateTimeOffset.UtcNow,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                _uow.Context.Set<Notification>().Add(notification);
            }
        }

        _uow.Save();
    }
}
