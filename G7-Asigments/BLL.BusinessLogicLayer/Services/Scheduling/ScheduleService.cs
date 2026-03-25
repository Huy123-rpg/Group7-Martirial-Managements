using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Scheduling;

public class ScheduleService : IScheduleService
{
    // ─── Status Constants ────────────────────────────────────────────────────
    public const string StatusDraft      = "scheduled";
    public const string StatusInProgress = "in_progress";
    public const string StatusCompleted  = "done";
    public const string StatusCancelled  = "cancelled";
    public const string StatusMissed     = "missed";

    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── Queries ─────────────────────────────────────────────────────────────
    public IEnumerable<Schedule> GetAll() =>
        _uow.Schedules.GetAll().OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetByWarehouse(Guid warehouseId) =>
        _uow.Schedules.Find(s => s.WarehouseId == warehouseId)
                      .OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetByManagerWarehouse(Guid managerId) =>
        _uow.Schedules.Find(s => s.CreatedBy == managerId)
                      .OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetAssignedTo(Guid userId) =>
        _uow.Schedules.Find(s => s.AssignedTo == userId &&
            s.StatusCode != StatusCancelled && s.StatusCode != StatusCompleted)
                      .OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetAllAssignedTo(Guid userId) =>
        _uow.Schedules.Find(s => s.AssignedTo == userId)
                      .OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetByMonth(int year, int month)
    {
        var from = new DateTimeOffset(new DateTime(year, month, 1));
        var to   = from.AddMonths(1);
        return _uow.Schedules.Find(s => s.StartTime >= from && s.StartTime < to)
                             .OrderBy(s => s.StartTime);
    }

