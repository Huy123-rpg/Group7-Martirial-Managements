using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.WarehouseConfig;

public interface IWarehouseStaffService
{
    /// <summary>Lấy danh sách staff đang được gán vào kho (có load User).</summary>
    IEnumerable<WarehouseStaff> GetByWarehouse(Guid warehouseId);

    /// <summary>Lấy danh sách user có role Staff (3) chưa được gán vào kho này.</summary>
    IEnumerable<User> GetAvailableStaff(Guid warehouseId);

    /// <summary>Gán một user vào kho. Ném exception nếu đã tồn tại.</summary>
    void Assign(Guid warehouseId, Guid userId, Guid assignedBy);

    /// <summary>Gỡ user khỏi kho.</summary>
    void Remove(Guid warehouseId, Guid userId);
}
