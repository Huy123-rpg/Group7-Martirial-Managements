using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public interface IInventoryLookupService
{
    IEnumerable<Warehouse> GetPermittedWarehouses(Guid userId, byte roleId);
    IEnumerable<Inventory> GetInventories(Guid userId, byte roleId, Guid? warehouseId = null, string searchTerm = "");
}