    public IEnumerable<Schedule> Search(string keyword) =>
        _uow.Schedules.Find(s =>
            s.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (s.Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.StartTime);

    public Schedule? GetById(Guid id) => _uow.Schedules.GetById(id);

    // ─── Conflict Detection ───────────────────────────────────────────────────
    public IEnumerable<Schedule> CheckConflicts(Guid warehouseId, Guid? assignedTo,
        DateTimeOffset start, DateTimeOffset end, Guid? excludeId = null)
    {
        var activeStatuses = new[] { StatusDraft, StatusInProgress };

        return _uow.Schedules.Find(s =>
            s.Id != (excludeId ?? Guid.Empty) &&
            activeStatuses.Contains(s.StatusCode) &&
            s.StartTime < end &&
            (s.EndTime ?? s.StartTime.AddHours(1)) > start &&
            (s.WarehouseId == warehouseId ||
             (assignedTo.HasValue && s.AssignedTo == assignedTo)));
    }

    // ─── CRUD ────────────────────────────────────────────────────────────────
    public void Create(Schedule schedule)
    {
        ValidateScheduleTimes(schedule);

        if (schedule.IsRecurring && !string.IsNullOrEmpty(schedule.RecurrenceRule))
            CreateRecurring(schedule);
        else
            CreateSingle(schedule);
    }

    private void CreateSingle(Schedule template)
    {
        template.Id         = Guid.NewGuid();
        template.StatusCode = StatusDraft;
        template.CreatedAt  = DateTimeOffset.UtcNow;
        template.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Add(template);

        if (template.AssignedTo.HasValue)
            _uow.Notifications.Add(BuildAssignNotification(template));

        _uow.Save();
    }

    private void CreateRecurring(Schedule template)
    {
        var parts = template.RecurrenceRule!.Split(':');
        string freq  = parts.Length > 0 ? parts[0] : "WEEKLY";
        int    count = parts.Length > 1 && int.TryParse(parts[1], out var n) ? Math.Max(1, n) : 4;

        for (int i = 0; i < count; i++)
        {
            var (start, end) = freq switch
            {
                "DAILY"   => (template.StartTime.AddDays(i),     template.EndTime?.AddDays(i)),
                "MONTHLY" => (template.StartTime.AddMonths(i),   template.EndTime?.AddMonths(i)),
                _         => (template.StartTime.AddDays(i * 7), template.EndTime?.AddDays(i * 7)),
            };

            var instance = new Schedule
            {
                Id              = Guid.NewGuid(),
                Title           = template.Title,
                ScheduleType    = template.ScheduleType,
                StatusCode      = StatusDraft,
                WarehouseId     = template.WarehouseId,
                StartTime       = start,
                EndTime         = end,
                AssignedTo      = template.AssignedTo,
                CreatedBy       = template.CreatedBy,
                Description     = template.Description,
                RefType         = template.RefType,
                RefId           = template.RefId,
                ReminderMinutes = template.ReminderMinutes,
                IsRecurring     = true,
                RecurrenceRule  = template.RecurrenceRule,
                CreatedAt       = DateTimeOffset.UtcNow,
                UpdatedAt       = DateTimeOffset.UtcNow,
            };
            _uow.Schedules.Add(instance);

            if (i == 0 && template.AssignedTo.HasValue)
                _uow.Notifications.Add(BuildAssignNotification(instance));
        }
        _uow.Save();
    }

    public void Update(Schedule schedule)
    {
        var existing = _uow.Schedules.GetById(schedule.Id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (existing.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ được sửa lịch ở trạng thái Đã lên lịch.");

        ValidateScheduleTimes(schedule);

        schedule.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        var schedule = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (schedule.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ được xóa lịch ở trạng thái Đã lên lịch.");

        _uow.Schedules.DeleteById(id);
        _uow.Save();
    }

    public IEnumerable<Schedule> GetByDateRange(DateTime from, DateTime to)
    {
        var fromOffset = new DateTimeOffset(from);
        var toOffset = new DateTimeOffset(to);

        return _uow.Schedules.Find(s => s.StartTime >= fromOffset && s.StartTime <= toOffset)
                             .OrderBy(s => s.StartTime);
    }

    // ─── Status Machine ───────────────────────────────────────────────────────
    public void StartSchedule(Guid id, Guid userId)
    {
        var s = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");
        if (s.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ được bắt đầu lịch ở trạng thái Đã lên lịch.");
        s.StatusCode = StatusInProgress;
        s.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(s);
        _uow.Save();
    }

    public void CompleteSchedule(Guid id, Guid userId)
    {
        var s = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");
        if (s.StatusCode != StatusInProgress)
            throw new InvalidOperationException("Chỉ được hoàn thành lịch ở trạng thái Đang thực hiện.");
        s.StatusCode  = StatusCompleted;
        s.CompletedAt = DateTimeOffset.UtcNow;
        s.UpdatedAt   = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(s);
        _uow.Save();
    }

    public void MarkMissed(Guid id, Guid markedByUserId)
    {
        var s = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");
        if (s.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ đánh dấu bỏ lỡ cho lịch Đã lên lịch.");
        s.StatusCode = StatusMissed;
        s.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(s);
        _uow.Save();
    }

    public void CancelSchedule(Guid id, Guid cancelledByUserId, string reason)
    {
        var s = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");
        if (s.StatusCode == StatusCompleted || s.StatusCode == StatusCancelled || s.StatusCode == StatusMissed)
            throw new InvalidOperationException("Không thể hủy lịch này.");
        s.StatusCode       = StatusCancelled;
        s.CancellationNote = reason;
        s.UpdatedAt        = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(s);

        if (s.AssignedTo.HasValue)
        {
            _uow.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid(),
                UserId    = s.AssignedTo.Value,
                Title     = "Lịch bị hủy",
                Body      = $"Lịch \"{s.Title}\" đã bị hủy. Lý do: {reason}",
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        _uow.Save();
    }

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    public IEnumerable<LkpScheduleType> GetScheduleTypes() =>
        _uow.ScheduleTypes.GetAll();

    public IEnumerable<Warehouse> GetWarehouses() =>
        _uow.Warehouses.GetAll();

    public IEnumerable<User> GetStaffUsers() =>
        _uow.Users.Find(u => u.IsActive && u.RoleId == 3); // Staff role

    // ─── Helpers ──────────────────────────────────────────────────────────────
    private static void ValidateScheduleTimes(Schedule s)
    {
        if (s.EndTime.HasValue && s.EndTime <= s.StartTime)
            throw new InvalidOperationException("Thời gian kết thúc phải sau thời gian bắt đầu.");
    }

    private static Notification BuildAssignNotification(Schedule s) => new()
    {
        Id        = Guid.NewGuid(),
        UserId    = s.AssignedTo!.Value,
        Title     = "Bạn được phân công nhiệm vụ",
        Body      = $"Nhiệm vụ \"{s.Title}\" vào lúc {s.StartTime:dd/MM/yyyy HH:mm}.",
        CreatedAt = DateTimeOffset.UtcNow,
    };

    private void SendAssignmentEmail(Schedule s)
    {
        // TODO: Implement email notification
    }
}
