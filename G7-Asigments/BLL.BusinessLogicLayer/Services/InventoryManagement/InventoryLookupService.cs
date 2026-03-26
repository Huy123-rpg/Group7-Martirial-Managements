using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public class InventoryLookupService : IInventoryLookupService
{
    private readonly UnitOfWork _uow;

    public InventoryLookupService()
    {
        _uow = UnitOfWork.Instance;
    }

    public IEnumerable<Warehouse> GetPermittedWarehouses(Guid userId, byte roleId)
    {
        var query = _uow.Context.Set<Warehouse>().AsNoTracking().Where(w => w.IsActive);

        if (roleId == 2) // Manager
        {
            query = query.Where(w => w.ManagerId == userId);
        }
        else if (roleId == 3) // Staff — dùng warehouse_staff làm nguồn phân quyền
        {
            var permittedWarehouseIds = _uow.Context.Set<WarehouseStaff>()
                .AsNoTracking()
                .Where(ws => ws.UserId == userId)
                .Select(ws => ws.WarehouseId)
                .Distinct();

            query = query.Where(w => permittedWarehouseIds.Contains(w.Id));
        }
        else if (roleId == 5) // Supplier
        {
            return Enumerable.Empty<Warehouse>();
        }
        // Admin (1) and Accountant (4) see all active warehouses.

        return query.OrderBy(w => w.Name).ToList();
    }

    public IEnumerable<Inventory> GetInventories(Guid userId, byte roleId, Guid? warehouseId = null, string searchTerm = "")
    {
        if (roleId == 5) return Enumerable.Empty<Inventory>(); // Supplier cannot view inventory

        var query = _uow.Context.Set<Inventory>()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Include(i => i.Zone)
            .AsNoTracking();

        // 1. Permission Check
        if (roleId == 2) // Manager
        {
            query = query.Where(i => i.Warehouse.ManagerId == userId);
        }
        else if (roleId == 3) // Staff — dùng warehouse_staff làm nguồn phân quyền
        {
            var permittedWarehouseIds = _uow.Context.Set<WarehouseStaff>()
                .AsNoTracking()
                .Where(ws => ws.UserId == userId)
                .Select(ws => ws.WarehouseId)
                .Distinct();

            query = query.Where(i => permittedWarehouseIds.Contains(i.WarehouseId));
        }

        // 2. Filter by selected Warehouse
        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(i => i.WarehouseId == warehouseId.Value);
        }

        // 3. Search Term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(i => 
                i.Product.Sku.ToLower().Contains(lowerSearch) ||
                i.Product.ProductName.ToLower().Contains(lowerSearch) ||
                i.Warehouse.Name.ToLower().Contains(lowerSearch) ||
                (i.Zone != null && i.Zone.ZoneCode.ToLower().Contains(lowerSearch))
            );
        }

        return query.OrderBy(i => i.Product.ProductName)
                    .ThenBy(i => i.Warehouse.Name)
                    .ToList();
    }
}
