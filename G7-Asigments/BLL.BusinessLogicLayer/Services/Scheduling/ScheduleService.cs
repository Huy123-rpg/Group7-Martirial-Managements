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

    public IEnumerable<Schedule> Search(string keyword) =>
        Enrich(_uow.Schedules.Find(s =>
            s.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (s.Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)));

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

        if (template.AssignedTo.HasValue)
            SendAssignmentEmail(template);
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

            // Chỉ gửi notification cho lần đầu
            if (i == 0 && template.AssignedTo.HasValue)
                _uow.Notifications.Add(BuildAssignNotification(instance));
        }
        _uow.Save();

        // Chỉ gửi mail 1 lần (dùng template vì nó có đủ thông tin)
        if (template.AssignedTo.HasValue)
            SendAssignmentEmail(template);
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
}
