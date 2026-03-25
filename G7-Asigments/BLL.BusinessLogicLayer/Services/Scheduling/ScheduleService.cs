using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Email;
using DAL.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        Enrich(_uow.Schedules.GetAll().OrderBy(s => s.StartTime));

    public IEnumerable<Schedule> GetByWarehouse(Guid warehouseId) =>
        Enrich(_uow.Schedules.Find(s => s.WarehouseId == warehouseId)
                             .OrderBy(s => s.StartTime));

    public IEnumerable<Schedule> GetByManagerWarehouse(Guid managerId)
    {
        var warehouseIds = _uow.Warehouses
            .Find(w => w.ManagerId == managerId)
            .Select(w => w.Id)
            .ToHashSet();

        return Enrich(_uow.Schedules
            .Find(s => s.WarehouseId.HasValue && warehouseIds.Contains(s.WarehouseId.Value))
            .OrderBy(s => s.StartTime));
    }

    public IEnumerable<Schedule> GetAssignedTo(Guid userId) =>
        Enrich(_uow.Schedules
            .Find(s => s.AssignedTo == userId &&
                       s.StatusCode != StatusCancelled &&
                       s.StatusCode != StatusCompleted &&
                       s.StatusCode != StatusMissed)
            .OrderBy(s => s.StartTime));

    public IEnumerable<Schedule> GetAllAssignedTo(Guid userId) =>
        Enrich(_uow.Schedules
            .Find(s => s.AssignedTo == userId)
            .OrderByDescending(s => s.StartTime));

    public IEnumerable<Schedule> GetByDateRange(DateTime from, DateTime to) =>
        Enrich(_uow.Schedules
            .Find(s => s.StartTime >= (DateTimeOffset)from && s.StartTime <= (DateTimeOffset)to)
            .OrderBy(s => s.StartTime));

    public IEnumerable<Schedule> GetByMonth(int year, int month)
    {
        var from = new DateTimeOffset(new DateTime(year, month, 1), TimeSpan.Zero);
        var to   = from.AddMonths(1);
        return Enrich(_uow.Schedules.Find(s =>
            s.StartTime < to &&
            (!s.EndTime.HasValue || s.EndTime.Value >= from))
            .OrderBy(s => s.StartTime));
    }

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
        {
            _uow.Notifications.Add(BuildAssignNotification(template));
            CheckAndCreateStockCount(template);
        }

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
            {
                _uow.Notifications.Add(BuildAssignNotification(instance));
                CheckAndCreateStockCount(instance);
            }
        }
        _uow.Save();

        // Chỉ gửi mail 1 lần (dùng template vì nó có đủ thông tin)
        if (template.AssignedTo.HasValue)
            SendAssignmentEmail(template);
    }

    private void CheckAndCreateStockCount(Schedule schedule)
    {
        if (!schedule.AssignedTo.HasValue || !schedule.WarehouseId.HasValue) return;

        var type = _uow.ScheduleTypes.Find(t => t.TypeId == schedule.ScheduleType).FirstOrDefault();
        if (type != null && (type.TypeCode == "STOCK_COUNT" || type.TypeName.Contains("Kiểm kho", StringComparison.OrdinalIgnoreCase)))
        {
            var sessionId = Guid.NewGuid();
            var sessionCode = "SC-" + DateTime.Now.ToString("yyMMddHHmm") + "-" + new Random().Next(100, 999);

            var session = new StockCountSession
            {
                Id = sessionId,
                SessionCode = sessionCode,
                WarehouseId = schedule.WarehouseId.Value,
                StatusId = 2, // PENDING
                CreatedBy = schedule.CreatedBy,
                AssignedTo = schedule.AssignedTo,
                PlannedDate = DateOnly.FromDateTime(schedule.StartTime.Date),
                CreatedAt = DateTimeOffset.UtcNow,
                Notes = $"Phiếu kiểm kho tự động tạo từ lịch công việc: {schedule.Title}"
            };

            schedule.RefType = "STOCK_COUNT";
            schedule.RefId = sessionId;

            _uow.Context.Set<StockCountSession>().Add(session);

            // Add auto Stock Count items for ALL products currently in that warehouse
            var inventories = _uow.Context.Set<Inventory>()
                .Include(i => i.Product)
                .Where(i => i.WarehouseId == schedule.WarehouseId.Value && i.QtyOnHand > 0)
                .ToList();

            foreach (var inv in inventories)
            {
                var item = new StockCountItem
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    ProductId = inv.ProductId,
                    ZoneId = inv.ZoneId,
                    QtySystem = inv.QtyOnHand,
                    UnitCost = inv.AvgCost
                };
                _uow.Context.Set<StockCountItem>().Add(item);
            }

            _uow.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = schedule.AssignedTo.Value,
                Title = "📦 Nhiệm vụ: Thực hiện Kiểm kho",
                Body = $"Một phiên kiểm kho ({sessionCode}) đã tự động được khởi tạo tại Kho của bạn theo lịch phân công. Vui lòng vào Cột chức năng [Thực hiện kiểm kho] để bắt đầu ghi nhận số liệu.",
                Channel = "IN_APP",
                RefType = "STOCK_COUNT",
                RefId = sessionId,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
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

    // ─── Status Machine ───────────────────────────────────────────────────────
    public void StartSchedule(Guid id, Guid userId)
    {
        var schedule = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (schedule.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ bắt đầu được lịch đang ở trạng thái Đã lên lịch.");

        if (schedule.AssignedTo != userId)
            throw new UnauthorizedAccessException("Bạn không được phân công lịch này.");

        schedule.StatusCode = StatusInProgress;
        schedule.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);
        _uow.Save();
    }

    public void CompleteSchedule(Guid id, Guid userId)
    {
        var schedule = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (schedule.StatusCode != StatusInProgress)
            throw new InvalidOperationException("Chỉ hoàn thành được lịch đang ở trạng thái Đang thực hiện.");

        if (schedule.AssignedTo != userId)
            throw new UnauthorizedAccessException("Bạn không được phân công lịch này.");

        schedule.StatusCode = StatusCompleted;
        schedule.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);
        _uow.Save();
    }

    public void MarkMissed(Guid id, Guid markedByUserId)
    {
        var schedule = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (schedule.StatusCode != StatusDraft)
            throw new InvalidOperationException("Chỉ đánh dấu bỏ lỡ được lịch ở trạng thái Đã lên lịch.");

        if (schedule.StartTime > DateTimeOffset.Now)
            throw new InvalidOperationException("Chưa thể đánh dấu bỏ lỡ — lịch chưa đến thời gian bắt đầu.");

        schedule.StatusCode = StatusMissed;
        schedule.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);

        if (schedule.AssignedTo.HasValue)
        {
            _uow.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid(),
                UserId    = schedule.AssignedTo.Value,
                Title     = "Lịch công việc bị đánh dấu bỏ lỡ",
                Body      = $"Lịch \"{schedule.Title}\" ngày {schedule.StartTime:dd/MM/yyyy HH:mm} đã bị đánh dấu là bỏ lỡ.",
                Channel   = "IN_APP",
                RefType   = "SCHEDULE",
                RefId     = schedule.Id,
                IsRead    = false,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        _uow.Save();
    }

    public void CancelSchedule(Guid id, Guid cancelledByUserId, string reason)
    {
        var schedule = _uow.Schedules.GetById(id)
            ?? throw new InvalidOperationException("Không tìm thấy lịch.");

        if (schedule.StatusCode == StatusCompleted)
            throw new InvalidOperationException("Không thể hủy lịch đã hoàn thành.");

        if (schedule.StatusCode == StatusCancelled)
            throw new InvalidOperationException("Lịch này đã bị hủy trước đó.");

        bool wasAssigned = schedule.AssignedTo.HasValue;

        schedule.StatusCode = StatusCancelled;
        schedule.Metadata   = $"{{\"cancelReason\":\"{reason.Replace("\"", "\\\"")}\",\"cancelledBy\":\"{cancelledByUserId}\"}}";
        schedule.UpdatedAt  = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);

        if (wasAssigned)
        {
            _uow.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid(),
                UserId    = schedule.AssignedTo!.Value,
                Title     = "Lịch công việc đã bị hủy",
                Body      = $"Lịch \"{schedule.Title}\" ngày {schedule.StartTime:dd/MM/yyyy HH:mm} đã bị hủy. Lý do: {reason}",
                Channel   = "IN_APP",
                RefType   = "SCHEDULE",
                RefId     = schedule.Id,
                IsRead    = false,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        _uow.Save();
    }

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    public IEnumerable<LkpScheduleType> GetScheduleTypes() =>
        _uow.ScheduleTypes.GetAll().OrderBy(t => t.TypeName);

    public IEnumerable<Warehouse> GetWarehouses() =>
        _uow.Warehouses.Find(w => w.IsActive).OrderBy(w => w.Name);

    public IEnumerable<User> GetStaffUsers() =>
        _uow.Users.Find(u => u.IsActive && u.RoleId == 3)
                  .OrderBy(u => u.FullName);

    // ─── Private Helpers ──────────────────────────────────────────────────────
    private IEnumerable<Schedule> Enrich(IEnumerable<Schedule> items)
    {
        var list = items.ToList();
        if (list.Count == 0) return list;

        var whIds   = list.Where(s => s.WarehouseId.HasValue).Select(s => s.WarehouseId!.Value).ToHashSet();
        var userIds = list.Where(s => s.AssignedTo.HasValue).Select(s => s.AssignedTo!.Value).ToHashSet();

        var whDict   = _uow.Warehouses.Find(w => whIds.Contains(w.Id)).ToDictionary(w => w.Id);
        var userDict = _uow.Users.Find(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id);
        var typeDict = _uow.ScheduleTypes.GetAll().ToDictionary(t => t.TypeId);

        foreach (var s in list)
        {
            if (s.WarehouseId.HasValue && whDict.TryGetValue(s.WarehouseId.Value, out var wh))
                s.Warehouse = wh;
            if (s.AssignedTo.HasValue && userDict.TryGetValue(s.AssignedTo.Value, out var u))
                s.AssignedToNavigation = u;
            if (typeDict.TryGetValue(s.ScheduleType, out var t))
                s.ScheduleTypeNavigation = t;
        }

        return list;
    }

    private void SendAssignmentEmail(Schedule schedule)
    {
        var user = _uow.Users.Find(u => u.Id == schedule.AssignedTo!.Value).FirstOrDefault();
        if (user == null) return;

        var warehouseName = schedule.WarehouseId.HasValue
            ? _uow.Warehouses.Find(w => w.Id == schedule.WarehouseId.Value).FirstOrDefault()?.Name ?? ""
            : "";

        var emailService = new EmailService();
        _ = Task.Run(async () =>
        {
            try
            {
                await emailService.SendScheduleAssignmentEmailAsync(
                    user.Email, user.FullName,
                    schedule.Title, schedule.StartTime,
                    warehouseName, schedule.Description);
            }
            catch { /* Email failure không được làm gián đoạn luồng chính */ }
        });
    }

    private static Notification BuildAssignNotification(Schedule s) => new()
    {
        Id        = Guid.NewGuid(),
        UserId    = s.AssignedTo!.Value,
        Title     = "Bạn có lịch công việc mới",
        Body      = $"Lịch \"{s.Title}\" bắt đầu lúc {s.StartTime:dd/MM/yyyy HH:mm} đã được tạo và phân công cho bạn.",
        Channel   = "IN_APP",
        RefType   = "SCHEDULE",
        RefId     = s.Id,
        IsRead    = false,
        CreatedAt = DateTimeOffset.UtcNow,
    };

    private static void ValidateScheduleTimes(Schedule s)
    {
        if (s.EndTime.HasValue && s.EndTime.Value <= s.StartTime)
            throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu.");
    }
}
