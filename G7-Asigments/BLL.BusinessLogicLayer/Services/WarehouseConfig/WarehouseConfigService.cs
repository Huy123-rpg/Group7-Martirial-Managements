using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.BusinessLogicLayer.Services.WarehouseConfig;

public class WarehouseConfigService : IWarehouseConfigService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── Warehouse ────────────────────────────────────────────────────────────
    public IEnumerable<Warehouse> GetAllWarehouses()
    {
        var warehouses = _uow.Warehouses.GetAll().OrderBy(w => w.Name).ToList();
        EnrichWarehouses(warehouses);
        return warehouses;
    }

    public Warehouse? GetWarehouseById(Guid id)
    {
        var w = _uow.Warehouses.GetById(id);
        if (w == null) return null;
        EnrichWarehouses([w]);
        return w;
    }

    public void CreateWarehouse(Warehouse w)
    {
        if (string.IsNullOrWhiteSpace(w.Code))
            throw new ArgumentException("Mã kho không được để trống.");
        if (string.IsNullOrWhiteSpace(w.Name))
            throw new ArgumentException("Tên kho không được để trống.");

        bool codeExists = _uow.Warehouses.Find(x => x.Code == w.Code.Trim().ToUpperInvariant()).Any();
        if (codeExists) throw new InvalidOperationException("Mã kho đã tồn tại.");

        w.Id        = Guid.NewGuid();
        w.Code      = w.Code.Trim().ToUpperInvariant();
        w.Name      = w.Name.Trim();
        w.IsActive  = true;
        w.CreatedAt = DateTimeOffset.UtcNow;
        w.UpdatedAt = DateTimeOffset.UtcNow;

        _uow.Warehouses.Add(w);
        _uow.Save();
    }

    public void UpdateWarehouse(Warehouse w)
    {
        var existing = _uow.Warehouses.GetById(w.Id)
            ?? throw new InvalidOperationException("Không tìm thấy kho.");

        // check duplicate code (excluding self)
        bool codeExists = _uow.Warehouses
            .Find(x => x.Code == w.Code.Trim().ToUpperInvariant() && x.Id != w.Id)
            .Any();
        if (codeExists) throw new InvalidOperationException("Mã kho đã tồn tại.");

        existing.Code      = w.Code.Trim().ToUpperInvariant();
        existing.Name      = w.Name.Trim();
        existing.Address   = w.Address;
        existing.City      = w.City;
        existing.ManagerId = w.ManagerId;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        _uow.Warehouses.Update(existing);
        _uow.Save();
    }

    public void ToggleWarehouseActive(Guid id)
    {
        var w = _uow.Warehouses.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy kho.");
        w.IsActive  = !w.IsActive;
        w.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Warehouses.Update(w);
        _uow.Save();
    }

    public void DeleteWarehouse(Guid id)
    {
        var w = _uow.Warehouses.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy kho.");
        
        var ctx = _uow.Context;
        
        // Manual Cascade Delete replacing the strict validation
        ctx.Set<Inventory>().Where(i => i.WarehouseId == id).ExecuteDelete();
        ctx.Set<GoodsReceipt>().Where(g => g.WarehouseId == id).ExecuteDelete();
        ctx.Set<GoodsIssue>().Where(g => g.WarehouseId == id).ExecuteDelete();
        ctx.Set<PurchaseOrder>().Where(p => p.WarehouseId == id).ExecuteDelete();
        ctx.Set<SalesOrder>().Where(s => s.WarehouseId == id).ExecuteDelete();
        ctx.Set<StockAdjustment>().Where(s => s.WarehouseId == id).ExecuteDelete();
        ctx.Set<StockCountSession>().Where(s => s.WarehouseId == id).ExecuteDelete();
        ctx.Set<StockTransaction>().Where(s => s.WarehouseId == id).ExecuteDelete();
        ctx.Set<DAL.DataAccessLayer.Models.StockTransfer>().Where(s => s.FromWarehouseId == id || s.ToWarehouseId == id).ExecuteDelete();
        
        ctx.Set<Schedule>().Where(s => s.WarehouseId == id).ExecuteDelete();
        ctx.Set<WarehouseZone>().Where(z => z.WarehouseId == id).ExecuteDelete();

        _uow.Warehouses.DeleteById(w.Id);
        _uow.Save();
    }

    // ─── Zone ─────────────────────────────────────────────────────────────────
    public IEnumerable<WarehouseZone> GetZonesByWarehouse(Guid warehouseId)
    {
        var zones = _uow.WarehouseZones
            .Find(z => z.WarehouseId == warehouseId)
            .OrderBy(z => z.ZoneCode)
            .ToList();
        EnrichZones(zones);
        return zones;
    }

    public WarehouseZone? GetZoneById(Guid id) => _uow.WarehouseZones.GetById(id);

    public void CreateZone(WarehouseZone z)
    {
        if (string.IsNullOrWhiteSpace(z.ZoneCode))
            throw new ArgumentException("Mã zone không được để trống.");

        bool codeExists = _uow.WarehouseZones
            .Find(x => x.WarehouseId == z.WarehouseId &&
                       x.ZoneCode == z.ZoneCode.Trim().ToUpperInvariant())
            .Any();
        if (codeExists) throw new InvalidOperationException("Mã zone đã tồn tại trong kho này.");

        z.Id       = Guid.NewGuid();
        z.ZoneCode = z.ZoneCode.Trim().ToUpperInvariant();
        z.IsActive = true;

        _uow.WarehouseZones.Add(z);
        _uow.Save();
    }

    public void UpdateZone(WarehouseZone z)
    {
        var existing = _uow.WarehouseZones.GetById(z.Id)
            ?? throw new InvalidOperationException("Không tìm thấy zone.");

        bool codeExists = _uow.WarehouseZones
            .Find(x => x.WarehouseId == existing.WarehouseId &&
                       x.ZoneCode == z.ZoneCode.Trim().ToUpperInvariant() &&
                       x.Id != z.Id)
            .Any();
        if (codeExists) throw new InvalidOperationException("Mã zone đã tồn tại trong kho này.");

        existing.ZoneCode      = z.ZoneCode.Trim().ToUpperInvariant();
        existing.ZoneName      = z.ZoneName;
        existing.ZoneType      = z.ZoneType;
        existing.CapacityM3    = z.CapacityM3;
        existing.CapacityPallet = z.CapacityPallet;

        _uow.WarehouseZones.Update(existing);
        _uow.Save();
    }

    public void ToggleZoneActive(Guid id)
    {
        var z = _uow.WarehouseZones.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy zone.");
        z.IsActive = !z.IsActive;
        _uow.WarehouseZones.Update(z);
        _uow.Save();
    }

    public void DeleteZone(Guid id)
    {
        var z = _uow.WarehouseZones.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy zone.");
        
        _uow.WarehouseZones.DeleteById(z.Id);
        _uow.Save();
    }

    // ─── Lookups ──────────────────────────────────────────────────────────────
    public IEnumerable<LkpZoneType>      GetZoneTypes()      => _uow.ZoneTypes.GetAll().OrderBy(t => t.TypeName);
    public IEnumerable<LkpCostingMethod> GetCostingMethods() => _uow.CostingMethods.GetAll().OrderBy(m => m.MethodName);
    public IEnumerable<User>             GetManagers()       =>
        _uow.Users.Find(u => u.IsActive && (u.RoleId == 1 || u.RoleId == 2))
                  .OrderBy(u => u.FullName);

    // ─── Enrichment ───────────────────────────────────────────────────────────
    private void EnrichWarehouses(IEnumerable<Warehouse> list)
    {
        var items = list.ToList();
        var mgrIds = items.Where(w => w.ManagerId.HasValue).Select(w => w.ManagerId!.Value).ToHashSet();
        var mgrDict = _uow.Users.Find(u => mgrIds.Contains(u.Id)).ToDictionary(u => u.Id);
        var methodDict = _uow.CostingMethods.GetAll().ToDictionary(m => m.MethodId);

        foreach (var w in items)
        {
            if (w.ManagerId.HasValue && mgrDict.TryGetValue(w.ManagerId.Value, out var mgr))
                w.Manager = mgr;
            if (methodDict.TryGetValue(w.CostingMethod, out var method))
                w.CostingMethodNavigation = method;
        }
    }

    private void EnrichZones(IEnumerable<WarehouseZone> list)
    {
        var typeDict = _uow.ZoneTypes.GetAll().ToDictionary(t => t.TypeId);
        foreach (var z in list)
        {
            if (typeDict.TryGetValue(z.ZoneType, out var t))
                z.ZoneTypeNavigation = t;
        }
    }
}
