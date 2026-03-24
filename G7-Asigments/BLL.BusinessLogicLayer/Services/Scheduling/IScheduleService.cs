using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.Scheduling;

public interface IScheduleService
{
    // ─── Queries ─────────────────────────────────────────────────────────────
    IEnumerable<Schedule> GetAll();
    IEnumerable<Schedule> GetByWarehouse(Guid warehouseId);
    IEnumerable<Schedule> GetByManagerWarehouse(Guid managerId);
    IEnumerable<Schedule> GetAssignedTo(Guid userId);
    IEnumerable<Schedule> GetAllAssignedTo(Guid userId);
    IEnumerable<Schedule> GetByDateRange(DateTime from, DateTime to);
    IEnumerable<Schedule> GetByMonth(int year, int month);
    IEnumerable<Schedule> Search(string keyword);
    Schedule? GetById(Guid id);

    // ─── Conflict Detection ───────────────────────────────────────────────────
    IEnumerable<Schedule> CheckConflicts(Guid warehouseId, Guid? assignedTo,
        DateTimeOffset start, DateTimeOffset end, Guid? excludeId = null);

    // ─── CRUD ────────────────────────────────────────────────────────────────
    void Create(Schedule schedule);
    void Update(Schedule schedule);
    void Delete(Guid id);

    // ─── Status Machine ───────────────────────────────────────────────────────
    // scheduled → in_progress (Staff được assign)
    void StartSchedule(Guid id, Guid userId);

    // in_progress → done (Staff được assign)
    void CompleteSchedule(Guid id, Guid userId);

    // scheduled → missed (Manager/Admin — khi lịch quá hạn mà chưa bắt đầu)
    void MarkMissed(Guid id, Guid markedByUserId);

    // Any → cancelled (Manager/Admin) — gửi notification nếu đã assign
    void CancelSchedule(Guid id, Guid cancelledByUserId, string reason);

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    IEnumerable<LkpScheduleType> GetScheduleTypes();
    IEnumerable<Warehouse> GetWarehouses();
    IEnumerable<User> GetStaffUsers();
}
