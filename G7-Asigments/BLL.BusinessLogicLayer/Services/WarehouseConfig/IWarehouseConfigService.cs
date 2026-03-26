using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.WarehouseConfig;

public interface IWarehouseConfigService
{
    // ─── Warehouse ────────────────────────────────────────────────────────────
    IEnumerable<Warehouse> GetAllWarehouses();
    Warehouse? GetWarehouseById(Guid id);
    void CreateWarehouse(Warehouse w);
    void UpdateWarehouse(Warehouse w);
    void ToggleWarehouseActive(Guid id);
    void DeleteWarehouse(Guid id);

    // ─── Zone ─────────────────────────────────────────────────────────────────
    IEnumerable<WarehouseZone> GetZonesByWarehouse(Guid warehouseId);
    WarehouseZone? GetZoneById(Guid id);
    void CreateZone(WarehouseZone z);
    void UpdateZone(WarehouseZone z);
    void ToggleZoneActive(Guid id);
    void DeleteZone(Guid id);

    // ─── Lookups ──────────────────────────────────────────────────────────────
    IEnumerable<LkpZoneType>      GetZoneTypes();
    IEnumerable<LkpCostingMethod> GetCostingMethods();
    IEnumerable<User>             GetManagers();
}
