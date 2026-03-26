using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.WarehouseConfig;

public class WarehouseStaffService : IWarehouseStaffService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<WarehouseStaff> GetByWarehouse(Guid warehouseId)
    {
        var rows = _uow.WarehouseStaffs
            .Find(ws => ws.WarehouseId == warehouseId)
            .ToList();

        if (rows.Count == 0) return rows;

        var userIds = rows.Select(r => r.UserId).ToHashSet();
        var users   = _uow.Users.Find(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id);

        foreach (var row in rows)
            if (users.TryGetValue(row.UserId, out var u))
                row.User = u;

        return rows.OrderBy(r => r.User?.FullName);
    }

    public IEnumerable<User> GetAvailableStaff(Guid warehouseId)
    {
        var assigned = _uow.WarehouseStaffs
            .Find(ws => ws.WarehouseId == warehouseId)
            .Select(ws => ws.UserId)
            .ToHashSet();

        // Role 3 = Staff, Role 2 = Manager (cả hai đều có thể được gán)
        return _uow.Users
            .Find(u => u.IsActive && (u.RoleId == 2 || u.RoleId == 3) && !assigned.Contains(u.Id))
            .OrderBy(u => u.FullName);
    }

    public void Assign(Guid warehouseId, Guid userId, Guid assignedBy)
    {
        bool exists = _uow.WarehouseStaffs
            .Find(ws => ws.WarehouseId == warehouseId && ws.UserId == userId)
            .Any();

        if (exists)
            throw new InvalidOperationException("Nhân viên này đã được gán vào kho.");

        _uow.WarehouseStaffs.Add(new WarehouseStaff
        {
            WarehouseId = warehouseId,
            UserId      = userId,
            AssignedBy  = assignedBy,
            AssignedAt  = DateTimeOffset.UtcNow,
        });
        _uow.Save();
    }

    public void Remove(Guid warehouseId, Guid userId)
    {
        var row = _uow.WarehouseStaffs
            .Find(ws => ws.WarehouseId == warehouseId && ws.UserId == userId)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Không tìm thấy phân công cần xóa.");

        _uow.WarehouseStaffs.Delete(row);
        _uow.Save();
    }
}
